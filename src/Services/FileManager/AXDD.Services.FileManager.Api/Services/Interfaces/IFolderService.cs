using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for folder operations
/// </summary>
public interface IFolderService
{
    /// <summary>
    /// Creates a new folder
    /// </summary>
    /// <param name="name">The folder name</param>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="parentFolderId">Optional parent folder ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="description">Optional description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the created folder</returns>
    Task<Result<FolderDto>> CreateFolderAsync(
        string name,
        string enterpriseCode,
        Guid? parentFolderId,
        string userId,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a folder by ID
    /// </summary>
    /// <param name="folderId">The folder ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the folder</returns>
    Task<Result<FolderDto>> GetFolderAsync(Guid folderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the root folder for an enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the root folder</returns>
    Task<Result<FolderDto>> GetRootFolderAsync(string enterpriseCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists folders with pagination
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="parentFolderId">Optional parent folder ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing paginated folder list</returns>
    Task<Result<PagedResult<FolderDto>>> ListFoldersAsync(
        string enterpriseCode,
        Guid? parentFolderId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a folder (soft delete)
    /// </summary>
    /// <param name="folderId">The folder ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> DeleteFolderAsync(Guid folderId, string userId, CancellationToken cancellationToken = default);
}
