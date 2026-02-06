using System.Security.Cryptography;
using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Exceptions;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using AXDD.Services.FileManager.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for file operations
/// </summary>
public class FileService : IFileService
{
    private readonly FileManagerDbContext _context;
    private readonly IMinioService _minioService;
    private readonly IStorageQuotaService _quotaService;
    private readonly IFolderService _folderService;
    private readonly MinioSettings _minioSettings;
    private readonly FileUploadSettings _uploadSettings;
    private readonly ILogger<FileService> _logger;

    /// <summary>
    /// Initializes a new instance of FileService
    /// </summary>
    public FileService(
        FileManagerDbContext context,
        IMinioService minioService,
        IStorageQuotaService quotaService,
        IFolderService folderService,
        IOptions<MinioSettings> minioSettings,
        IOptions<FileUploadSettings> uploadSettings,
        ILogger<FileService> logger)
    {
        _context = context;
        _minioService = minioService;
        _quotaService = quotaService;
        _folderService = folderService;
        _minioSettings = minioSettings.Value;
        _uploadSettings = uploadSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<FileMetadataDto>> UploadAsync(
        Stream fileStream,
        string fileName,
        string mimeType,
        string enterpriseCode,
        Guid? folderId,
        string userId,
        string? description = null,
        string? tags = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream, nameof(fileStream));
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Validate file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_uploadSettings.AllowedExtensions.Contains(extension))
            {
                return Result<FileMetadataDto>.Failure(
                    $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _uploadSettings.AllowedExtensions)}");
            }

            // Validate file size
            var fileSize = fileStream.Length;
            if (fileSize > _uploadSettings.MaxFileSizeBytes)
            {
                return Result<FileMetadataDto>.Failure(
                    $"File size {fileSize:N0} bytes exceeds the maximum allowed size of {_uploadSettings.MaxFileSizeBytes:N0} bytes");
            }

            // Check storage quota
            var quotaCheckResult = await _quotaService.CheckQuotaAsync(enterpriseCode, fileSize, cancellationToken);
            if (quotaCheckResult.IsFailure || !quotaCheckResult.Value)
            {
                return Result<FileMetadataDto>.Failure(
                    $"Storage quota exceeded for enterprise '{enterpriseCode}'");
            }

            // Validate folder if specified
            if (folderId.HasValue)
            {
                var folderResult = await _folderService.GetFolderAsync(folderId.Value, cancellationToken);
                if (folderResult.IsFailure)
                {
                    return Result<FileMetadataDto>.Failure($"Folder with ID '{folderId}' not found");
                }

                var folder = folderResult.Value!;
                if (folder.EnterpriseCode != enterpriseCode)
                {
                    return Result<FileMetadataDto>.Failure("Folder does not belong to the specified enterprise");
                }
            }
            else
            {
                // Ensure root folder exists
                var rootFolderResult = await _folderService.GetRootFolderAsync(enterpriseCode, cancellationToken);
                if (rootFolderResult.IsSuccess)
                {
                    folderId = rootFolderResult.Value!.Id;
                }
            }

            // Calculate checksum
            var checksum = await CalculateChecksumAsync(fileStream, cancellationToken);
            fileStream.Position = 0; // Reset stream position

            // Virus scanning placeholder
            if (_uploadSettings.EnableVirusScanning)
            {
                _logger.LogInformation("Virus scanning is enabled but not implemented yet for file: {FileName}", fileName);
                // TODO: Implement virus scanning
            }

            // Generate unique object key
            var bucketName = _minioSettings.BucketNames.Documents;
            var objectKey = GenerateObjectKey(enterpriseCode, fileName);

            // Upload to MinIO
            await _minioService.UploadFileAsync(bucketName, objectKey, fileStream, mimeType, cancellationToken);

            // Create file metadata
            var fileMetadata = new FileMetadata
            {
                FileName = fileName,
                FileSize = fileSize,
                MimeType = mimeType,
                Extension = extension,
                BucketName = bucketName,
                ObjectKey = objectKey,
                EnterpriseCode = enterpriseCode,
                FolderId = folderId,
                Version = 1,
                IsLatest = true,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow,
                Checksum = checksum,
                Description = description,
                Tags = tags,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.FileMetadata.Add(fileMetadata);

            // Create initial version
            var fileVersion = new FileVersion
            {
                FileMetadataId = fileMetadata.Id,
                Version = 1,
                ObjectKey = objectKey,
                FileSize = fileSize,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow,
                Checksum = checksum,
                CreatedAt = DateTime.UtcNow
            };

            _context.FileVersions.Add(fileVersion);

            // Update storage quota
            await _quotaService.UpdateUsageAsync(enterpriseCode, fileSize, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "File uploaded successfully: Id={FileId}, Name={FileName}, Size={FileSize}, Enterprise={EnterpriseCode}",
                fileMetadata.Id, fileName, fileSize, enterpriseCode);

            var dto = await MapToDto(fileMetadata, cancellationToken);
            return Result<FileMetadataDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: FileName={FileName}, Enterprise={EnterpriseCode}", fileName, enterpriseCode);
            return Result<FileMetadataDto>.Failure($"Failed to upload file: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<(Stream Stream, string FileName, string MimeType)>> DownloadAsync(
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
                return Result<(Stream, string, string)>.Failure($"File with ID '{fileId}' not found");
            }

            var stream = await _minioService.DownloadFileAsync(
                fileMetadata.BucketName,
                fileMetadata.ObjectKey,
                cancellationToken);

            _logger.LogInformation("File downloaded: Id={FileId}, Name={FileName}", fileId, fileMetadata.FileName);

            return Result<(Stream, string, string)>.Success((stream, fileMetadata.FileName, fileMetadata.MimeType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: FileId={FileId}", fileId);
            return Result<(Stream, string, string)>.Failure($"Failed to download file: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<string>> GetFileUrlAsync(
        Guid fileId,
        int expiryMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fileMetadata = await _context.FileMetadata
                .Where(f => f.Id == fileId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileMetadata == null)
            {
                return Result<string>.Failure($"File with ID '{fileId}' not found");
            }

            var expirySeconds = expiryMinutes * 60;
            var url = await _minioService.GetPresignedUrlAsync(
                fileMetadata.BucketName,
                fileMetadata.ObjectKey,
                expirySeconds,
                cancellationToken);

            _logger.LogInformation("Presigned URL generated for file: Id={FileId}, ExpiryMinutes={ExpiryMinutes}", fileId, expiryMinutes);

            return Result<string>.Success(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating file URL: FileId={FileId}", fileId);
            return Result<string>.Failure($"Failed to generate file URL: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(
        Guid fileId,
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
                return Result.Failure($"File with ID '{fileId}' not found");
            }

            // Soft delete
            fileMetadata.IsDeleted = true;
            fileMetadata.DeletedAt = DateTime.UtcNow;
            fileMetadata.DeletedBy = userId;

            _context.FileMetadata.Update(fileMetadata);

            // Update storage quota
            await _quotaService.UpdateUsageAsync(fileMetadata.EnterpriseCode, -fileMetadata.FileSize, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("File deleted: Id={FileId}, Name={FileName}", fileId, fileMetadata.FileName);

            // Note: Actual MinIO file deletion can be done asynchronously via a background job
            // For now, we keep the file in MinIO for potential recovery

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: FileId={FileId}", fileId);
            return Result.Failure($"Failed to delete file: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FileMetadataDto>> GetFileMetadataAsync(
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
                return Result<FileMetadataDto>.Failure($"File with ID '{fileId}' not found");
            }

            var dto = await MapToDto(fileMetadata, cancellationToken);
            return Result<FileMetadataDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file metadata: FileId={FileId}", fileId);
            return Result<FileMetadataDto>.Failure($"Failed to get file metadata: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<FileMetadataDto>>> ListFilesAsync(
        FileListQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));

        try
        {
            var dbQuery = _context.FileMetadata
                .Where(f => !f.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.EnterpriseCode))
            {
                dbQuery = dbQuery.Where(f => f.EnterpriseCode == query.EnterpriseCode);
            }

            if (query.FolderId.HasValue)
            {
                dbQuery = dbQuery.Where(f => f.FolderId == query.FolderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                dbQuery = dbQuery.Where(f =>
                    f.FileName.Contains(query.SearchTerm) ||
                    (f.Description != null && f.Description.Contains(query.SearchTerm)) ||
                    (f.Tags != null && f.Tags.Contains(query.SearchTerm)));
            }

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            var files = await dbQuery
                .OrderByDescending(f => f.UploadedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = new List<FileMetadataDto>();
            foreach (var file in files)
            {
                dtos.Add(await MapToDto(file, cancellationToken));
            }

            var pagedResult = new PagedResult<FileMetadataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<PagedResult<FileMetadataDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files");
            return Result<PagedResult<FileMetadataDto>>.Failure($"Failed to list files: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> CheckQuotaAsync(
        string enterpriseCode,
        long fileSize,
        CancellationToken cancellationToken = default)
    {
        return await _quotaService.CheckQuotaAsync(enterpriseCode, fileSize, cancellationToken);
    }

    private static string GenerateObjectKey(string enterpriseCode, string fileName)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var uniqueId = Guid.NewGuid().ToString("N");
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        // Sanitize filename
        var sanitizedFileName = string.Join("_", fileNameWithoutExtension.Split(Path.GetInvalidFileNameChars()));

        return $"{enterpriseCode}/{timestamp}/{uniqueId}_{sanitizedFileName}{extension}";
    }

    private static async Task<string> CalculateChecksumAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }

    private async Task<FileMetadataDto> MapToDto(FileMetadata fileMetadata, CancellationToken cancellationToken)
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
