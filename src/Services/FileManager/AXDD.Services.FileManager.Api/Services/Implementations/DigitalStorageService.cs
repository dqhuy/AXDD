using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for digital storage operations
/// </summary>
public class DigitalStorageService : IDigitalStorageService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<DigitalStorageService> _logger;

    public DigitalStorageService(FileManagerDbContext context, ILogger<DigitalStorageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DigitalStorageDto>> CreateAsync(CreateDigitalStorageRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BucketName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EnterpriseCode);

        try
        {
            var existingCode = await _context.DigitalStorages
                .AnyAsync(d => d.Code == request.Code && !d.IsDeleted, cancellationToken);

            if (existingCode)
            {
                return Result<DigitalStorageDto>.Failure($"Digital storage with code '{request.Code}' already exists");
            }

            var entity = new DigitalStorage
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                BucketName = request.BucketName,
                TotalCapacityBytes = request.TotalCapacityBytes,
                UsedCapacityBytes = 0,
                IsActive = request.IsActive,
                EnterpriseCode = request.EnterpriseCode,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DigitalStorages.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Digital storage created: {Code}", request.Code);

            return Result<DigitalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating digital storage: {Code}", request.Code);
            return Result<DigitalStorageDto>.Failure($"Failed to create digital storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DigitalStorageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.DigitalStorages
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<DigitalStorageDto>.Failure($"Digital storage with ID '{id}' not found");
            }

            return Result<DigitalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting digital storage: {Id}", id);
            return Result<DigitalStorageDto>.Failure($"Failed to get digital storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DigitalStorageDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        try
        {
            var entity = await _context.DigitalStorages
                .FirstOrDefaultAsync(d => d.Code == code && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<DigitalStorageDto>.Failure($"Digital storage with code '{code}' not found");
            }

            return Result<DigitalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting digital storage by code: {Code}", code);
            return Result<DigitalStorageDto>.Failure($"Failed to get digital storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<DigitalStorageDto>>> ListAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 10, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.DigitalStorages.Where(d => !d.IsDeleted);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                query = query.Where(d => d.EnterpriseCode == enterpriseCode);
            }

            if (isActive.HasValue)
            {
                query = query.Where(d => d.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(d => d.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<DigitalStorageDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<DigitalStorageDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing digital storages");
            return Result<PagedResult<DigitalStorageDto>>.Failure($"Failed to list digital storages: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DigitalStorageDto>> UpdateAsync(Guid id, CreateDigitalStorageRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var entity = await _context.DigitalStorages
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<DigitalStorageDto>.Failure($"Digital storage with ID '{id}' not found");
            }

            if (entity.Code != request.Code)
            {
                var codeExists = await _context.DigitalStorages
                    .AnyAsync(d => d.Code == request.Code && d.Id != id && !d.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    return Result<DigitalStorageDto>.Failure($"Digital storage with code '{request.Code}' already exists");
                }
            }

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.BucketName = request.BucketName;
            entity.TotalCapacityBytes = request.TotalCapacityBytes;
            entity.IsActive = request.IsActive;
            entity.EnterpriseCode = request.EnterpriseCode;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Digital storage updated: {Id}", id);

            return Result<DigitalStorageDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating digital storage: {Id}", id);
            return Result<DigitalStorageDto>.Failure($"Failed to update digital storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.DigitalStorages
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result.Failure($"Digital storage with ID '{id}' not found");
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Digital storage deleted: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting digital storage: {Id}", id);
            return Result.Failure($"Failed to delete digital storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> UpdateUsageAsync(Guid id, long bytesUsed, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.DigitalStorages
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result.Failure($"Digital storage with ID '{id}' not found");
            }

            entity.UsedCapacityBytes += bytesUsed;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Digital storage usage updated: {Id}, Bytes: {Bytes}", id, bytesUsed);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating digital storage usage: {Id}", id);
            return Result.Failure($"Failed to update digital storage usage: {ex.Message}");
        }
    }

    private static DigitalStorageDto MapToDto(DigitalStorage entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        BucketName = entity.BucketName,
        TotalCapacityBytes = entity.TotalCapacityBytes,
        UsedCapacityBytes = entity.UsedCapacityBytes,
        AvailableCapacityBytes = entity.AvailableCapacityBytes,
        UsagePercentage = entity.UsagePercentage,
        IsActive = entity.IsActive,
        EnterpriseCode = entity.EnterpriseCode,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
