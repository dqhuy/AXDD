using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for file sharing operations
/// </summary>
public interface IFileShareService
{
    /// <summary>
    /// Shares a file with a user
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="sharedWithUserId">The user ID to share with</param>
    /// <param name="permission">The permission level</param>
    /// <param name="sharedBy">The user ID who is sharing</param>
    /// <param name="expiresAt">Optional expiry date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the file share</returns>
    Task<Result<FileShareDto>> ShareFileAsync(
        Guid fileId,
        string sharedWithUserId,
        string permission,
        string sharedBy,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets files shared with a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the list of shared files</returns>
    Task<Result<List<FileShareDto>>> GetSharedFilesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a file share
    /// </summary>
    /// <param name="shareId">The share ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> RevokeShareAsync(Guid shareId, CancellationToken cancellationToken = default);
}
