using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for security level operations
/// </summary>
public class SecurityLevelService : ISecurityLevelService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<SecurityLevelService> _logger;

    public SecurityLevelService(FileManagerDbContext context, ILogger<SecurityLevelService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<SecurityLevelDto>> CreateAsync(CreateSecurityLevelRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        try
        {
            var existingCode = await _context.SecurityLevels
                .AnyAsync(s => s.Code == request.Code && !s.IsDeleted, cancellationToken);

            if (existingCode)
            {
                return Result<SecurityLevelDto>.Failure($"Security level with code '{request.Code}' already exists");
            }

            var entity = new SecurityLevel
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Level = request.Level,
                RequiresApproval = request.RequiresApproval,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.SecurityLevels.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Security level created: {Code}", request.Code);

            return Result<SecurityLevelDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating security level: {Code}", request.Code);
            return Result<SecurityLevelDto>.Failure($"Failed to create security level: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<SecurityLevelDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.SecurityLevels
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<SecurityLevelDto>.Failure($"Security level with ID '{id}' not found");
            }

            return Result<SecurityLevelDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security level: {Id}", id);
            return Result<SecurityLevelDto>.Failure($"Failed to get security level: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<SecurityLevelDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        try
        {
            var entity = await _context.SecurityLevels
                .FirstOrDefaultAsync(s => s.Code == code && !s.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<SecurityLevelDto>.Failure($"Security level with code '{code}' not found");
            }

            return Result<SecurityLevelDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security level by code: {Code}", code);
            return Result<SecurityLevelDto>.Failure($"Failed to get security level: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<SecurityLevelDto>>> ListAsync(int pageNumber = 1, int pageSize = 10, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.SecurityLevels.Where(s => !s.IsDeleted);

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(s => s.Level)
                .ThenBy(s => s.DisplayOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<SecurityLevelDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<SecurityLevelDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing security levels");
            return Result<PagedResult<SecurityLevelDto>>.Failure($"Failed to list security levels: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<SecurityLevelDto>> UpdateAsync(Guid id, CreateSecurityLevelRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var entity = await _context.SecurityLevels
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<SecurityLevelDto>.Failure($"Security level with ID '{id}' not found");
            }

            if (entity.Code != request.Code)
            {
                var codeExists = await _context.SecurityLevels
                    .AnyAsync(s => s.Code == request.Code && s.Id != id && !s.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    return Result<SecurityLevelDto>.Failure($"Security level with code '{request.Code}' already exists");
                }
            }

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Level = request.Level;
            entity.RequiresApproval = request.RequiresApproval;
            entity.IsActive = request.IsActive;
            entity.DisplayOrder = request.DisplayOrder;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Security level updated: {Id}", id);

            return Result<SecurityLevelDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating security level: {Id}", id);
            return Result<SecurityLevelDto>.Failure($"Failed to update security level: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.SecurityLevels
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result.Failure($"Security level with ID '{id}' not found");
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Security level deleted: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting security level: {Id}", id);
            return Result.Failure($"Failed to delete security level: {ex.Message}");
        }
    }

    private static SecurityLevelDto MapToDto(SecurityLevel entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        Level = entity.Level,
        RequiresApproval = entity.RequiresApproval,
        IsActive = entity.IsActive,
        DisplayOrder = entity.DisplayOrder,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
