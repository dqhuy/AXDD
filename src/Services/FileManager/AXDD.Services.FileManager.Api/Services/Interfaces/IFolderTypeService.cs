using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for folder type operations
/// </summary>
public interface IFolderTypeService
{
    /// <summary>
    /// Creates a new folder type
    /// </summary>
    Task<Result<FolderTypeDto>> CreateAsync(CreateFolderTypeRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a folder type by ID
    /// </summary>
    Task<Result<FolderTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a folder type by code
    /// </summary>
    Task<Result<FolderTypeDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all folder types
    /// </summary>
    Task<Result<PagedResult<FolderTypeDto>>> ListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a folder type
    /// </summary>
    Task<Result<FolderTypeDto>> UpdateAsync(Guid id, CreateFolderTypeRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a folder type
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a metadata field to a folder type
    /// </summary>
    Task<Result<FolderTypeMetadataFieldDto>> AddMetadataFieldAsync(Guid folderTypeId, CreateFolderTypeMetadataFieldRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a metadata field from a folder type
    /// </summary>
    Task<Result> RemoveMetadataFieldAsync(Guid folderTypeId, Guid fieldId, string userId, CancellationToken cancellationToken = default);
}
