using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for physical storage operations
/// </summary>
public class PhysicalStorageService : IPhysicalStorageService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<PhysicalStorageService> _logger;

    public PhysicalStorageService(FileManagerDbContext context, ILogger<PhysicalStorageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<PhysicalStorageDto>> CreateAsync(CreatePhysicalStorageRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EnterpriseCode);

        try
        {
            var existingCode = await _context.PhysicalStorages
                .AnyAsync(p => p.Code == request.Code && !p.IsDeleted, cancellationToken);

            if (existingCode)
            {
                return Result<PhysicalStorageDto>.Failure($"Physical storage with code '{request.Code}' already exists");
            }

            var entity = new PhysicalStorage
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                IsActive = request.IsActive,
                EnterpriseCode = request.EnterpriseCode,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.PhysicalStorages.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Physical storage created: {Code}", request.Code);

            return Result<PhysicalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating physical storage: {Code}", request.Code);
            return Result<PhysicalStorageDto>.Failure($"Failed to create physical storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PhysicalStorageDto>> GetByIdAsync(Guid id, bool includeLocations = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.PhysicalStorages.AsQueryable();

            if (includeLocations)
            {
                query = query.Include(p => p.Locations.Where(l => !l.IsDeleted));
            }

            var entity = await query
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<PhysicalStorageDto>.Failure($"Physical storage with ID '{id}' not found");
            }

            return Result<PhysicalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting physical storage: {Id}", id);
            return Result<PhysicalStorageDto>.Failure($"Failed to get physical storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PhysicalStorageDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        try
        {
            var entity = await _context.PhysicalStorages
                .Include(p => p.Locations.Where(l => !l.IsDeleted))
                .FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<PhysicalStorageDto>.Failure($"Physical storage with code '{code}' not found");
            }

            return Result<PhysicalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting physical storage by code: {Code}", code);
            return Result<PhysicalStorageDto>.Failure($"Failed to get physical storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<PhysicalStorageDto>>> ListAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 10, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.PhysicalStorages
                .Include(p => p.Locations.Where(l => !l.IsDeleted))
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                query = query.Where(p => p.EnterpriseCode == enterpriseCode);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<PhysicalStorageDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<PhysicalStorageDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing physical storages");
            return Result<PagedResult<PhysicalStorageDto>>.Failure($"Failed to list physical storages: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PhysicalStorageDto>> UpdateAsync(Guid id, CreatePhysicalStorageRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var entity = await _context.PhysicalStorages
                .Include(p => p.Locations.Where(l => !l.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<PhysicalStorageDto>.Failure($"Physical storage with ID '{id}' not found");
            }

            if (entity.Code != request.Code)
            {
                var codeExists = await _context.PhysicalStorages
                    .AnyAsync(p => p.Code == request.Code && p.Id != id && !p.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    return Result<PhysicalStorageDto>.Failure($"Physical storage with code '{request.Code}' already exists");
                }
            }

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Address = request.Address;
            entity.IsActive = request.IsActive;
            entity.EnterpriseCode = request.EnterpriseCode;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Physical storage updated: {Id}", id);

            return Result<PhysicalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating physical storage: {Id}", id);
            return Result<PhysicalStorageDto>.Failure($"Failed to update physical storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PhysicalStorages
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result.Failure($"Physical storage with ID '{id}' not found");
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Physical storage deleted: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting physical storage: {Id}", id);
            return Result.Failure($"Failed to delete physical storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PhysicalStorageLocationDto>> AddLocationAsync(Guid storageId, CreatePhysicalStorageLocationRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var storage = await _context.PhysicalStorages
                .FirstOrDefaultAsync(p => p.Id == storageId && !p.IsDeleted, cancellationToken);

            if (storage == null)
            {
                return Result<PhysicalStorageLocationDto>.Failure($"Physical storage with ID '{storageId}' not found");
            }

            var locationExists = await _context.PhysicalStorageLocations
                .AnyAsync(l => l.PhysicalStorageId == storageId && l.Code == request.Code && !l.IsDeleted, cancellationToken);

            if (locationExists)
            {
                return Result<PhysicalStorageLocationDto>.Failure($"Location with code '{request.Code}' already exists in this storage");
            }

            var location = new PhysicalStorageLocation
            {
                PhysicalStorageId = storageId,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsOccupied = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.PhysicalStorageLocations.Add(location);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Location added to physical storage {StorageId}: {Code}", storageId, request.Code);

            return Result<PhysicalStorageLocationDto>.Success(MapLocationToDto(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding location to physical storage: {StorageId}", storageId);
            return Result<PhysicalStorageLocationDto>.Failure($"Failed to add location: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PhysicalStorageLocationDto>> UpdateLocationAsync(Guid storageId, Guid locationId, CreatePhysicalStorageLocationRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var location = await _context.PhysicalStorageLocations
                .FirstOrDefaultAsync(l => l.Id == locationId && l.PhysicalStorageId == storageId && !l.IsDeleted, cancellationToken);

            if (location == null)
            {
                return Result<PhysicalStorageLocationDto>.Failure($"Location with ID '{locationId}' not found");
            }

            if (location.Code != request.Code)
            {
                var codeExists = await _context.PhysicalStorageLocations
                    .AnyAsync(l => l.PhysicalStorageId == storageId && l.Code == request.Code && l.Id != locationId && !l.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    return Result<PhysicalStorageLocationDto>.Failure($"Location with code '{request.Code}' already exists in this storage");
                }
            }

            location.Code = request.Code;
            location.Name = request.Name;
            location.Description = request.Description;
            location.UpdatedAt = DateTime.UtcNow;
            location.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Location updated in physical storage {StorageId}: {LocationId}", storageId, locationId);

            return Result<PhysicalStorageLocationDto>.Success(MapLocationToDto(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location: {LocationId}", locationId);
            return Result<PhysicalStorageLocationDto>.Failure($"Failed to update location: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteLocationAsync(Guid storageId, Guid locationId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _context.PhysicalStorageLocations
                .FirstOrDefaultAsync(l => l.Id == locationId && l.PhysicalStorageId == storageId && !l.IsDeleted, cancellationToken);

            if (location == null)
            {
                return Result.Failure($"Location with ID '{locationId}' not found");
            }

            if (location.IsOccupied)
            {
                return Result.Failure("Cannot delete an occupied location");
            }

            location.IsDeleted = true;
            location.DeletedAt = DateTime.UtcNow;
            location.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Location deleted from physical storage {StorageId}: {LocationId}", storageId, locationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting location: {LocationId}", locationId);
            return Result.Failure($"Failed to delete location: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<PhysicalStorageLocationDto>>> ListLocationsAsync(Guid storageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var locations = await _context.PhysicalStorageLocations
                .Where(l => l.PhysicalStorageId == storageId && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync(cancellationToken);

            return Result<List<PhysicalStorageLocationDto>>.Success(locations.Select(MapLocationToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing locations for storage: {StorageId}", storageId);
            return Result<List<PhysicalStorageLocationDto>>.Failure($"Failed to list locations: {ex.Message}");
        }
    }

    private static PhysicalStorageDto MapToDto(PhysicalStorage entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        Address = entity.Address,
        IsActive = entity.IsActive,
        EnterpriseCode = entity.EnterpriseCode,
        LocationCount = entity.Locations?.Count(l => !l.IsDeleted) ?? 0,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Locations = entity.Locations?.Where(l => !l.IsDeleted).Select(MapLocationToDto).ToList() ?? []
    };

    private static PhysicalStorageLocationDto MapLocationToDto(PhysicalStorageLocation location) => new()
    {
        Id = location.Id,
        PhysicalStorageId = location.PhysicalStorageId,
        Code = location.Code,
        Name = location.Name,
        Description = location.Description,
        IsOccupied = location.IsOccupied,
        CreatedAt = location.CreatedAt,
        UpdatedAt = location.UpdatedAt
    };
}
