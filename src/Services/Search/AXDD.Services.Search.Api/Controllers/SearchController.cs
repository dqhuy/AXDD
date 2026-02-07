using AXDD.Services.Search.Api.DTOs;
using AXDD.Services.Search.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Search.Api.Controllers;

/// <summary>
/// Controller for search operations
/// </summary>
[ApiController]
[Route("api/v1/search")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        ISearchService searchService,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search enterprises with filters and pagination
    /// </summary>
    /// <param name="request">Enterprise search request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    /// <response code="200">Returns search results</response>
    /// <response code="400">Invalid request</response>
    [HttpPost("enterprises")]
    [ProducesResponseType(typeof(SearchResponse<Models.EnterpriseSearchDocument>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchResponse<Models.EnterpriseSearchDocument>>> SearchEnterprises(
        [FromBody] EnterpriseSearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enterprise search request: query='{Query}', page={Page}", 
            request.Query, request.PageNumber);

        var result = await _searchService.SearchEnterprisesAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search documents with filters and pagination
    /// </summary>
    /// <param name="request">Document search request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    /// <response code="200">Returns search results</response>
    /// <response code="400">Invalid request</response>
    [HttpPost("documents")]
    [ProducesResponseType(typeof(SearchResponse<Models.DocumentSearchDocument>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchResponse<Models.DocumentSearchDocument>>> SearchDocuments(
        [FromBody] DocumentSearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Document search request: query='{Query}', page={Page}", 
            request.Query, request.PageNumber);

        var result = await _searchService.SearchDocumentsAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search projects with filters and pagination
    /// </summary>
    /// <param name="request">Project search request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    /// <response code="200">Returns search results</response>
    /// <response code="400">Invalid request</response>
    [HttpPost("projects")]
    [ProducesResponseType(typeof(SearchResponse<Models.ProjectSearchDocument>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchResponse<Models.ProjectSearchDocument>>> SearchProjects(
        [FromBody] ProjectSearchRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Project search request: query='{Query}', page={Page}", 
            request.Query, request.PageNumber);

        var result = await _searchService.SearchProjectsAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search across all indexes (enterprises, documents, projects)
    /// </summary>
    /// <param name="request">Search request with query and page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Multi-index search results</returns>
    /// <response code="200">Returns search results from all indexes</response>
    /// <response code="400">Invalid query</response>
    [HttpPost("all")]
    [ProducesResponseType(typeof(MultiSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MultiSearchResponse>> SearchAll(
        [FromBody] SearchAllRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Query cannot be empty");
        }

        _logger.LogInformation("Multi-search request: query='{Query}'", request.Query);

        var result = await _searchService.SearchAllAsync(request.Query, request.PageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get search suggestions for autocomplete
    /// </summary>
    /// <param name="q">Partial query text</param>
    /// <param name="type">Type of suggestions (enterprise, document, project)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of suggestions</returns>
    /// <response code="200">Returns suggestions</response>
    /// <response code="400">Invalid parameters</response>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(SuggestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SuggestionResponse>> GetSuggestions(
        [FromQuery] string q,
        [FromQuery] string type = "enterprise",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
        {
            return Ok(new SuggestionResponse { Query = q });
        }

        _logger.LogDebug("Suggestion request: query='{Query}', type={Type}", q, type);

        var result = await _searchService.GetSuggestionsAsync(q, type, cancellationToken);
        return Ok(result);
    }
}

/// <summary>
/// Request for multi-index search
/// </summary>
public class SearchAllRequest
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Number of results per index (max 10)
    /// </summary>
    public int PageSize { get; set; } = 5;
}
