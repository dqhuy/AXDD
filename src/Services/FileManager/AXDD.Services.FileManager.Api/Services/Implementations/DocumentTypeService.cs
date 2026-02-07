using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for document type operations
/// </summary>
public class DocumentTypeService : IDocumentTypeService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<DocumentTypeService> _logger;

    public DocumentTypeService(FileManagerDbContext context, ILogger<DocumentTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentTypeDto>> CreateAsync(CreateDocumentTypeRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        try
        {
            // Check if code already exists
            var existingCode = await _context.DocumentTypes
                .AnyAsync(d => d.Code == request.Code && !d.IsDeleted, cancellationToken);

            if (existingCode)
            {
                return Result<DocumentTypeDto>.Failure($"Document type with code '{request.Code}' already exists");
            }

            var entity = new DocumentType
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DocumentTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document type created: {Code}", request.Code);

            return Result<DocumentTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document type: {Code}", request.Code);
            return Result<DocumentTypeDto>.Failure($"Failed to create document type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.DocumentTypes
                .Include(d => d.MetadataFields.Where(f => !f.IsDeleted))
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<DocumentTypeDto>.Failure($"Document type with ID '{id}' not found");
            }

            return Result<DocumentTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document type: {Id}", id);
            return Result<DocumentTypeDto>.Failure($"Failed to get document type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentTypeDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        try
        {
            var entity = await _context.DocumentTypes
                .Include(d => d.MetadataFields.Where(f => !f.IsDeleted))
                .FirstOrDefaultAsync(d => d.Code == code && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<DocumentTypeDto>.Failure($"Document type with code '{code}' not found");
            }

            return Result<DocumentTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document type by code: {Code}", code);
            return Result<DocumentTypeDto>.Failure($"Failed to get document type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<DocumentTypeDto>>> ListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.DocumentTypes
                .Include(d => d.MetadataFields.Where(f => !f.IsDeleted))
                .Where(d => !d.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.Name.Contains(searchTerm) || d.Code.Contains(searchTerm));
            }

            if (isActive.HasValue)
            {
                query = query.Where(d => d.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(d => d.DisplayOrder)
                .ThenBy(d => d.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<DocumentTypeDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<DocumentTypeDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing document types");
            return Result<PagedResult<DocumentTypeDto>>.Failure($"Failed to list document types: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentTypeDto>> UpdateAsync(Guid id, CreateDocumentTypeRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var entity = await _context.DocumentTypes
                .Include(d => d.MetadataFields.Where(f => !f.IsDeleted))
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result<DocumentTypeDto>.Failure($"Document type with ID '{id}' not found");
            }

            // Check if new code conflicts with existing
            if (entity.Code != request.Code)
            {
                var codeExists = await _context.DocumentTypes
                    .AnyAsync(d => d.Code == request.Code && d.Id != id && !d.IsDeleted, cancellationToken);

                if (codeExists)
                {
                    return Result<DocumentTypeDto>.Failure($"Document type with code '{request.Code}' already exists");
                }
            }

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.DisplayOrder = request.DisplayOrder;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document type updated: {Id}", id);

            return Result<DocumentTypeDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document type: {Id}", id);
            return Result<DocumentTypeDto>.Failure($"Failed to update document type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.DocumentTypes
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

            if (entity == null)
            {
                return Result.Failure($"Document type with ID '{id}' not found");
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document type deleted: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document type: {Id}", id);
            return Result.Failure($"Failed to delete document type: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentTypeMetadataFieldDto>> AddMetadataFieldAsync(Guid documentTypeId, CreateDocumentTypeMetadataFieldRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var documentType = await _context.DocumentTypes
                .FirstOrDefaultAsync(d => d.Id == documentTypeId && !d.IsDeleted, cancellationToken);

            if (documentType == null)
            {
                return Result<DocumentTypeMetadataFieldDto>.Failure($"Document type with ID '{documentTypeId}' not found");
            }

            // Check if field name already exists
            var fieldExists = await _context.DocumentTypeMetadataFields
                .AnyAsync(f => f.DocumentTypeId == documentTypeId && f.FieldName == request.FieldName && !f.IsDeleted, cancellationToken);

            if (fieldExists)
            {
                return Result<DocumentTypeMetadataFieldDto>.Failure($"Field '{request.FieldName}' already exists for this document type");
            }

            var field = new DocumentTypeMetadataField
            {
                DocumentTypeId = documentTypeId,
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

            _context.DocumentTypeMetadataFields.Add(field);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata field added to document type {DocumentTypeId}: {FieldName}", documentTypeId, request.FieldName);

            return Result<DocumentTypeMetadataFieldDto>.Success(MapFieldToDto(field));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding metadata field to document type: {DocumentTypeId}", documentTypeId);
            return Result<DocumentTypeMetadataFieldDto>.Failure($"Failed to add metadata field: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> RemoveMetadataFieldAsync(Guid documentTypeId, Guid fieldId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var field = await _context.DocumentTypeMetadataFields
                .FirstOrDefaultAsync(f => f.Id == fieldId && f.DocumentTypeId == documentTypeId && !f.IsDeleted, cancellationToken);

            if (field == null)
            {
                return Result.Failure($"Metadata field with ID '{fieldId}' not found");
            }

            field.IsDeleted = true;
            field.DeletedAt = DateTime.UtcNow;
            field.DeletedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata field removed from document type {DocumentTypeId}: {FieldId}", documentTypeId, fieldId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing metadata field: {FieldId}", fieldId);
            return Result.Failure($"Failed to remove metadata field: {ex.Message}");
        }
    }

    private static DocumentTypeDto MapToDto(DocumentType entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        IsActive = entity.IsActive,
        DisplayOrder = entity.DisplayOrder,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        MetadataFields = entity.MetadataFields?.Select(MapFieldToDto).ToList() ?? []
    };

    private static DocumentTypeMetadataFieldDto MapFieldToDto(DocumentTypeMetadataField field) => new()
    {
        Id = field.Id,
        DocumentTypeId = field.DocumentTypeId,
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
