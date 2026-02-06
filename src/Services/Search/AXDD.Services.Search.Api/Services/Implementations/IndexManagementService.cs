using AXDD.Services.Search.Api.DTOs;
using AXDD.Services.Search.Api.Exceptions;
using AXDD.Services.Search.Api.Services.Interfaces;
using AXDD.Services.Search.Api.Settings;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.Extensions.Options;

namespace AXDD.Services.Search.Api.Services.Implementations;

/// <summary>
/// Service for managing Elasticsearch indexes
/// </summary>
public class IndexManagementService : IIndexManagementService
{
    private readonly IElasticsearchClientFactory _clientFactory;
    private readonly ElasticsearchSettings _elasticsearchSettings;
    private readonly ILogger<IndexManagementService> _logger;

    public IndexManagementService(
        IElasticsearchClientFactory clientFactory,
        IOptions<ElasticsearchSettings> elasticsearchSettings,
        ILogger<IndexManagementService> logger)
    {
        _clientFactory = clientFactory;
        _elasticsearchSettings = elasticsearchSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task CreateIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        try
        {
            var client = _clientFactory.GetClient();

            // Check if index already exists
            var existsResponse = await client.Indices.ExistsAsync(indexName, cancellationToken);
            if (existsResponse.Exists)
            {
                _logger.LogInformation("Index {IndexName} already exists", indexName);
                return;
            }

            // Create index with mappings based on index name
            var createResponse = indexName switch
            {
                var name when name.Contains("enterprises") => await CreateEnterpriseIndexAsync(client, indexName, cancellationToken),
                var name when name.Contains("documents") => await CreateDocumentIndexAsync(client, indexName, cancellationToken),
                var name when name.Contains("projects") => await CreateProjectIndexAsync(client, indexName, cancellationToken),
                _ => throw new ArgumentException($"Unknown index type: {indexName}")
            };

            if (!createResponse.IsValidResponse)
            {
                throw new IndexingException($"Failed to create index {indexName}: {createResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            _logger.LogInformation("Index {IndexName} created successfully", indexName);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error creating index {IndexName}", indexName);
            throw new IndexingException($"Failed to create index {indexName}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.Indices.DeleteAsync(indexName, cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException($"Failed to delete index {indexName}: {response.ElasticsearchServerError?.Error?.Reason}");
            }

            _logger.LogInformation("Index {IndexName} deleted successfully", indexName);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error deleting index {IndexName}", indexName);
            throw new IndexingException($"Failed to delete index {indexName}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.Indices.ExistsAsync(indexName, cancellationToken);
            return response.Exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if index {IndexName} exists", indexName);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IndexStatsResponse> GetIndexStatsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        try
        {
            var client = _clientFactory.GetClient();
            
            // Simplified stats retrieval
            // TODO: Implement proper stats retrieval with Elasticsearch 8.x API
            var healthResponse = await client.Cluster.HealthAsync(cancellationToken);

            return new IndexStatsResponse
            {
                IndexName = indexName,
                DocumentCount = 0, // TODO: Get actual count
                SizeBytes = 0, // TODO: Get actual size
                Health = healthResponse.Status.ToString(),
                Shards = 0, // TODO: Get actual shards
                Replicas = 0 // TODO: Get actual replicas
            };
        }
        catch (Exception ex) when (ex is not IndexNotFoundException)
        {
            _logger.LogError(ex, "Error getting stats for index {IndexName}", indexName);
            throw new IndexingException($"Failed to get stats for index {indexName}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task InitializeIndexesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing all indexes");

        await CreateIndexAsync(_elasticsearchSettings.Indexes.Enterprises, cancellationToken);
        await CreateIndexAsync(_elasticsearchSettings.Indexes.Documents, cancellationToken);
        await CreateIndexAsync(_elasticsearchSettings.Indexes.Projects, cancellationToken);

        _logger.LogInformation("All indexes initialized successfully");
    }

    /// <inheritdoc/>
    public async Task RefreshIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.Indices.RefreshAsync(indexName, cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException($"Failed to refresh index {indexName}: {response.ElasticsearchServerError?.Error?.Reason}");
            }

            _logger.LogDebug("Index {IndexName} refreshed successfully", indexName);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error refreshing index {IndexName}", indexName);
            throw new IndexingException($"Failed to refresh index {indexName}", ex);
        }
    }

    private async Task<CreateIndexResponse> CreateEnterpriseIndexAsync(
        ElasticsearchClient client, 
        string indexName, 
        CancellationToken cancellationToken)
    {
        // Simplified index creation - Elasticsearch will use dynamic mapping
        // TODO: Add proper typed mappings when Elasticsearch 8.x syntax is finalized
        return await client.Indices.CreateAsync(indexName, cancellationToken);
    }

    private async Task<CreateIndexResponse> CreateDocumentIndexAsync(
        ElasticsearchClient client, 
        string indexName, 
        CancellationToken cancellationToken)
    {
        // Simplified index creation - Elasticsearch will use dynamic mapping
        // TODO: Add proper typed mappings when Elasticsearch 8.x syntax is finalized
        return await client.Indices.CreateAsync(indexName, cancellationToken);
    }

    private async Task<CreateIndexResponse> CreateProjectIndexAsync(
        ElasticsearchClient client, 
        string indexName, 
        CancellationToken cancellationToken)
    {
        // Simplified index creation - Elasticsearch will use dynamic mapping
        // TODO: Add proper typed mappings when Elasticsearch 8.x syntax is finalized
        return await client.Indices.CreateAsync(indexName, cancellationToken);
    }
}
