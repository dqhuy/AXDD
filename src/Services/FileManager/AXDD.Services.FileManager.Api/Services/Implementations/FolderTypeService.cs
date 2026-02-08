using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for folder type operations
/// </summary>
public class FolderTypeService : IFolderTypeService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<FolderTypeService> _logger;

    public FolderTypeService(FileManagerDbContext context, ILogger<FolderTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<FolderTypeDto>> CreateAsync(CreateFolderTypeRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        try
        {
            var existingCode = await _context.FolderTypes
                .AnyAsync(f => f.Code == request.Code && !f.IsDeleted, cancellationToken);

            if (existingCode)
            {
                return Result<FolderTypeDto>.Failure($"Folder type with code '{request.Code}' already exists");
            }

            var entity = new FolderType
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                RetentionPeriodMonths = request.RetentionPeriodMonths,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.FolderTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder type created: {Code}", request.Code);

            return Result<FolderTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating folder type: {Code}", request.Code);
            return Result<FolderTypeDto>.Failure($"Failed to create folder type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.FolderTypes
                .Include(f => f.MetadataFields.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<FolderTypeDto>.Failure($"Folder type with ID '{id}' not found");
            }

            return Result<FolderTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting folder type: {Id}", id);
            return Result<FolderTypeDto>.Failure($"Failed to get folder type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderTypeDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        try
        {
            var entity = await _context.FolderTypes
                .Include(f => f.MetadataFields.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(f => f.Code == code && !f.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<FolderTypeDto>.Failure($"Folder type with code '{code}' not found");
            }

            return Result<FolderTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting folder type by code: {Code}", code);
            return Result<FolderTypeDto>.Failure($"Failed to get folder type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<FolderTypeDto>>> ListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.FolderTypes
                .Include(f => f.MetadataFields.Where(m => !m.IsDeleted))
                .Where(f => !f.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(f => f.Name.Contains(searchTerm) || f.Code.Contains(searchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(f => f.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(f => f.DisplayOrder)
                .ThenBy(f => f.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<FolderTypeDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<FolderTypeDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing folder types");
            return Result<PagedResult<FolderTypeDto>>.Failure($"Failed to list folder types: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderTypeDto>> UpdateAsync(Guid id, CreateFolderTypeRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var entity = await _context.FolderTypes
                .Include(f => f.MetadataFields.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<FolderTypeDto>.Failure($"Folder type with ID '{id}' not found");
            }

            if (entity.Code != request.Code)
            {
                var codeExists = await _context.FolderTypes
                    .AnyAsync(f => f.Code == request.Code && f.Id != id && !f.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    return Result<FolderTypeDto>.Failure($"Folder type with code '{request.Code}' already exists");
                }
            }

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.RetentionPeriodMonths = request.RetentionPeriodMonths;
            entity.DisplayOrder = request.DisplayOrder;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder type updated: {Id}", id);

            return Result<FolderTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating folder type: {Id}", id);
            return Result<FolderTypeDto>.Failure($"Failed to update folder type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.FolderTypes
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result.Failure($"Folder type with ID '{id}' not found");
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Folder type deleted: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting folder type: {Id}", id);
            return Result.Failure($"Failed to delete folder type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FolderTypeMetadataFieldDto>> AddMetadataFieldAsync(Guid folderTypeId, CreateFolderTypeMetadataFieldRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var folderType = await _context.FolderTypes
                .FirstOrDefaultAsync(f => f.Id == folderTypeId && !f.IsDeleted, cancellationToken);

            if (folderType == null)
            {
                return Result<FolderTypeMetadataFieldDto>.Failure($"Folder type with ID '{folderTypeId}' not found");
            }

            var fieldExists = await _context.FolderTypeMetadataFields
                .AnyAsync(f => f.FolderTypeId == folderTypeId && f.FieldName == request.FieldName && !f.IsDeleted, cancellationToken);

            if (fieldExists)
            {
                return Result<FolderTypeMetadataFieldDto>.Failure($"Field '{request.FieldName}' already exists for this folder type");
            }

            var field = new FolderTypeMetadataField
            {
                FolderTypeId = folderTypeId,
                FieldName = request.FieldName,
                DisplayName = request.DisplayName,
                DataType = request.DataType,
                IsRequired = request.IsRequired,
                DefaultValue = request.DefaultValue,
                ValidationRules = request.ValidationRules,
                DisplayOrder = request.DisplayOrder,
                ListOptions = request.ListOptions,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.FolderTypeMetadataFields.Add(field);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata field added to folder type {FolderTypeId}: {FieldName}", folderTypeId, request.FieldName);

            return Result<FolderTypeMetadataFieldDto>.Success(MapFieldToDto(field));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding metadata field to folder type: {FolderTypeId}", folderTypeId);
            return Result<FolderTypeMetadataFieldDto>.Failure($"Failed to add metadata field: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> RemoveMetadataFieldAsync(Guid folderTypeId, Guid fieldId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var field = await _context.FolderTypeMetadataFields
                .FirstOrDefaultAsync(f => f.Id == fieldId && f.FolderTypeId == folderTypeId && !f.IsDeleted, cancellationToken);

            if (field == null)
            {
                return Result.Failure($"Metadata field with ID '{fieldId}' not found");
            }

            field.IsDeleted = true;
            field.DeletedAt = DateTime.UtcNow;
            field.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata field removed from folder type {FolderTypeId}: {FieldId}", folderTypeId, fieldId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing metadata field: {FieldId}", fieldId);
            return Result.Failure($"Failed to remove metadata field: {ex.Message}");
        }
    }

    private static FolderTypeDto MapToDto(FolderType entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        IsActive = entity.IsActive,
        RetentionPeriodMonths = entity.RetentionPeriodMonths,
        DisplayOrder = entity.DisplayOrder,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        MetadataFields = entity.MetadataFields?.Select(MapFieldToDto).ToList() ?? []
    };

    private static FolderTypeMetadataFieldDto MapFieldToDto(FolderTypeMetadataField field) => new()
    {
        Id = field.Id,
        FolderTypeId = field.FolderTypeId,
        FieldName = field.FieldName,
        DisplayName = field.DisplayName,
        DataType = field.DataType,
        IsRequired = field.IsRequired,
        DefaultValue = field.DefaultValue,
        ValidationRules = field.ValidationRules,
        DisplayOrder = field.DisplayOrder,
        ListOptions = field.ListOptions
    };
}
