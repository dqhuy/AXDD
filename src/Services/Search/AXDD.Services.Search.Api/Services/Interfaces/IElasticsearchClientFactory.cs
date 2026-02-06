using Elastic.Clients.Elasticsearch;

namespace AXDD.Services.Search.Api.Services.Interfaces;

/// <summary>
/// Factory for creating Elasticsearch client instances
/// </summary>
public interface IElasticsearchClientFactory
{
    /// <summary>
    /// Get or create the Elasticsearch client
    /// </summary>
    /// <returns>Configured Elasticsearch client</returns>
    ElasticsearchClient GetClient();

    /// <summary>
    /// Check if the Elasticsearch cluster is healthy
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if cluster is healthy, false otherwise</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}
