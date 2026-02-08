using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for digital storage operations
/// </summary>
public interface IDigitalStorageService
{
    /// <summary>
    /// Creates a new digital storage
    /// </summary>
    Task<Result<DigitalStorageDto>> CreateAsync(CreateDigitalStorageRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a digital storage by ID
    /// </summary>
    Task<Result<DigitalStorageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a digital storage by code
    /// </summary>
    Task<Result<DigitalStorageDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all digital storages
    /// </summary>
    Task<Result<PagedResult<DigitalStorageDto>>> ListAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 10, bool? isActive = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a digital storage
    /// </summary>
    Task<Result<DigitalStorageDto>> UpdateAsync(Guid id, CreateDigitalStorageRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a digital storage
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates usage for a digital storage
    /// </summary>
    Task<Result> UpdateUsageAsync(Guid id, long bytesUsed, CancellationToken cancellationToken = default);
}
