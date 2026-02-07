using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for file sharing operations
/// </summary>
public class FileShareService : IFileShareService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<FileShareService> _logger;

    /// <summary>
    /// Initializes a new instance of FileShareService
    /// </summary>
    public FileShareService(
        FileManagerDbContext context,
        ILogger<FileShareService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<FileShareDto>> ShareFileAsync(
        Guid fileId,
        string sharedWithUserId,
        string permission,
        string sharedBy,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sharedWithUserId, nameof(sharedWithUserId));
        ArgumentException.ThrowIfNullOrWhiteSpace(permission, nameof(permission));
        ArgumentException.ThrowIfNullOrWhiteSpace(sharedBy, nameof(sharedBy));

        try
        {
            var fileMetadata = await _context.FileMetadata
                .Where(f => f.Id == fileId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileMetadata == null)
            {
                return Result<FileShareDto>.Failure($"File with ID '{fileId}' not found");
            }

            // Check if already shared with this user
            var existingShare = await _context.FileShares
                .Where(s => s.FileMetadataId == fileId
                    && s.SharedWithUserId == sharedWithUserId
                    && s.IsActive
                    && !s.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingShare != null)
            {
                // Update existing share
                existingShare.Permission = permission;
                existingShare.ExpiresAt = expiresAt;
                existingShare.UpdatedAt = DateTime.UtcNow;

                _context.FileShares.Update(existingShare);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "File share updated: FileId={FileId}, SharedWith={SharedWithUserId}",
                    fileId, sharedWithUserId);

                var existingDto = MapToDto(existingShare, fileMetadata.FileName);
                return Result<FileShareDto>.Success(existingDto);
            }

            // Create new share
            var fileShare = new Entities.FileShare
            {
                FileMetadataId = fileId,
                SharedWithUserId = sharedWithUserId,
                Permission = permission,
                ExpiresAt = expiresAt,
                IsActive = true,
                SharedBy = sharedBy,
                SharedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = sharedBy
            };

            _context.FileShares.Add(fileShare);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "File shared: FileId={FileId}, SharedWith={SharedWithUserId}, Permission={Permission}",
                fileId, sharedWithUserId, permission);

            var dto = MapToDto(fileShare, fileMetadata.FileName);
            return Result<FileShareDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing file: FileId={FileId}", fileId);
            return Result<FileShareDto>.Failure($"Failed to share file: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<FileShareDto>>> GetSharedFilesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var shares = await _context.FileShares
                .Where(s => s.SharedWithUserId == userId
                    && s.IsActive
                    && !s.IsDeleted
                    && (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow))
                .Include(s => s.FileMetadata)
                .ToListAsync(cancellationToken);

            var dtos = shares
                .Where(s => s.FileMetadata != null && !s.FileMetadata.IsDeleted)
                .Select(s => MapToDto(s, s.FileMetadata!.FileName))
                .ToList();

            return Result<List<FileShareDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shared files for user: UserId={UserId}", userId);
            return Result<List<FileShareDto>>.Failure($"Failed to get shared files: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> RevokeShareAsync(Guid shareId, CancellationToken cancellationToken = default)
    {
        try
        {
            var share = await _context.FileShares
                .Where(s => s.Id == shareId && !s.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (share == null)
            {
                return Result.Failure($"File share with ID '{shareId}' not found");
            }

            share.IsActive = false;
            share.UpdatedAt = DateTime.UtcNow;

            _context.FileShares.Update(share);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("File share revoked: ShareId={ShareId}", shareId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking file share: ShareId={ShareId}", shareId);
            return Result.Failure($"Failed to revoke file share: {ex.Message}");
        }
    }

    private static FileShareDto MapToDto(Entities.FileShare share, string fileName)
    {
        return new FileShareDto
        {
            Id = share.Id,
            FileMetadataId = share.FileMetadataId,
            FileName = fileName,
            SharedWithUserId = share.SharedWithUserId,
            Permission = share.Permission,
            ExpiresAt = share.ExpiresAt,
            IsActive = share.IsActive,
            SharedBy = share.SharedBy,
            SharedAt = share.SharedAt
        };
    }
}
