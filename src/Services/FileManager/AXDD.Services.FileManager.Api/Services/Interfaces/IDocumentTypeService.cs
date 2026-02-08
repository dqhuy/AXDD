using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for document type operations
/// </summary>
public interface IDocumentTypeService
{
    /// <summary>
    /// Creates a new document type
    /// </summary>
    Task<Result<DocumentTypeDto>> CreateAsync(CreateDocumentTypeRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document type by ID
    /// </summary>
    Task<Result<DocumentTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document type by code
    /// </summary>
    Task<Result<DocumentTypeDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all document types
    /// </summary>
    Task<Result<PagedResult<DocumentTypeDto>>> ListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a document type
    /// </summary>
    Task<Result<DocumentTypeDto>> UpdateAsync(Guid id, CreateDocumentTypeRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document type
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a metadata field to a document type
    /// </summary>
    Task<Result<DocumentTypeMetadataFieldDto>> AddMetadataFieldAsync(Guid documentTypeId, CreateDocumentTypeMetadataFieldRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a metadata field from a document type
    /// </summary>
    Task<Result> RemoveMetadataFieldAsync(Guid documentTypeId, Guid fieldId, string userId, CancellationToken cancellationToken = default);
}
