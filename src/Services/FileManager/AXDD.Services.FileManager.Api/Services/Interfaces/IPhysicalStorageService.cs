using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for physical storage operations
/// </summary>
public interface IPhysicalStorageService
{
    /// <summary>
    /// Creates a new physical storage
    /// </summary>
    Task<Result<PhysicalStorageDto>> CreateAsync(CreatePhysicalStorageRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a physical storage by ID
    /// </summary>
    Task<Result<PhysicalStorageDto>> GetByIdAsync(Guid id, bool includeLocations = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a physical storage by code
    /// </summary>
    Task<Result<PhysicalStorageDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all physical storages
    /// </summary>
    Task<Result<PagedResult<PhysicalStorageDto>>> ListAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 10, bool? isActive = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a physical storage
    /// </summary>
    Task<Result<PhysicalStorageDto>> UpdateAsync(Guid id, CreatePhysicalStorageRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a physical storage
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a location to a physical storage
    /// </summary>
    Task<Result<PhysicalStorageLocationDto>> AddLocationAsync(Guid storageId, CreatePhysicalStorageLocationRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a location in a physical storage
    /// </summary>
    Task<Result<PhysicalStorageLocationDto>> UpdateLocationAsync(Guid storageId, Guid locationId, CreatePhysicalStorageLocationRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a location from a physical storage
    /// </summary>
    Task<Result> DeleteLocationAsync(Guid storageId, Guid locationId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists locations in a physical storage
    /// </summary>
    Task<Result<List<PhysicalStorageLocationDto>>> ListLocationsAsync(Guid storageId, CancellationToken cancellationToken = default);
}
