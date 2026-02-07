using System.Text.Json;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for profile metadata field operations
/// </summary>
public class ProfileMetadataFieldService : IProfileMetadataFieldService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<ProfileMetadataFieldService> _logger;

    /// <summary>
    /// Initializes a new instance of ProfileMetadataFieldService
    /// </summary>
    public ProfileMetadataFieldService(
        FileManagerDbContext context,
        ILogger<ProfileMetadataFieldService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<ProfileMetadataFieldDto>> CreateFieldAsync(
        CreateProfileMetadataFieldRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FieldName, nameof(request.FieldName));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DisplayLabel, nameof(request.DisplayLabel));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Validate profile exists if ProfileId is provided
            if (request.ProfileId.HasValue)
            {
                var profileExists = await _context.DocumentProfiles
                    .AnyAsync(p => p.Id == request.ProfileId.Value && !p.IsDeleted, cancellationToken);

                if (!profileExists)
                {
                    return Result<ProfileMetadataFieldDto>.Failure($"Profile with ID '{request.ProfileId}' not found");
                }

                // Check if field with same name exists in profile
                var existingField = await _context.ProfileMetadataFields
                    .Where(f => f.ProfileId == request.ProfileId && f.FieldName == request.FieldName && !f.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingField != null)
                {
                    return Result<ProfileMetadataFieldDto>.Failure($"Field with name '{request.FieldName}' already exists in this profile");
                }
            }

            var field = new ProfileMetadataField
            {
                ProfileId = request.ProfileId,
                FieldName = request.FieldName,
                DisplayLabel = request.DisplayLabel,
                DataType = request.DataType,
                IsRequired = request.IsRequired,
                DefaultValue = request.DefaultValue,
                Placeholder = request.Placeholder,
                SelectOptions = request.SelectOptions != null
                    ? JsonSerializer.Serialize(request.SelectOptions)
                    : null,
                ValidationPattern = request.ValidationPattern,
                ValidationMessage = request.ValidationMessage,
                MinValue = request.MinValue,
                MaxValue = request.MaxValue,
                MaxLength = request.MaxLength,
                DisplayOrder = request.DisplayOrder,
                IsVisibleInList = request.IsVisibleInList,
                IsSearchable = request.IsSearchable,
                HelpText = request.HelpText,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.ProfileMetadataFields.Add(field);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Metadata field created: Id={FieldId}, Name={FieldName}, ProfileId={ProfileId}",
                field.Id, field.FieldName, field.ProfileId);

            var dto = MapToDto(field);
            return Result<ProfileMetadataFieldDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating metadata field: FieldName={FieldName}", request.FieldName);
            return Result<ProfileMetadataFieldDto>.Failure($"Failed to create metadata field: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<ProfileMetadataFieldDto>> GetFieldAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var field = await _context.ProfileMetadataFields
                .Where(f => f.Id == fieldId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (field == null)
            {
                return Result<ProfileMetadataFieldDto>.Failure($"Metadata field with ID '{fieldId}' not found");
            }

            var dto = MapToDto(field);
            return Result<ProfileMetadataFieldDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata field: FieldId={FieldId}", fieldId);
            return Result<ProfileMetadataFieldDto>.Failure($"Failed to get metadata field: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<ProfileMetadataFieldDto>> UpdateFieldAsync(
        Guid fieldId,
        UpdateProfileMetadataFieldRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var field = await _context.ProfileMetadataFields
                .Where(f => f.Id == fieldId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (field == null)
            {
                return Result<ProfileMetadataFieldDto>.Failure($"Metadata field with ID '{fieldId}' not found");
            }

            if (!string.IsNullOrEmpty(request.DisplayLabel))
            {
                field.DisplayLabel = request.DisplayLabel;
            }

            if (request.IsRequired.HasValue)
            {
                field.IsRequired = request.IsRequired.Value;
            }

            if (request.DefaultValue != null)
            {
                field.DefaultValue = request.DefaultValue;
            }

            if (request.Placeholder != null)
            {
                field.Placeholder = request.Placeholder;
            }

            if (request.SelectOptions != null)
            {
                field.SelectOptions = JsonSerializer.Serialize(request.SelectOptions);
            }

            if (request.ValidationPattern != null)
            {
                field.ValidationPattern = request.ValidationPattern;
            }

            if (request.ValidationMessage != null)
            {
                field.ValidationMessage = request.ValidationMessage;
            }

            if (request.MinValue.HasValue)
            {
                field.MinValue = request.MinValue;
            }

            if (request.MaxValue.HasValue)
            {
                field.MaxValue = request.MaxValue;
            }

            if (request.MaxLength.HasValue)
            {
                field.MaxLength = request.MaxLength;
            }

            if (request.DisplayOrder.HasValue)
            {
                field.DisplayOrder = request.DisplayOrder.Value;
            }

            if (request.IsVisibleInList.HasValue)
            {
                field.IsVisibleInList = request.IsVisibleInList.Value;
            }

            if (request.IsSearchable.HasValue)
            {
                field.IsSearchable = request.IsSearchable.Value;
            }

            if (request.IsEnabled.HasValue)
            {
                field.IsEnabled = request.IsEnabled.Value;
            }

            if (request.HelpText != null)
            {
                field.HelpText = request.HelpText;
            }

            field.UpdatedAt = DateTime.UtcNow;
            field.UpdatedBy = userId;

            _context.ProfileMetadataFields.Update(field);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata field updated: Id={FieldId}, Name={FieldName}", fieldId, field.FieldName);

            var dto = MapToDto(field);
            return Result<ProfileMetadataFieldDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating metadata field: FieldId={FieldId}", fieldId);
            return Result<ProfileMetadataFieldDto>.Failure($"Failed to update metadata field: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteFieldAsync(
        Guid fieldId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var field = await _context.ProfileMetadataFields
                .Where(f => f.Id == fieldId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (field == null)
            {
                return Result.Failure($"Metadata field with ID '{fieldId}' not found");
            }

            // Check if field has values
            var hasValues = await _context.DocumentMetadataValues
                .AnyAsync(v => v.MetadataFieldId == fieldId && !v.IsDeleted, cancellationToken);

            if (hasValues)
            {
                return Result.Failure("Cannot delete metadata field that has values assigned to documents");
            }

            // Soft delete
            field.IsDeleted = true;
            field.DeletedAt = DateTime.UtcNow;
            field.DeletedBy = userId;

            _context.ProfileMetadataFields.Update(field);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata field deleted: Id={FieldId}, Name={FieldName}", fieldId, field.FieldName);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata field: FieldId={FieldId}", fieldId);
            return Result.Failure($"Failed to delete metadata field: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<ProfileMetadataFieldDto>>> ListFieldsAsync(
        Guid profileId,
        bool includeDisabled = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.ProfileMetadataFields
                .Where(f => f.ProfileId == profileId && !f.IsDeleted);

            if (!includeDisabled)
            {
                query = query.Where(f => f.IsEnabled);
            }

            var fields = await query
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync(cancellationToken);

            var dtos = fields.Select(MapToDto).ToList();

            return Result<List<ProfileMetadataFieldDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing metadata fields: ProfileId={ProfileId}", profileId);
            return Result<List<ProfileMetadataFieldDto>>.Failure($"Failed to list metadata fields: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> ReorderFieldsAsync(
        Guid profileId,
        Dictionary<Guid, int> fieldOrders,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var fields = await _context.ProfileMetadataFields
                .Where(f => f.ProfileId == profileId && !f.IsDeleted && fieldOrders.Keys.Contains(f.Id))
                .ToListAsync(cancellationToken);

            foreach (var field in fields)
            {
                if (fieldOrders.TryGetValue(field.Id, out var order))
                {
                    field.DisplayOrder = order;
                    field.UpdatedAt = DateTime.UtcNow;
                    field.UpdatedBy = userId;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Metadata fields reordered: ProfileId={ProfileId}", profileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering metadata fields: ProfileId={ProfileId}", profileId);
            return Result.Failure($"Failed to reorder metadata fields: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<ProfileMetadataFieldDto>>> CopyFieldsAsync(
        Guid sourceProfileId,
        Guid targetProfileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Validate target profile exists
            var targetProfileExists = await _context.DocumentProfiles
                .AnyAsync(p => p.Id == targetProfileId && !p.IsDeleted, cancellationToken);

            if (!targetProfileExists)
            {
                return Result<List<ProfileMetadataFieldDto>>.Failure($"Target profile with ID '{targetProfileId}' not found");
            }

            // Get source fields
            var sourceFields = await _context.ProfileMetadataFields
                .Where(f => f.ProfileId == sourceProfileId && !f.IsDeleted)
                .ToListAsync(cancellationToken);

            // Get existing field names in target
            var existingFieldNames = await _context.ProfileMetadataFields
                .Where(f => f.ProfileId == targetProfileId && !f.IsDeleted)
                .Select(f => f.FieldName)
                .ToListAsync(cancellationToken);

            var copiedFields = new List<ProfileMetadataField>();

            foreach (var sourceField in sourceFields)
            {
                // Skip if field already exists in target
                if (existingFieldNames.Contains(sourceField.FieldName))
                {
                    continue;
                }

                var newField = new ProfileMetadataField
                {
                    ProfileId = targetProfileId,
                    FieldName = sourceField.FieldName,
                    DisplayLabel = sourceField.DisplayLabel,
                    DataType = sourceField.DataType,
                    IsRequired = sourceField.IsRequired,
                    DefaultValue = sourceField.DefaultValue,
                    Placeholder = sourceField.Placeholder,
                    SelectOptions = sourceField.SelectOptions,
                    ValidationPattern = sourceField.ValidationPattern,
                    ValidationMessage = sourceField.ValidationMessage,
                    MinValue = sourceField.MinValue,
                    MaxValue = sourceField.MaxValue,
                    MaxLength = sourceField.MaxLength,
                    DisplayOrder = sourceField.DisplayOrder,
                    IsVisibleInList = sourceField.IsVisibleInList,
                    IsSearchable = sourceField.IsSearchable,
                    HelpText = sourceField.HelpText,
                    IsEnabled = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.ProfileMetadataFields.Add(newField);
                copiedFields.Add(newField);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Metadata fields copied: From={SourceProfileId}, To={TargetProfileId}, Count={Count}",
                sourceProfileId, targetProfileId, copiedFields.Count);

            var dtos = copiedFields.Select(MapToDto).ToList();

            return Result<List<ProfileMetadataFieldDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying metadata fields: Source={SourceProfileId}, Target={TargetProfileId}",
                sourceProfileId, targetProfileId);
            return Result<List<ProfileMetadataFieldDto>>.Failure($"Failed to copy metadata fields: {ex.Message}");
        }
    }

    private static ProfileMetadataFieldDto MapToDto(ProfileMetadataField field)
    {
        return new ProfileMetadataFieldDto
        {
            Id = field.Id,
            ProfileId = field.ProfileId,
            FieldName = field.FieldName,
            DisplayLabel = field.DisplayLabel,
            DataType = field.DataType,
            IsRequired = field.IsRequired,
            DefaultValue = field.DefaultValue,
            Placeholder = field.Placeholder,
            SelectOptions = !string.IsNullOrEmpty(field.SelectOptions)
                ? JsonSerializer.Deserialize<List<string>>(field.SelectOptions)
                : null,
            ValidationPattern = field.ValidationPattern,
            ValidationMessage = field.ValidationMessage,
            MinValue = field.MinValue,
            MaxValue = field.MaxValue,
            MaxLength = field.MaxLength,
            DisplayOrder = field.DisplayOrder,
            IsVisibleInList = field.IsVisibleInList,
            IsSearchable = field.IsSearchable,
            IsEnabled = field.IsEnabled,
            HelpText = field.HelpText,
            CreatedAt = field.CreatedAt,
            UpdatedAt = field.UpdatedAt
        };
    }
}
