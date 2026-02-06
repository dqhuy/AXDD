using AXDD.Services.Search.Api.DTOs;
using AXDD.Services.Search.Api.Models;

namespace AXDD.Services.Search.Api.Services.Interfaces;

/// <summary>
/// Service for performing search operations
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Search enterprises with filters and pagination
    /// </summary>
    /// <param name="request">Search request with query and filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response with results</returns>
    Task<SearchResponse<EnterpriseSearchDocument>> SearchEnterprisesAsync(
        EnterpriseSearchRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search documents with filters and pagination
    /// </summary>
    /// <param name="request">Search request with query and filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response with results</returns>
    Task<SearchResponse<DocumentSearchDocument>> SearchDocumentsAsync(
        DocumentSearchRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search projects with filters and pagination
    /// </summary>
    /// <param name="request">Search request with query and filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search response with results</returns>
    Task<SearchResponse<ProjectSearchDocument>> SearchProjectsAsync(
        ProjectSearchRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search across all indexes
    /// </summary>
    /// <param name="query">Search query text</param>
    /// <param name="pageSize">Number of results per index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Multi-index search response</returns>
    Task<MultiSearchResponse> SearchAllAsync(
        string query, 
        int pageSize = 5, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get search suggestions for autocomplete
    /// </summary>
    /// <param name="query">Partial query text</param>
    /// <param name="type">Type of suggestions (enterprise, document, project)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of suggestions</returns>
    Task<SuggestionResponse> GetSuggestionsAsync(
        string query, 
        string type, 
        CancellationToken cancellationToken = default);
}
