using AXDD.Services.Search.Api.Exceptions;
using AXDD.Services.Search.Api.Models;
using AXDD.Services.Search.Api.Services.Interfaces;
using AXDD.Services.Search.Api.Settings;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace AXDD.Services.Search.Api.Services.Implementations;

/// <summary>
/// Service for indexing documents in Elasticsearch
/// </summary>
public class IndexingService : IIndexingService
{
    private readonly IElasticsearchClientFactory _clientFactory;
    private readonly ElasticsearchSettings _settings;
    private readonly ILogger<IndexingService> _logger;

    public IndexingService(
        IElasticsearchClientFactory clientFactory,
        IOptions<ElasticsearchSettings> settings,
        ILogger<IndexingService> logger)
    {
        _clientFactory = clientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task IndexEnterpriseAsync(EnterpriseSearchDocument enterprise, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enterprise);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.IndexAsync(
                enterprise, 
                _settings.Indexes.Enterprises, 
                enterprise.Id.ToString(),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException(
                    $"Failed to index enterprise {enterprise.Id}: {response.ElasticsearchServerError?.Error?.Reason}", 
                    enterprise.Id.ToString());
            }

            _logger.LogDebug("Enterprise {Id} indexed successfully", enterprise.Id);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error indexing enterprise {Id}", enterprise.Id);
            throw new IndexingException($"Failed to index enterprise {enterprise.Id}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task IndexDocumentAsync(DocumentSearchDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.IndexAsync(
                document, 
                _settings.Indexes.Documents, 
                document.Id.ToString(),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException(
                    $"Failed to index document {document.Id}: {response.ElasticsearchServerError?.Error?.Reason}", 
                    document.Id.ToString());
            }

            _logger.LogDebug("Document {Id} indexed successfully", document.Id);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error indexing document {Id}", document.Id);
            throw new IndexingException($"Failed to index document {document.Id}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task IndexProjectAsync(ProjectSearchDocument project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.IndexAsync(
                project, 
                _settings.Indexes.Projects, 
                project.Id.ToString(),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException(
                    $"Failed to index project {project.Id}: {response.ElasticsearchServerError?.Error?.Reason}", 
                    project.Id.ToString());
            }

            _logger.LogDebug("Project {Id} indexed successfully", project.Id);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error indexing project {Id}", project.Id);
            throw new IndexingException($"Failed to index project {project.Id}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<int> BulkIndexEnterprisesAsync(IEnumerable<EnterpriseSearchDocument> enterprises, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enterprises);

        var enterpriseList = enterprises.ToList();
        if (enterpriseList.Count == 0)
        {
            return 0;
        }

        try
        {
            var client = _clientFactory.GetClient();
            
            // Simplified: Index documents one by one
            // TODO: Use proper bulk API when Elasticsearch 8.x syntax is finalized
            var successCount = 0;
            foreach (var enterprise in enterpriseList)
            {
                try
                {
                    await IndexEnterpriseAsync(enterprise, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to index enterprise {Id}", enterprise.Id);
                }
            }

            _logger.LogInformation("Bulk indexed {SuccessCount}/{TotalCount} enterprises", 
                successCount, 
                enterpriseList.Count);

            return successCount;
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error during bulk index of enterprises");
            throw new IndexingException("Failed to bulk index enterprises", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<int> BulkIndexDocumentsAsync(IEnumerable<DocumentSearchDocument> documents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(documents);

        var documentList = documents.ToList();
        if (documentList.Count == 0)
        {
            return 0;
        }

        try
        {
            // Simplified: Index documents one by one
            // TODO: Use proper bulk API when Elasticsearch 8.x syntax is finalized
            var successCount = 0;
            foreach (var document in documentList)
            {
                try
                {
                    await IndexDocumentAsync(document, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to index document {Id}", document.Id);
                }
            }

            _logger.LogInformation("Bulk indexed {SuccessCount}/{TotalCount} documents", 
                successCount, 
                documentList.Count);

            return successCount;
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error during bulk index of documents");
            throw new IndexingException("Failed to bulk index documents", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<int> BulkIndexProjectsAsync(IEnumerable<ProjectSearchDocument> projects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projects);

        var projectList = projects.ToList();
        if (projectList.Count == 0)
        {
            return 0;
        }

        try
        {
            // Simplified: Index documents one by one
            // TODO: Use proper bulk API when Elasticsearch 8.x syntax is finalized
            var successCount = 0;
            foreach (var project in projectList)
            {
                try
                {
                    await IndexProjectAsync(project, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to index project {Id}", project.Id);
                }
            }

            _logger.LogInformation("Bulk indexed {SuccessCount}/{TotalCount} projects", 
                successCount, 
                projectList.Count);

            return successCount;
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error during bulk index of projects");
            throw new IndexingException("Failed to bulk index projects", ex);
        }
    }

    /// <inheritdoc/>
    public async Task DeleteFromIndexAsync(string id, string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.DeleteAsync(new DeleteRequest(indexName, id), cancellationToken);

            if (!response.IsValidResponse && response.Result != Elastic.Clients.Elasticsearch.Result.NotFound)
            {
                throw new IndexingException(
                    $"Failed to delete document {id} from index {indexName}: {response.ElasticsearchServerError?.Error?.Reason}",
                    id);
            }

            _logger.LogDebug("Document {Id} deleted from index {IndexName}", id, indexName);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error deleting document {Id} from index {IndexName}", id, indexName);
            throw new IndexingException($"Failed to delete document {id} from index {indexName}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task ReindexAllAsync(string indexName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(indexName);

        _logger.LogWarning("Reindex operation requested for {IndexName}. This should trigger data reload from source systems.", indexName);
        
        // Note: This is a placeholder. In a real implementation, this would:
        // 1. Connect to the source database (Enterprise, Document, or Project service)
        // 2. Fetch all records
        // 3. Bulk index them
        // For now, we just log a warning that this needs to be implemented
        
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task UpdateEnterpriseAsync(EnterpriseSearchDocument enterprise, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enterprise);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.UpdateAsync<EnterpriseSearchDocument, EnterpriseSearchDocument>(
                _settings.Indexes.Enterprises,
                enterprise.Id.ToString(),
                u => u.Doc(enterprise).DocAsUpsert(true),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException(
                    $"Failed to update enterprise {enterprise.Id}: {response.ElasticsearchServerError?.Error?.Reason}",
                    enterprise.Id.ToString());
            }

            _logger.LogDebug("Enterprise {Id} updated successfully", enterprise.Id);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error updating enterprise {Id}", enterprise.Id);
            throw new IndexingException($"Failed to update enterprise {enterprise.Id}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateDocumentAsync(DocumentSearchDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.UpdateAsync<DocumentSearchDocument, DocumentSearchDocument>(
                _settings.Indexes.Documents,
                document.Id.ToString(),
                u => u.Doc(document).DocAsUpsert(true),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException(
                    $"Failed to update document {document.Id}: {response.ElasticsearchServerError?.Error?.Reason}",
                    document.Id.ToString());
            }

            _logger.LogDebug("Document {Id} updated successfully", document.Id);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error updating document {Id}", document.Id);
            throw new IndexingException($"Failed to update document {document.Id}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateProjectAsync(ProjectSearchDocument project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        try
        {
            var client = _clientFactory.GetClient();
            var response = await client.UpdateAsync<ProjectSearchDocument, ProjectSearchDocument>(
                _settings.Indexes.Projects,
                project.Id.ToString(),
                u => u.Doc(project).DocAsUpsert(true),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                throw new IndexingException(
                    $"Failed to update project {project.Id}: {response.ElasticsearchServerError?.Error?.Reason}",
                    project.Id.ToString());
            }

            _logger.LogDebug("Project {Id} updated successfully", project.Id);
        }
        catch (Exception ex) when (ex is not IndexingException)
        {
            _logger.LogError(ex, "Error updating project {Id}", project.Id);
            throw new IndexingException($"Failed to update project {project.Id}", ex);
        }
    }
}
