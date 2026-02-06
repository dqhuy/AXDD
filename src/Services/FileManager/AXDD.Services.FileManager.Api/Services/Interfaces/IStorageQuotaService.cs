using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for storage quota operations
/// </summary>
public interface IStorageQuotaService
{
    /// <summary>
    /// Gets the storage quota for an enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the storage quota</returns>
    Task<Result<StorageQuotaDto>> GetQuotaAsync(string enterpriseCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the storage usage for an enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="sizeChange">The size change in bytes (positive or negative)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the operation</returns>
    Task<Result> UpdateUsageAsync(string enterpriseCode, long sizeChange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there's enough quota for a required size
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="requiredSize">The required size in bytes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating whether quota is available</returns>
    Task<Result<bool>> CheckQuotaAsync(
        string enterpriseCode,
        long requiredSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes quota for a new enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="quotaBytes">Optional custom quota in bytes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing the created quota</returns>
    Task<Result<StorageQuotaDto>> InitializeQuotaAsync(
        string enterpriseCode,
        long? quotaBytes = null,
        CancellationToken cancellationToken = default);
}
