using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for document profile document operations
/// </summary>
public interface IDocumentProfileDocumentService
{
    /// <summary>
    /// Adds a document to a profile
    /// </summary>
    /// <param name="request">The add document request</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the added document</returns>
    Task<Result<DocumentProfileDocumentDto>> AddDocumentAsync(
        AddDocumentToProfileRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document by ID
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="includeMetadata">Whether to include metadata values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the document</returns>
    Task<Result<DocumentProfileDocumentDto>> GetDocumentAsync(
        Guid documentId,
        bool includeMetadata = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="request">The update request</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the updated document</returns>
    Task<Result<DocumentProfileDocumentDto>> UpdateDocumentAsync(
        Guid documentId,
        UpdateDocumentProfileDocumentRequest request,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a document from a profile (soft delete)
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> RemoveDocumentAsync(
        Guid documentId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists documents in a profile with pagination
    /// </summary>
    /// <param name="query">The list query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing paginated document list</returns>
    Task<Result<PagedResult<DocumentProfileDocumentDto>>> ListDocumentsAsync(
        DocumentProfileDocumentListQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets metadata values for a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="values">The metadata values to set</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the updated metadata values</returns>
    Task<Result<List<DocumentMetadataValueDto>>> SetMetadataValuesAsync(
        Guid documentId,
        List<SetMetadataValueRequest> values,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metadata values for a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the metadata values</returns>
    Task<Result<List<DocumentMetadataValueDto>>> GetMetadataValuesAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a document to a different profile
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="targetProfileId">The target profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> MoveDocumentAsync(
        Guid documentId,
        Guid targetProfileId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a document to another profile
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="targetProfileId">The target profile ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the copied document</returns>
    Task<Result<DocumentProfileDocumentDto>> CopyDocumentAsync(
        Guid documentId,
        Guid targetProfileId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reorders documents within a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="documentOrders">Dictionary of document ID to display order</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> ReorderDocumentsAsync(
        Guid profileId,
        Dictionary<Guid, int> documentOrders,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets documents expiring within a specified period
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="daysAhead">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing expiring documents</returns>
    Task<Result<List<DocumentProfileDocumentDto>>> GetExpiringDocumentsAsync(
        string enterpriseCode,
        int daysAhead = 30,
        CancellationToken cancellationToken = default);
}
