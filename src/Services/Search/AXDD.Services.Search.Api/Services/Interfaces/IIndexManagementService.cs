using AXDD.Services.Search.Api.DTOs;

namespace AXDD.Services.Search.Api.Services.Interfaces;

/// <summary>
/// Service for managing Elasticsearch indexes
/// </summary>
public interface IIndexManagementService
{
    /// <summary>
    /// Create an index with proper mappings and settings
    /// </summary>
    /// <param name="indexName">Name of the index to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateIndexAsync(string indexName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an index
    /// </summary>
    /// <param name="indexName">Name of the index to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if an index exists
    /// </summary>
    /// <param name="indexName">Name of the index to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if index exists, false otherwise</returns>
    Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get statistics for an index
    /// </summary>
    /// <param name="indexName">Name of the index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Index statistics</returns>
    Task<IndexStatsResponse> GetIndexStatsAsync(string indexName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initialize all required indexes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InitializeIndexesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh an index to make recent changes searchable
    /// </summary>
    /// <param name="indexName">Name of the index to refresh</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshIndexAsync(string indexName, CancellationToken cancellationToken = default);
}
