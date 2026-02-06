using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for file operations
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Uploads a file to storage
    /// </summary>
    /// <param name="fileStream">The file stream</param>
    /// <param name="fileName">The file name</param>
    /// <param name="mimeType">The MIME type</param>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="folderId">Optional folder ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="description">Optional description</param>
    /// <param name="tags">Optional tags</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the file metadata</returns>
    Task<Result<FileMetadataDto>> UploadAsync(
        Stream fileStream,
        string fileName,
        string mimeType,
        string enterpriseCode,
        Guid? folderId,
        string userId,
        string? description = null,
        string? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from storage
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the file stream, filename, and MIME type</returns>
    Task<Result<(Stream Stream, string FileName, string MimeType)>> DownloadAsync(
        Guid fileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a presigned URL for viewing/downloading a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="expiryMinutes">URL expiry in minutes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the presigned URL</returns>
    Task<Result<string>> GetFileUrlAsync(
        Guid fileId,
        int expiryMinutes = 60,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file (soft delete)
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> DeleteAsync(Guid fileId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file metadata by ID
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the file metadata</returns>
    Task<Result<FileMetadataDto>> GetFileMetadataAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists files with pagination
    /// </summary>
    /// <param name="query">The query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing paginated file list</returns>
    Task<Result<PagedResult<FileMetadataDto>>> ListFilesAsync(
        FileListQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there's enough quota for a file
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="fileSize">The file size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating whether quota is available</returns>
    Task<Result<bool>> CheckQuotaAsync(
        string enterpriseCode,
        long fileSize,
        CancellationToken cancellationToken = default);
}
