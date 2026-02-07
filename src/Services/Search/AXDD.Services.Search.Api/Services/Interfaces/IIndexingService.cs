using AXDD.Services.Search.Api.DTOs;
using AXDD.Services.Search.Api.Models;

namespace AXDD.Services.Search.Api.Services.Interfaces;

/// <summary>
/// Service for indexing documents in Elasticsearch
/// </summary>
public interface IIndexingService
{
    /// <summary>
    /// Index a single enterprise document
    /// </summary>
    /// <param name="enterprise">Enterprise document to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task IndexEnterpriseAsync(
        EnterpriseSearchDocument enterprise, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Index a single document
    /// </summary>
    /// <param name="document">Document to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task IndexDocumentAsync(
        DocumentSearchDocument document, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Index a single project document
    /// </summary>
    /// <param name="project">Project document to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task IndexProjectAsync(
        ProjectSearchDocument project, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk index multiple enterprises
    /// </summary>
    /// <param name="enterprises">List of enterprises to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of successfully indexed documents</returns>
    Task<int> BulkIndexEnterprisesAsync(
        IEnumerable<EnterpriseSearchDocument> enterprises, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk index multiple documents
    /// </summary>
    /// <param name="documents">List of documents to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of successfully indexed documents</returns>
    Task<int> BulkIndexDocumentsAsync(
        IEnumerable<DocumentSearchDocument> documents, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk index multiple projects
    /// </summary>
    /// <param name="projects">List of projects to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of successfully indexed documents</returns>
    Task<int> BulkIndexProjectsAsync(
        IEnumerable<ProjectSearchDocument> projects, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a document from an index
    /// </summary>
    /// <param name="id">Document ID</param>
    /// <param name="indexName">Index name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFromIndexAsync(
        string id, 
        string indexName, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reindex all documents for a specific index
    /// </summary>
    /// <param name="indexName">Index name to reindex</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ReindexAllAsync(
        string indexName, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing enterprise document
    /// </summary>
    /// <param name="enterprise">Enterprise document to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateEnterpriseAsync(
        EnterpriseSearchDocument enterprise, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing document
    /// </summary>
    /// <param name="document">Document to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateDocumentAsync(
        DocumentSearchDocument document, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing project document
    /// </summary>
    /// <param name="project">Project document to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateProjectAsync(
        ProjectSearchDocument project, 
        CancellationToken cancellationToken = default);
}
