using AXDD.Services.Search.Api.DTOs;
using AXDD.Services.Search.Api.Models;
using AXDD.Services.Search.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Search.Api.Controllers;

/// <summary>
/// Controller for index management operations (admin only)
/// </summary>
[ApiController]
[Route("api/v1/index")]
[Produces("application/json")]
// [Authorize(Roles = "Admin")] // Uncomment when authentication is enabled
public class IndexManagementController : ControllerBase
{
    private readonly IIndexManagementService _indexManagementService;
    private readonly IIndexingService _indexingService;
    private readonly ILogger<IndexManagementController> _logger;

    public IndexManagementController(
        IIndexManagementService indexManagementService,
        IIndexingService indexingService,
        ILogger<IndexManagementController> logger)
    {
        _indexManagementService = indexManagementService;
        _indexingService = indexingService;
        _logger = logger;
    }

    /// <summary>
    /// Initialize all indexes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Indexes initialized successfully</response>
    [HttpPost("initialize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> InitializeIndexes(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing all indexes");
        
        await _indexManagementService.InitializeIndexesAsync(cancellationToken);
        
        return Ok(new { message = "All indexes initialized successfully" });
    }

    /// <summary>
    /// Create a specific index
    /// </summary>
    /// <param name="indexName">Name of the index to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Index created successfully</response>
    /// <response code="400">Invalid index name</response>
    [HttpPost("{indexName}/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateIndex(
        string indexName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            return BadRequest("Index name is required");
        }

        _logger.LogInformation("Creating index: {IndexName}", indexName);
        
        await _indexManagementService.CreateIndexAsync(indexName, cancellationToken);
        
        return Ok(new { message = $"Index '{indexName}' created successfully" });
    }

    /// <summary>
    /// Delete a specific index
    /// </summary>
    /// <param name="indexName">Name of the index to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Index deleted successfully</response>
    /// <response code="400">Invalid index name</response>
    [HttpDelete("{indexName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteIndex(
        string indexName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(indexName))
        {
            return BadRequest("Index name is required");
        }

        _logger.LogWarning("Deleting index: {IndexName}", indexName);
        
        await _indexManagementService.DeleteIndexAsync(indexName, cancellationToken);
        
        return Ok(new { message = $"Index '{indexName}' deleted successfully" });
    }

    /// <summary>
    /// Check if an index exists
    /// </summary>
    /// <param name="indexName">Name of the index to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Existence status</returns>
    /// <response code="200">Returns existence status</response>
    [HttpGet("{indexName}/exists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> IndexExists(
        string indexName,
        CancellationToken cancellationToken)
    {
        var exists = await _indexManagementService.IndexExistsAsync(indexName, cancellationToken);
        return Ok(new { indexName, exists });
    }

    /// <summary>
    /// Get statistics for a specific index
    /// </summary>
    /// <param name="indexName">Name of the index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Index statistics</returns>
    /// <response code="200">Returns index statistics</response>
    /// <response code="404">Index not found</response>
    [HttpGet("{indexName}/stats")]
    [ProducesResponseType(typeof(IndexStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndexStatsResponse>> GetIndexStats(
        string indexName,
        CancellationToken cancellationToken)
    {
        var stats = await _indexManagementService.GetIndexStatsAsync(indexName, cancellationToken);
        return Ok(stats);
    }

    /// <summary>
    /// Refresh an index to make recent changes searchable
    /// </summary>
    /// <param name="indexName">Name of the index to refresh</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Index refreshed successfully</response>
    [HttpPost("{indexName}/refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshIndex(
        string indexName,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refreshing index: {IndexName}", indexName);
        
        await _indexManagementService.RefreshIndexAsync(indexName, cancellationToken);
        
        return Ok(new { message = $"Index '{indexName}' refreshed successfully" });
    }

    /// <summary>
    /// Reindex all documents for enterprises
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Reindex triggered successfully</response>
    [HttpPost("enterprises/reindex")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ReindexEnterprises(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reindexing enterprises");
        
        await _indexingService.ReindexAllAsync("enterprises_idx", cancellationToken);
        
        return Ok(new { message = "Enterprise reindex operation triggered. This may take some time." });
    }

    /// <summary>
    /// Reindex all documents
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Reindex triggered successfully</response>
    [HttpPost("documents/reindex")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ReindexDocuments(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reindexing documents");
        
        await _indexingService.ReindexAllAsync("documents_idx", cancellationToken);
        
        return Ok(new { message = "Document reindex operation triggered. This may take some time." });
    }

    /// <summary>
    /// Reindex all projects
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    /// <response code="200">Reindex triggered successfully</response>
    [HttpPost("projects/reindex")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ReindexProjects(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reindexing projects");
        
        await _indexingService.ReindexAllAsync("projects_idx", cancellationToken);
        
        return Ok(new { message = "Project reindex operation triggered. This may take some time." });
    }

    /// <summary>
    /// Bulk index sample enterprises for testing
    /// </summary>
    /// <param name="request">List of enterprises to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of indexed documents</returns>
    /// <response code="200">Documents indexed successfully</response>
    [HttpPost("enterprises/bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> BulkIndexEnterprises(
        [FromBody] List<EnterpriseSearchDocument> request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bulk indexing {Count} enterprises", request.Count);
        
        var count = await _indexingService.BulkIndexEnterprisesAsync(request, cancellationToken);
        
        return Ok(new { message = $"{count} enterprises indexed successfully", count });
    }

    /// <summary>
    /// Bulk index sample documents for testing
    /// </summary>
    /// <param name="request">List of documents to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of indexed documents</returns>
    /// <response code="200">Documents indexed successfully</response>
    [HttpPost("documents/bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> BulkIndexDocuments(
        [FromBody] List<DocumentSearchDocument> request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bulk indexing {Count} documents", request.Count);
        
        var count = await _indexingService.BulkIndexDocumentsAsync(request, cancellationToken);
        
        return Ok(new { message = $"{count} documents indexed successfully", count });
    }

    /// <summary>
    /// Bulk index sample projects for testing
    /// </summary>
    /// <param name="request">List of projects to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of indexed documents</returns>
    /// <response code="200">Projects indexed successfully</response>
    [HttpPost("projects/bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> BulkIndexProjects(
        [FromBody] List<ProjectSearchDocument> request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bulk indexing {Count} projects", request.Count);
        
        var count = await _indexingService.BulkIndexProjectsAsync(request, cancellationToken);
        
        return Ok(new { message = $"{count} projects indexed successfully", count });
    }
}
