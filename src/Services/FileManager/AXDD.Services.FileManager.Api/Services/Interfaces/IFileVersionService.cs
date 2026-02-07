using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for file version operations
/// </summary>
public interface IFileVersionService
{
    /// <summary>
    /// Creates a new version of a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="fileStream">The file stream</param>
    /// <param name="userId">The user ID</param>
    /// <param name="notes">Optional version notes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the new version</returns>
    Task<Result<FileVersionDto>> CreateVersionAsync(
        Guid fileId,
        Stream fileStream,
        string userId,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all versions of a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the list of versions</returns>
    Task<Result<List<FileVersionDto>>> GetVersionsAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a specific version of a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="version">The version number to restore</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result<FileMetadataDto>> RestoreVersionAsync(
        Guid fileId,
        int version,
        string userId,
        CancellationToken cancellationToken = default);
}
