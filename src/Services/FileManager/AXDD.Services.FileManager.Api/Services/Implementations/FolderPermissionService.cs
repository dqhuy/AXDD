using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for folder permission operations
/// </summary>
public class FolderPermissionService : IFolderPermissionService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<FolderPermissionService> _logger;

    public FolderPermissionService(FileManagerDbContext context, ILogger<FolderPermissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<FolderPermissionDto>> GrantPermissionAsync(Guid folderId, GrantFolderPermissionRequest request, string grantedBy, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(grantedBy);

        if (string.IsNullOrWhiteSpace(request.UserId) && string.IsNullOrWhiteSpace(request.UserGroupId))
        {
            return Result<FolderPermissionDto>.Failure("Either UserId or UserGroupId must be specified");
        }

        try
        {
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

            if (folder == null)
            {
                return Result<FolderPermissionDto>.Failure($"Folder with ID '{folderId}' not found");
            }

            // Check if permission already exists
            var existingPermission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => 
                    p.FolderId == folderId && 
                    !p.IsDeleted &&
                    ((request.UserId != null && p.UserId == request.UserId) ||
                     (request.UserGroupId != null && p.UserGroupId == request.UserGroupId)), 
                    cancellationToken);

            if (existingPermission != null)
            {
                // Update existing permission
                existingPermission.Permission = request.Permission;
                existingPermission.CanShare = request.CanShare;
                existingPermission.CanDownload = request.CanDownload;
                existingPermission.CanPrint = request.CanPrint;
                existingPermission.ExpiresAt = request.ExpiresAt;
                existingPermission.UpdatedAt = DateTime.UtcNow;
                existingPermission.UpdatedBy = grantedBy;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Folder permission updated: FolderId={FolderId}, UserId={UserId}", folderId, request.UserId ?? request.UserGroupId);

                return Result<FolderPermissionDto>.Success(MapToDto(existingPermission));
            }

            // Create new permission
            var permission = new FolderPermission
            {
                FolderId = folderId,
                UserId = request.UserId,
                UserGroupId = request.UserGroupId,
                Permission = request.Permission,
                CanShare = request.CanShare,
                CanDownload = request.CanDownload,
                CanPrint = request.CanPrint,
                ExpiresAt = request.ExpiresAt,
                GrantedBy = grantedBy,
                GrantedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = grantedBy
            };

            _context.FolderPermissions.Add(permission);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder permission granted: FolderId={FolderId}, UserId={UserId}", folderId, request.UserId ?? request.UserGroupId);

            return Result<FolderPermissionDto>.Success(MapToDto(permission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting folder permission: FolderId={FolderId}", folderId);
            return Result<FolderPermissionDto>.Failure($"Failed to grant permission: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderPermissionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (permission == null)
            {
                return Result<FolderPermissionDto>.Failure($"Permission with ID '{id}' not found");
            }

            return Result<FolderPermissionDto>.Success(MapToDto(permission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission: {Id}", id);
            return Result<FolderPermissionDto>.Failure($"Failed to get permission: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<FolderPermissionDto>>> ListByFolderAsync(Guid folderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await _context.FolderPermissions
                .Where(p => p.FolderId == folderId && !p.IsDeleted)
                .OrderBy(p => p.GrantedAt)
                .ToListAsync(cancellationToken);

            return Result<List<FolderPermissionDto>>.Success(permissions.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing permissions for folder: {FolderId}", folderId);
            return Result<List<FolderPermissionDto>>.Failure($"Failed to list permissions: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<FolderPermissionDto>>> ListByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var permissions = await _context.FolderPermissions
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderBy(p => p.GrantedAt)
                .ToListAsync(cancellationToken);

            return Result<List<FolderPermissionDto>>.Success(permissions.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing permissions for user: {UserId}", userId);
            return Result<List<FolderPermissionDto>>.Failure($"Failed to list permissions: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderPermissionDto>> UpdatePermissionAsync(Guid folderId, Guid permissionId, GrantFolderPermissionRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.Id == permissionId && p.FolderId == folderId && !p.IsDeleted, cancellationToken);

            if (permission == null)
            {
                return Result<FolderPermissionDto>.Failure($"Permission with ID '{permissionId}' not found");
            }

            permission.Permission = request.Permission;
            permission.CanShare = request.CanShare;
            permission.CanDownload = request.CanDownload;
            permission.CanPrint = request.CanPrint;
            permission.ExpiresAt = request.ExpiresAt;
            permission.UpdatedAt = DateTime.UtcNow;
            permission.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder permission updated: PermissionId={PermissionId}", permissionId);

            return Result<FolderPermissionDto>.Success(MapToDto(permission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission: {PermissionId}", permissionId);
            return Result<FolderPermissionDto>.Failure($"Failed to update permission: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> RevokePermissionAsync(Guid folderId, Guid permissionId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var permission = await _context.FolderPermissions
                .FirstOrDefaultAsync(p => p.Id == permissionId && p.FolderId == folderId && !p.IsDeleted, cancellationToken);

            if (permission == null)
            {
                return Result.Failure($"Permission with ID '{permissionId}' not found");
            }

            permission.IsDeleted = true;
            permission.DeletedAt = DateTime.UtcNow;
            permission.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder permission revoked: PermissionId={PermissionId}", permissionId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission: {PermissionId}", permissionId);
            return Result.Failure($"Failed to revoke permission: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PermissionLevel>> CheckPermissionAsync(Guid folderId, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var permission = await _context.FolderPermissions
                .Where(p => p.FolderId == folderId && p.UserId == userId && !p.IsDeleted)
                .Where(p => p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(p => p.Permission) // Get highest permission level
                .FirstOrDefaultAsync(cancellationToken);

            if (permission == null)
            {
                return Result<PermissionLevel>.Success(PermissionLevel.None);
            }

            return Result<PermissionLevel>.Success(permission.Permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission: FolderId={FolderId}, UserId={UserId}", folderId, userId);
            return Result<PermissionLevel>.Failure($"Failed to check permission: {ex.Message}");
        }
    }

    private static FolderPermissionDto MapToDto(FolderPermission permission) => new()
    {
        Id = permission.Id,
        FolderId = permission.FolderId,
        UserId = permission.UserId,
        UserGroupId = permission.UserGroupId,
        Permission = permission.Permission,
        CanShare = permission.CanShare,
        CanDownload = permission.CanDownload,
        CanPrint = permission.CanPrint,
        ExpiresAt = permission.ExpiresAt,
        GrantedBy = permission.GrantedBy,
        GrantedAt = permission.GrantedAt
    };
}
