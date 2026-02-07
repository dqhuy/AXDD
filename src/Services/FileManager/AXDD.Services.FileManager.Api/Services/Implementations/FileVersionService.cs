using System.Security.Cryptography;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for file version operations
/// </summary>
public class FileVersionService : IFileVersionService
{
    private readonly FileManagerDbContext _context;
    private readonly IMinioService _minioService;
    private readonly ILogger<FileVersionService> _logger;

    /// <summary>
    /// Initializes a new instance of FileVersionService
    /// </summary>
    public FileVersionService(
        FileManagerDbContext context,
        IMinioService minioService,
        ILogger<FileVersionService> logger)
    {
        _context = context;
        _minioService = minioService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<FileVersionDto>> CreateVersionAsync(
        Guid fileId,
        Stream fileStream,
        string userId,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream, nameof(fileStream));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var fileMetadata = await _context.FileMetadata
                .Where(f => f.Id == fileId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileMetadata == null)
            {
                return Result<FileVersionDto>.Failure($"File with ID '{fileId}' not found");
            }

            // Calculate checksum
            var checksum = await CalculateChecksumAsync(fileStream, cancellationToken);
            fileStream.Position = 0;

            // Get the next version number
            var maxVersion = await _context.FileVersions
                .Where(v => v.FileMetadataId == fileId)
                .MaxAsync(v => (int?)v.Version, cancellationToken) ?? 0;

            var newVersion = maxVersion + 1;

            // Generate new object key for this version
            var objectKey = GenerateVersionObjectKey(fileMetadata.ObjectKey, newVersion);

            // Upload to MinIO
            await _minioService.UploadFileAsync(
                fileMetadata.BucketName,
                objectKey,
                fileStream,
                fileMetadata.MimeType,
                cancellationToken);

            // Create new version record
            var fileVersion = new FileVersion
            {
                FileMetadataId = fileId,
                Version = newVersion,
                ObjectKey = objectKey,
                FileSize = fileStream.Length,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow,
                Checksum = checksum,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.FileVersions.Add(fileVersion);

            // Mark previous version as not latest
            fileMetadata.IsLatest = false;

            // Update file metadata with new version
            fileMetadata.Version = newVersion;
            fileMetadata.IsLatest = true;
            fileMetadata.ObjectKey = objectKey;
            fileMetadata.FileSize = fileStream.Length;
            fileMetadata.Checksum = checksum;
            fileMetadata.UpdatedAt = DateTime.UtcNow;
            fileMetadata.UpdatedBy = userId;

            _context.FileMetadata.Update(fileMetadata);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "New file version created: FileId={FileId}, Version={Version}",
                fileId, newVersion);

            var dto = MapToDto(fileVersion);
            return Result<FileVersionDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating file version: FileId={FileId}", fileId);
            return Result<FileVersionDto>.Failure($"Failed to create file version: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<FileVersionDto>>> GetVersionsAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fileMetadata = await _context.FileMetadata
                .Where(f => f.Id == fileId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileMetadata == null)
            {
                return Result<List<FileVersionDto>>.Failure($"File with ID '{fileId}' not found");
            }

            var versions = await _context.FileVersions
                .Where(v => v.FileMetadataId == fileId)
                .OrderByDescending(v => v.Version)
                .ToListAsync(cancellationToken);

            var dtos = versions.Select(MapToDto).ToList();

            return Result<List<FileVersionDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file versions: FileId={FileId}", fileId);
            return Result<List<FileVersionDto>>.Failure($"Failed to get file versions: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FileMetadataDto>> RestoreVersionAsync(
        Guid fileId,
        int version,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var fileMetadata = await _context.FileMetadata
                .Where(f => f.Id == fileId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileMetadata == null)
            {
                return Result<FileMetadataDto>.Failure($"File with ID '{fileId}' not found");
            }

            var versionToRestore = await _context.FileVersions
                .Where(v => v.FileMetadataId == fileId && v.Version == version)
                .FirstOrDefaultAsync(cancellationToken);

            if (versionToRestore == null)
            {
                return Result<FileMetadataDto>.Failure($"Version {version} not found for file '{fileId}'");
            }

            // Update file metadata to point to the restored version
            fileMetadata.ObjectKey = versionToRestore.ObjectKey;
            fileMetadata.FileSize = versionToRestore.FileSize;
            fileMetadata.Checksum = versionToRestore.Checksum;
            fileMetadata.Version = fileMetadata.Version + 1; // Increment version
            fileMetadata.IsLatest = true;
            fileMetadata.UpdatedAt = DateTime.UtcNow;
            fileMetadata.UpdatedBy = userId;

            // Create a new version entry for the restore
            var newVersion = new FileVersion
            {
                FileMetadataId = fileId,
                Version = fileMetadata.Version,
                ObjectKey = versionToRestore.ObjectKey,
                FileSize = versionToRestore.FileSize,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow,
                Checksum = versionToRestore.Checksum,
                Notes = $"Restored from version {version}",
                CreatedAt = DateTime.UtcNow
            };

            _context.FileVersions.Add(newVersion);
            _context.FileMetadata.Update(fileMetadata);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "File version restored: FileId={FileId}, RestoredVersion={Version}, NewVersion={NewVersion}",
                fileId, version, fileMetadata.Version);

            var dto = await MapFileMetadataToDto(fileMetadata, cancellationToken);
            return Result<FileMetadataDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring file version: FileId={FileId}, Version={Version}", fileId, version);
            return Result<FileMetadataDto>.Failure($"Failed to restore file version: {ex.Message}");
        }
    }

    private static string GenerateVersionObjectKey(string originalObjectKey, int version)
    {
        var extension = Path.GetExtension(originalObjectKey);
        var pathWithoutExtension = originalObjectKey[..^extension.Length];
        return $"{pathWithoutExtension}_v{version}{extension}";
    }

    private static async Task<string> CalculateChecksumAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }

    private static FileVersionDto MapToDto(FileVersion version)
    {
        return new FileVersionDto
        {
            Id = version.Id,
            FileMetadataId = version.FileMetadataId,
            Version = version.Version,
            FileSize = version.FileSize,
            UploadedBy = version.UploadedBy,
            UploadedAt = version.UploadedAt,
            Checksum = version.Checksum,
            Notes = version.Notes
        };
    }

    private async Task<FileMetadataDto> MapFileMetadataToDto(FileMetadata fileMetadata, CancellationToken cancellationToken)
    {
        string? folderName = null;
        if (fileMetadata.FolderId.HasValue)
        {
            folderName = await _context.Folders
                .Where(f => f.Id == fileMetadata.FolderId.Value)
                .Select(f => f.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new FileMetadataDto
        {
            Id = fileMetadata.Id,
            FileName = fileMetadata.FileName,
            FileSize = fileMetadata.FileSize,
            MimeType = fileMetadata.MimeType,
            Extension = fileMetadata.Extension,
            EnterpriseCode = fileMetadata.EnterpriseCode,
            FolderId = fileMetadata.FolderId,
            FolderName = folderName,
            Version = fileMetadata.Version,
            IsLatest = fileMetadata.IsLatest,
            UploadedBy = fileMetadata.UploadedBy,
            UploadedAt = fileMetadata.UploadedAt,
            Checksum = fileMetadata.Checksum,
            Description = fileMetadata.Description,
            Tags = fileMetadata.Tags,
            CreatedAt = fileMetadata.CreatedAt,
            UpdatedAt = fileMetadata.UpdatedAt
        };
    }
}
