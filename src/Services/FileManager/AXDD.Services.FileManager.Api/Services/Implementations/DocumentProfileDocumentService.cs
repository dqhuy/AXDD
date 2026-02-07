using System.Text.Json;
using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for document profile document operations
/// </summary>
public class DocumentProfileDocumentService : IDocumentProfileDocumentService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<DocumentProfileDocumentService> _logger;

    /// <summary>
    /// Initializes a new instance of DocumentProfileDocumentService
    /// </summary>
    public DocumentProfileDocumentService(
        FileManagerDbContext context,
        ILogger<DocumentProfileDocumentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDocumentDto>> AddDocumentAsync(
        AddDocumentToProfileRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title, nameof(request.Title));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Validate profile exists
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == request.ProfileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result<DocumentProfileDocumentDto>.Failure($"Profile with ID '{request.ProfileId}' not found");
            }

            // Validate file metadata exists
            var fileMetadata = await _context.FileMetadata
                .Where(f => f.Id == request.FileMetadataId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileMetadata == null)
            {
                return Result<DocumentProfileDocumentDto>.Failure($"File with ID '{request.FileMetadataId}' not found");
            }

            // Check if document already exists in profile
            var existingDoc = await _context.DocumentProfileDocuments
                .Where(d => d.ProfileId == request.ProfileId && d.FileMetadataId == request.FileMetadataId && !d.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingDoc != null)
            {
                return Result<DocumentProfileDocumentDto>.Failure("This file is already added to the profile");
            }

            // Get next display order
            var maxOrder = await _context.DocumentProfileDocuments
                .Where(d => d.ProfileId == request.ProfileId && !d.IsDeleted)
                .MaxAsync(d => (int?)d.DisplayOrder, cancellationToken) ?? 0;

            var document = new DocumentProfileDocument
            {
                ProfileId = request.ProfileId,
                FileMetadataId = request.FileMetadataId,
                Title = request.Title,
                Description = request.Description,
                DocumentType = request.DocumentType,
                DocumentNumber = request.DocumentNumber,
                IssueDate = request.IssueDate,
                ExpiryDate = request.ExpiryDate,
                IssuingAuthority = request.IssuingAuthority,
                Notes = request.Notes,
                Status = "Active",
                DisplayOrder = maxOrder + 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DocumentProfileDocuments.Add(document);
            await _context.SaveChangesAsync(cancellationToken);

            // Add metadata values if provided
            if (request.MetadataValues?.Any() == true)
            {
                await SetMetadataValuesInternalAsync(document.Id, request.MetadataValues, userId, cancellationToken);
            }

            _logger.LogInformation(
                "Document added to profile: DocId={DocumentId}, ProfileId={ProfileId}, FileId={FileId}",
                document.Id, request.ProfileId, request.FileMetadataId);

            var dto = await MapToDto(document, true, cancellationToken);
            return Result<DocumentProfileDocumentDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document to profile: ProfileId={ProfileId}", request.ProfileId);
            return Result<DocumentProfileDocumentDto>.Failure($"Failed to add document to profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDocumentDto>> GetDocumentAsync(
        Guid documentId,
        bool includeMetadata = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _context.DocumentProfileDocuments
                .Include(d => d.FileMetadata)
                .Where(d => d.Id == documentId && !d.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return Result<DocumentProfileDocumentDto>.Failure($"Document with ID '{documentId}' not found");
            }

            var dto = await MapToDto(document, includeMetadata, cancellationToken);
            return Result<DocumentProfileDocumentDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document: DocumentId={DocumentId}", documentId);
            return Result<DocumentProfileDocumentDto>.Failure($"Failed to get document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDocumentDto>> UpdateDocumentAsync(
        Guid documentId,
        UpdateDocumentProfileDocumentRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var document = await _context.DocumentProfileDocuments
                .Where(d => d.Id == documentId && !d.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return Result<DocumentProfileDocumentDto>.Failure($"Document with ID '{documentId}' not found");
            }

            if (!string.IsNullOrEmpty(request.Title))
            {
                document.Title = request.Title;
            }

            if (request.Description != null)
            {
                document.Description = request.Description;
            }

            if (!string.IsNullOrEmpty(request.DocumentType))
            {
                document.DocumentType = request.DocumentType;
            }

            if (request.DocumentNumber != null)
            {
                document.DocumentNumber = request.DocumentNumber;
            }

            if (request.IssueDate.HasValue)
            {
                document.IssueDate = request.IssueDate;
            }

            if (request.ExpiryDate.HasValue)
            {
                document.ExpiryDate = request.ExpiryDate;
            }

            if (request.IssuingAuthority != null)
            {
                document.IssuingAuthority = request.IssuingAuthority;
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                document.Status = request.Status;
            }

            if (request.DisplayOrder.HasValue)
            {
                document.DisplayOrder = request.DisplayOrder.Value;
            }

            if (request.Notes != null)
            {
                document.Notes = request.Notes;
            }

            document.UpdatedAt = DateTime.UtcNow;
            document.UpdatedBy = userId;

            _context.DocumentProfileDocuments.Update(document);
            await _context.SaveChangesAsync(cancellationToken);

            // Update metadata values if provided
            if (request.MetadataValues?.Any() == true)
            {
                await SetMetadataValuesInternalAsync(documentId, request.MetadataValues, userId, cancellationToken);
            }

            _logger.LogInformation("Document updated: DocumentId={DocumentId}", documentId);

            var dto = await MapToDto(document, true, cancellationToken);
            return Result<DocumentProfileDocumentDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document: DocumentId={DocumentId}", documentId);
            return Result<DocumentProfileDocumentDto>.Failure($"Failed to update document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> RemoveDocumentAsync(
        Guid documentId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var document = await _context.DocumentProfileDocuments
                .Where(d => d.Id == documentId && !d.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return Result.Failure($"Document with ID '{documentId}' not found");
            }

            // Soft delete
            document.IsDeleted = true;
            document.DeletedAt = DateTime.UtcNow;
            document.DeletedBy = userId;

            _context.DocumentProfileDocuments.Update(document);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document removed from profile: DocumentId={DocumentId}", documentId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing document: DocumentId={DocumentId}", documentId);
            return Result.Failure($"Failed to remove document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<DocumentProfileDocumentDto>>> ListDocumentsAsync(
        DocumentProfileDocumentListQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryable = _context.DocumentProfileDocuments
                .Include(d => d.FileMetadata)
                .Where(d => !d.IsDeleted);

            if (query.ProfileId.HasValue)
            {
                queryable = queryable.Where(d => d.ProfileId == query.ProfileId);
            }

            if (!string.IsNullOrEmpty(query.DocumentType))
            {
                queryable = queryable.Where(d => d.DocumentType == query.DocumentType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                queryable = queryable.Where(d => d.Status == query.Status);
            }

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                queryable = queryable.Where(d =>
                    d.Title.Contains(query.SearchTerm) ||
                    (d.DocumentNumber != null && d.DocumentNumber.Contains(query.SearchTerm)) ||
                    (d.Description != null && d.Description.Contains(query.SearchTerm)));
            }

            if (query.IssueDateFrom.HasValue)
            {
                queryable = queryable.Where(d => d.IssueDate >= query.IssueDateFrom);
            }

            if (query.IssueDateTo.HasValue)
            {
                queryable = queryable.Where(d => d.IssueDate <= query.IssueDateTo);
            }

            if (query.ExpiryDateFrom.HasValue)
            {
                queryable = queryable.Where(d => d.ExpiryDate >= query.ExpiryDateFrom);
            }

            if (query.ExpiryDateTo.HasValue)
            {
                queryable = queryable.Where(d => d.ExpiryDate <= query.ExpiryDateTo);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var documents = await queryable
                .OrderBy(d => d.DisplayOrder)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = new List<DocumentProfileDocumentDto>();
            foreach (var document in documents)
            {
                dtos.Add(await MapToDto(document, false, cancellationToken));
            }

            var pagedResult = new PagedResult<DocumentProfileDocumentDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<PagedResult<DocumentProfileDocumentDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing documents");
            return Result<PagedResult<DocumentProfileDocumentDto>>.Failure($"Failed to list documents: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<DocumentMetadataValueDto>>> SetMetadataValuesAsync(
        Guid documentId,
        List<SetMetadataValueRequest> values,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Validate document exists
            var documentExists = await _context.DocumentProfileDocuments
                .AnyAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

            if (!documentExists)
            {
                return Result<List<DocumentMetadataValueDto>>.Failure($"Document with ID '{documentId}' not found");
            }

            var result = await SetMetadataValuesInternalAsync(documentId, values, userId, cancellationToken);
            return Result<List<DocumentMetadataValueDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting metadata values: DocumentId={DocumentId}", documentId);
            return Result<List<DocumentMetadataValueDto>>.Failure($"Failed to set metadata values: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<DocumentMetadataValueDto>>> GetMetadataValuesAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var values = await _context.DocumentMetadataValues
                .Include(v => v.MetadataField)
                .Where(v => v.DocumentId == documentId && !v.IsDeleted)
                .ToListAsync(cancellationToken);

            var dtos = values.Select(MapMetadataValueToDto).ToList();

            return Result<List<DocumentMetadataValueDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata values: DocumentId={DocumentId}", documentId);
            return Result<List<DocumentMetadataValueDto>>.Failure($"Failed to get metadata values: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> MoveDocumentAsync(
        Guid documentId,
        Guid targetProfileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var document = await _context.DocumentProfileDocuments
                .Where(d => d.Id == documentId && !d.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return Result.Failure($"Document with ID '{documentId}' not found");
            }

            // Validate target profile exists
            var targetProfileExists = await _context.DocumentProfiles
                .AnyAsync(p => p.Id == targetProfileId && !p.IsDeleted, cancellationToken);

            if (!targetProfileExists)
            {
                return Result.Failure($"Target profile with ID '{targetProfileId}' not found");
            }

            // Get next display order in target profile
            var maxOrder = await _context.DocumentProfileDocuments
                .Where(d => d.ProfileId == targetProfileId && !d.IsDeleted)
                .MaxAsync(d => (int?)d.DisplayOrder, cancellationToken) ?? 0;

            document.ProfileId = targetProfileId;
            document.DisplayOrder = maxOrder + 1;
            document.UpdatedAt = DateTime.UtcNow;
            document.UpdatedBy = userId;

            _context.DocumentProfileDocuments.Update(document);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Document moved: DocumentId={DocumentId}, TargetProfileId={TargetProfileId}",
                documentId, targetProfileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving document: DocumentId={DocumentId}", documentId);
            return Result.Failure($"Failed to move document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDocumentDto>> CopyDocumentAsync(
        Guid documentId,
        Guid targetProfileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var document = await _context.DocumentProfileDocuments
                .Include(d => d.MetadataValues.Where(v => !v.IsDeleted))
                .Where(d => d.Id == documentId && !d.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return Result<DocumentProfileDocumentDto>.Failure($"Document with ID '{documentId}' not found");
            }

            // Validate target profile exists
            var targetProfileExists = await _context.DocumentProfiles
                .AnyAsync(p => p.Id == targetProfileId && !p.IsDeleted, cancellationToken);

            if (!targetProfileExists)
            {
                return Result<DocumentProfileDocumentDto>.Failure($"Target profile with ID '{targetProfileId}' not found");
            }

            // Get next display order in target profile
            var maxOrder = await _context.DocumentProfileDocuments
                .Where(d => d.ProfileId == targetProfileId && !d.IsDeleted)
                .MaxAsync(d => (int?)d.DisplayOrder, cancellationToken) ?? 0;

            var newDocument = new DocumentProfileDocument
            {
                ProfileId = targetProfileId,
                FileMetadataId = document.FileMetadataId,
                Title = document.Title,
                Description = document.Description,
                DocumentType = document.DocumentType,
                DocumentNumber = document.DocumentNumber,
                IssueDate = document.IssueDate,
                ExpiryDate = document.ExpiryDate,
                IssuingAuthority = document.IssuingAuthority,
                Notes = document.Notes,
                Status = document.Status,
                DisplayOrder = maxOrder + 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DocumentProfileDocuments.Add(newDocument);
            await _context.SaveChangesAsync(cancellationToken);

            // Copy metadata values
            foreach (var value in document.MetadataValues)
            {
                var newValue = new DocumentMetadataValue
                {
                    DocumentId = newDocument.Id,
                    MetadataFieldId = value.MetadataFieldId,
                    StringValue = value.StringValue,
                    NumberValue = value.NumberValue,
                    DateValue = value.DateValue,
                    BooleanValue = value.BooleanValue,
                    JsonValue = value.JsonValue,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.DocumentMetadataValues.Add(newValue);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Document copied: SourceId={SourceId}, NewId={NewId}, TargetProfileId={TargetProfileId}",
                documentId, newDocument.Id, targetProfileId);

            var dto = await MapToDto(newDocument, true, cancellationToken);
            return Result<DocumentProfileDocumentDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying document: DocumentId={DocumentId}", documentId);
            return Result<DocumentProfileDocumentDto>.Failure($"Failed to copy document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> ReorderDocumentsAsync(
        Guid profileId,
        Dictionary<Guid, int> documentOrders,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var documents = await _context.DocumentProfileDocuments
                .Where(d => d.ProfileId == profileId && !d.IsDeleted && documentOrders.Keys.Contains(d.Id))
                .ToListAsync(cancellationToken);

            foreach (var document in documents)
            {
                if (documentOrders.TryGetValue(document.Id, out var order))
                {
                    document.DisplayOrder = order;
                    document.UpdatedAt = DateTime.UtcNow;
                    document.UpdatedBy = userId;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Documents reordered: ProfileId={ProfileId}", profileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering documents: ProfileId={ProfileId}", profileId);
            return Result.Failure($"Failed to reorder documents: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<DocumentProfileDocumentDto>>> GetExpiringDocumentsAsync(
        string enterpriseCode,
        int daysAhead = 30,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysAhead);

            var documents = await _context.DocumentProfileDocuments
                .Include(d => d.FileMetadata)
                .Include(d => d.Profile)
                .Where(d => !d.IsDeleted &&
                    d.Profile.EnterpriseCode == enterpriseCode &&
                    d.ExpiryDate.HasValue &&
                    d.ExpiryDate <= expiryDate &&
                    d.Status == "Active")
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync(cancellationToken);

            var dtos = new List<DocumentProfileDocumentDto>();
            foreach (var document in documents)
            {
                dtos.Add(await MapToDto(document, false, cancellationToken));
            }

            return Result<List<DocumentProfileDocumentDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expiring documents: EnterpriseCode={EnterpriseCode}", enterpriseCode);
            return Result<List<DocumentProfileDocumentDto>>.Failure($"Failed to get expiring documents: {ex.Message}");
        }
    }

    private async Task<List<DocumentMetadataValueDto>> SetMetadataValuesInternalAsync(
        Guid documentId,
        List<SetMetadataValueRequest> values,
        string userId,
        CancellationToken cancellationToken)
    {
        var result = new List<DocumentMetadataValueDto>();

        foreach (var valueRequest in values)
        {
            // Find existing value
            var existingValue = await _context.DocumentMetadataValues
                .Include(v => v.MetadataField)
                .Where(v => v.DocumentId == documentId && v.MetadataFieldId == valueRequest.MetadataFieldId && !v.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingValue != null)
            {
                // Update existing value
                existingValue.StringValue = valueRequest.StringValue;
                existingValue.NumberValue = valueRequest.NumberValue;
                existingValue.DateValue = valueRequest.DateValue;
                existingValue.BooleanValue = valueRequest.BooleanValue;
                existingValue.JsonValue = valueRequest.JsonValue != null
                    ? JsonSerializer.Serialize(valueRequest.JsonValue)
                    : null;
                existingValue.UpdatedAt = DateTime.UtcNow;
                existingValue.UpdatedBy = userId;

                _context.DocumentMetadataValues.Update(existingValue);
                result.Add(MapMetadataValueToDto(existingValue));
            }
            else
            {
                // Get field information
                var field = await _context.ProfileMetadataFields
                    .Where(f => f.Id == valueRequest.MetadataFieldId && !f.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (field == null)
                {
                    continue; // Skip invalid field IDs
                }

                // Create new value
                var newValue = new DocumentMetadataValue
                {
                    DocumentId = documentId,
                    MetadataFieldId = valueRequest.MetadataFieldId,
                    StringValue = valueRequest.StringValue,
                    NumberValue = valueRequest.NumberValue,
                    DateValue = valueRequest.DateValue,
                    BooleanValue = valueRequest.BooleanValue,
                    JsonValue = valueRequest.JsonValue != null
                        ? JsonSerializer.Serialize(valueRequest.JsonValue)
                        : null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.DocumentMetadataValues.Add(newValue);

                // Need to save to get ID
                await _context.SaveChangesAsync(cancellationToken);

                // Reload with field
                newValue = await _context.DocumentMetadataValues
                    .Include(v => v.MetadataField)
                    .FirstAsync(v => v.Id == newValue.Id, cancellationToken);

                result.Add(MapMetadataValueToDto(newValue));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }

    private async Task<DocumentProfileDocumentDto> MapToDto(
        DocumentProfileDocument document,
        bool includeMetadata,
        CancellationToken cancellationToken)
    {
        var profileName = await _context.DocumentProfiles
            .Where(p => p.Id == document.ProfileId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var fileMetadata = document.FileMetadata ?? await _context.FileMetadata
            .Where(f => f.Id == document.FileMetadataId)
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new DocumentProfileDocumentDto
        {
            Id = document.Id,
            ProfileId = document.ProfileId,
            ProfileName = profileName,
            FileMetadataId = document.FileMetadataId,
            FileName = fileMetadata?.FileName ?? string.Empty,
            FileSize = fileMetadata?.FileSize ?? 0,
            MimeType = fileMetadata?.MimeType ?? string.Empty,
            Title = document.Title,
            Description = document.Description,
            DocumentType = document.DocumentType,
            DocumentNumber = document.DocumentNumber,
            IssueDate = document.IssueDate,
            ExpiryDate = document.ExpiryDate,
            IssuingAuthority = document.IssuingAuthority,
            Status = document.Status,
            DisplayOrder = document.DisplayOrder,
            Notes = document.Notes,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            CreatedBy = document.CreatedBy
        };

        if (includeMetadata)
        {
            var values = await _context.DocumentMetadataValues
                .Include(v => v.MetadataField)
                .Where(v => v.DocumentId == document.Id && !v.IsDeleted)
                .ToListAsync(cancellationToken);

            dto.MetadataValues = values.Select(MapMetadataValueToDto).ToList();
        }

        return dto;
    }

    private static DocumentMetadataValueDto MapMetadataValueToDto(DocumentMetadataValue value)
    {
        object? displayValue = value.MetadataField.DataType switch
        {
            "String" or "Select" => value.StringValue,
            "Number" => value.NumberValue,
            "Date" => value.DateValue,
            "Boolean" => value.BooleanValue,
            "MultiSelect" => !string.IsNullOrEmpty(value.JsonValue)
                ? JsonSerializer.Deserialize<object>(value.JsonValue)
                : null,
            _ => value.StringValue ?? value.JsonValue
        };

        return new DocumentMetadataValueDto
        {
            Id = value.Id,
            DocumentId = value.DocumentId,
            MetadataFieldId = value.MetadataFieldId,
            FieldName = value.MetadataField.FieldName,
            DisplayLabel = value.MetadataField.DisplayLabel,
            DataType = value.MetadataField.DataType,
            StringValue = value.StringValue,
            NumberValue = value.NumberValue,
            DateValue = value.DateValue,
            BooleanValue = value.BooleanValue,
            JsonValue = !string.IsNullOrEmpty(value.JsonValue)
                ? JsonSerializer.Deserialize<object>(value.JsonValue)
                : null,
            DisplayValue = displayValue
        };
    }
}
