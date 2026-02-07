using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for folder permission operations
/// </summary>
public interface IFolderPermissionService
{
    /// <summary>
    /// Grants permission to a folder
    /// </summary>
    Task<Result<FolderPermissionDto>> GrantPermissionAsync(Guid folderId, GrantFolderPermissionRequest request, string grantedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a permission by ID
    /// </summary>
    Task<Result<FolderPermissionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists permissions for a folder
    /// </summary>
    Task<Result<List<FolderPermissionDto>>> ListByFolderAsync(Guid folderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists permissions for a user
    /// </summary>
    Task<Result<List<FolderPermissionDto>>> ListByUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a permission
    /// </summary>
    Task<Result<FolderPermissionDto>> UpdatePermissionAsync(Guid folderId, Guid permissionId, GrantFolderPermissionRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a permission
    /// </summary>
    Task<Result> RevokePermissionAsync(Guid folderId, Guid permissionId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has permission to access a folder
    /// </summary>
    Task<Result<PermissionLevel>> CheckPermissionAsync(Guid folderId, string userId, CancellationToken cancellationToken = default);
}
