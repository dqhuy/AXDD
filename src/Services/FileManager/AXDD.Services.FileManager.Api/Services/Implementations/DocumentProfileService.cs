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
/// Service implementation for document profile operations
/// </summary>
public class DocumentProfileService : IDocumentProfileService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<DocumentProfileService> _logger;

    /// <summary>
    /// Initializes a new instance of DocumentProfileService
    /// </summary>
    public DocumentProfileService(
        FileManagerDbContext context,
        ILogger<DocumentProfileService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDto>> CreateProfileAsync(
        CreateDocumentProfileRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name, nameof(request.Name));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code, nameof(request.Code));
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EnterpriseCode, nameof(request.EnterpriseCode));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            // Check if profile with same code exists in enterprise
            var existingProfile = await _context.DocumentProfiles
                .Where(p => p.Code == request.Code
                    && p.EnterpriseCode == request.EnterpriseCode
                    && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingProfile != null)
            {
                return Result<DocumentProfileDto>.Failure($"Profile with code '{request.Code}' already exists");
            }

            // Validate parent profile if specified
            DocumentProfile? parentProfile = null;
            if (request.ParentProfileId.HasValue)
            {
                parentProfile = await _context.DocumentProfiles
                    .Where(p => p.Id == request.ParentProfileId.Value && !p.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (parentProfile == null)
                {
                    return Result<DocumentProfileDto>.Failure($"Parent profile with ID '{request.ParentProfileId}' not found");
                }

                if (parentProfile.EnterpriseCode != request.EnterpriseCode)
                {
                    return Result<DocumentProfileDto>.Failure("Parent profile does not belong to the specified enterprise");
                }
            }

            // Build the full path
            var path = parentProfile != null
                ? $"{parentProfile.Path}/{request.Code}"
                : $"/{request.EnterpriseCode}/{request.Code}";

            var profile = new DocumentProfile
            {
                Name = request.Name,
                Code = request.Code,
                EnterpriseCode = request.EnterpriseCode,
                ProfileType = request.ProfileType,
                Description = request.Description,
                ParentProfileId = request.ParentProfileId,
                Path = path,
                IsTemplate = request.IsTemplate,
                RetentionPeriodMonths = request.RetentionPeriodMonths,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DocumentProfiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);

            // Create metadata fields if provided
            if (request.MetadataFields?.Any() == true)
            {
                foreach (var fieldRequest in request.MetadataFields)
                {
                    var field = new ProfileMetadataField
                    {
                        ProfileId = profile.Id,
                        FieldName = fieldRequest.FieldName,
                        DisplayLabel = fieldRequest.DisplayLabel,
                        DataType = fieldRequest.DataType,
                        IsRequired = fieldRequest.IsRequired,
                        DefaultValue = fieldRequest.DefaultValue,
                        Placeholder = fieldRequest.Placeholder,
                        SelectOptions = fieldRequest.SelectOptions != null
                            ? JsonSerializer.Serialize(fieldRequest.SelectOptions)
                            : null,
                        ValidationPattern = fieldRequest.ValidationPattern,
                        ValidationMessage = fieldRequest.ValidationMessage,
                        MinValue = fieldRequest.MinValue,
                        MaxValue = fieldRequest.MaxValue,
                        MaxLength = fieldRequest.MaxLength,
                        DisplayOrder = fieldRequest.DisplayOrder,
                        IsVisibleInList = fieldRequest.IsVisibleInList,
                        IsSearchable = fieldRequest.IsSearchable,
                        HelpText = fieldRequest.HelpText,
                        IsEnabled = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    };

                    _context.ProfileMetadataFields.Add(field);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Profile created: Id={ProfileId}, Name={Name}, Code={Code}, Enterprise={EnterpriseCode}",
                profile.Id, profile.Name, profile.Code, profile.EnterpriseCode);

            var dto = await MapToDto(profile, true, cancellationToken);
            return Result<DocumentProfileDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile: Name={Name}, Code={Code}", request.Name, request.Code);
            return Result<DocumentProfileDto>.Failure($"Failed to create profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDto>> GetProfileAsync(
        Guid profileId,
        bool includeMetadataFields = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result<DocumentProfileDto>.Failure($"Profile with ID '{profileId}' not found");
            }

            var dto = await MapToDto(profile, includeMetadataFields, cancellationToken);
            return Result<DocumentProfileDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile: ProfileId={ProfileId}", profileId);
            return Result<DocumentProfileDto>.Failure($"Failed to get profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDto>> UpdateProfileAsync(
        Guid profileId,
        UpdateDocumentProfileRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result<DocumentProfileDto>.Failure($"Profile with ID '{profileId}' not found");
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                profile.Name = request.Name;
            }

            if (request.Description != null)
            {
                profile.Description = request.Description;
            }

            if (request.RetentionPeriodMonths.HasValue)
            {
                profile.RetentionPeriodMonths = request.RetentionPeriodMonths.Value;
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                profile.Status = request.Status;
            }

            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = userId;

            _context.DocumentProfiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Profile updated: Id={ProfileId}, Name={Name}",
                profile.Id, profile.Name);

            var dto = await MapToDto(profile, false, cancellationToken);
            return Result<DocumentProfileDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile: ProfileId={ProfileId}", profileId);
            return Result<DocumentProfileDto>.Failure($"Failed to update profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result.Failure($"Profile with ID '{profileId}' not found");
            }

            // Check if profile has documents
            var hasDocuments = await _context.DocumentProfileDocuments
                .AnyAsync(d => d.ProfileId == profileId && !d.IsDeleted, cancellationToken);

            if (hasDocuments)
            {
                return Result.Failure("Cannot delete profile that contains documents");
            }

            // Check if profile has child profiles
            var hasChildren = await _context.DocumentProfiles
                .AnyAsync(p => p.ParentProfileId == profileId && !p.IsDeleted, cancellationToken);

            if (hasChildren)
            {
                return Result.Failure("Cannot delete profile that has child profiles");
            }

            // Soft delete
            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;
            profile.DeletedBy = userId;

            _context.DocumentProfiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile deleted: Id={ProfileId}, Name={Name}", profileId, profile.Name);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile: ProfileId={ProfileId}", profileId);
            return Result.Failure($"Failed to delete profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<DocumentProfileDto>>> ListProfilesAsync(
        DocumentProfileListQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryable = _context.DocumentProfiles
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(query.EnterpriseCode))
            {
                queryable = queryable.Where(p => p.EnterpriseCode == query.EnterpriseCode);
            }

            if (query.ParentProfileId.HasValue)
            {
                queryable = queryable.Where(p => p.ParentProfileId == query.ParentProfileId);
            }
            else if (string.IsNullOrEmpty(query.SearchTerm))
            {
                // Only show root profiles if no search term and no parent specified
                queryable = queryable.Where(p => p.ParentProfileId == null);
            }

            if (!string.IsNullOrEmpty(query.ProfileType))
            {
                queryable = queryable.Where(p => p.ProfileType == query.ProfileType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                queryable = queryable.Where(p => p.Status == query.Status);
            }

            if (query.IsTemplate.HasValue)
            {
                queryable = queryable.Where(p => p.IsTemplate == query.IsTemplate.Value);
            }

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                queryable = queryable.Where(p =>
                    p.Name.Contains(query.SearchTerm) ||
                    p.Code.Contains(query.SearchTerm) ||
                    (p.Description != null && p.Description.Contains(query.SearchTerm)));
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var profiles = await queryable
                .OrderBy(p => p.Name)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = new List<DocumentProfileDto>();
            foreach (var profile in profiles)
            {
                dtos.Add(await MapToDto(profile, false, cancellationToken));
            }

            var pagedResult = new PagedResult<DocumentProfileDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<PagedResult<DocumentProfileDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing profiles");
            return Result<PagedResult<DocumentProfileDto>>.Failure($"Failed to list profiles: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<DocumentProfileDto>>> GetProfileHierarchyAsync(
        string enterpriseCode,
        Guid? rootProfileId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));

        try
        {
            var queryable = _context.DocumentProfiles
                .Where(p => p.EnterpriseCode == enterpriseCode && !p.IsDeleted);

            if (rootProfileId.HasValue)
            {
                queryable = queryable.Where(p => p.ParentProfileId == rootProfileId || p.Id == rootProfileId);
            }
            else
            {
                queryable = queryable.Where(p => p.ParentProfileId == null);
            }

            var profiles = await queryable
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);

            var dtos = new List<DocumentProfileDto>();
            foreach (var profile in profiles)
            {
                var dto = await MapToDto(profile, false, cancellationToken);
                dtos.Add(dto);
            }

            return Result<List<DocumentProfileDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile hierarchy");
            return Result<List<DocumentProfileDto>>.Failure($"Failed to get profile hierarchy: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentProfileDto>> CreateFromTemplateAsync(
        Guid templateId,
        string enterpriseCode,
        string name,
        string code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode, nameof(enterpriseCode));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var template = await _context.DocumentProfiles
                .Include(p => p.MetadataFields.Where(f => !f.IsDeleted))
                .Where(p => p.Id == templateId && p.IsTemplate && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (template == null)
            {
                return Result<DocumentProfileDto>.Failure($"Template with ID '{templateId}' not found");
            }

            // Check if profile with same code exists
            var existingProfile = await _context.DocumentProfiles
                .Where(p => p.Code == code && p.EnterpriseCode == enterpriseCode && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingProfile != null)
            {
                return Result<DocumentProfileDto>.Failure($"Profile with code '{code}' already exists");
            }

            var profile = new DocumentProfile
            {
                Name = name,
                Code = code,
                EnterpriseCode = enterpriseCode,
                ProfileType = template.ProfileType,
                Description = template.Description,
                ParentProfileId = null,
                Path = $"/{enterpriseCode}/{code}",
                IsTemplate = false,
                RetentionPeriodMonths = template.RetentionPeriodMonths,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DocumentProfiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);

            // Copy metadata fields from template
            foreach (var templateField in template.MetadataFields)
            {
                var field = new ProfileMetadataField
                {
                    ProfileId = profile.Id,
                    FieldName = templateField.FieldName,
                    DisplayLabel = templateField.DisplayLabel,
                    DataType = templateField.DataType,
                    IsRequired = templateField.IsRequired,
                    DefaultValue = templateField.DefaultValue,
                    Placeholder = templateField.Placeholder,
                    SelectOptions = templateField.SelectOptions,
                    ValidationPattern = templateField.ValidationPattern,
                    ValidationMessage = templateField.ValidationMessage,
                    MinValue = templateField.MinValue,
                    MaxValue = templateField.MaxValue,
                    MaxLength = templateField.MaxLength,
                    DisplayOrder = templateField.DisplayOrder,
                    IsVisibleInList = templateField.IsVisibleInList,
                    IsSearchable = templateField.IsSearchable,
                    HelpText = templateField.HelpText,
                    IsEnabled = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.ProfileMetadataFields.Add(field);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Profile created from template: Id={ProfileId}, Name={Name}, TemplateId={TemplateId}",
                profile.Id, profile.Name, templateId);

            var dto = await MapToDto(profile, true, cancellationToken);
            return Result<DocumentProfileDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile from template: TemplateId={TemplateId}", templateId);
            return Result<DocumentProfileDto>.Failure($"Failed to create profile from template: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> OpenProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result.Failure($"Profile with ID '{profileId}' not found");
            }

            profile.Status = "Active";
            profile.OpenedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = userId;

            _context.DocumentProfiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile opened: Id={ProfileId}", profileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening profile: ProfileId={ProfileId}", profileId);
            return Result.Failure($"Failed to open profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> CloseProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result.Failure($"Profile with ID '{profileId}' not found");
            }

            profile.Status = "Closed";
            profile.ClosedAt = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = userId;

            _context.DocumentProfiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile closed: Id={ProfileId}", profileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing profile: ProfileId={ProfileId}", profileId);
            return Result.Failure($"Failed to close profile: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result> ArchiveProfileAsync(
        Guid profileId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        try
        {
            var profile = await _context.DocumentProfiles
                .Where(p => p.Id == profileId && !p.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                return Result.Failure($"Profile with ID '{profileId}' not found");
            }

            profile.Status = "Archived";
            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = userId;

            _context.DocumentProfiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Profile archived: Id={ProfileId}", profileId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving profile: ProfileId={ProfileId}", profileId);
            return Result.Failure($"Failed to archive profile: {ex.Message}");
        }
    }

    private async Task<DocumentProfileDto> MapToDto(
        DocumentProfile profile,
        bool includeMetadataFields,
        CancellationToken cancellationToken)
    {
        var documentCount = await _context.DocumentProfileDocuments
            .CountAsync(d => d.ProfileId == profile.Id && !d.IsDeleted, cancellationToken);

        var childProfileCount = await _context.DocumentProfiles
            .CountAsync(p => p.ParentProfileId == profile.Id && !p.IsDeleted, cancellationToken);

        var metadataFieldCount = await _context.ProfileMetadataFields
            .CountAsync(f => f.ProfileId == profile.Id && !f.IsDeleted, cancellationToken);

        string? parentProfileName = null;
        if (profile.ParentProfileId.HasValue)
        {
            parentProfileName = await _context.DocumentProfiles
                .Where(p => p.Id == profile.ParentProfileId.Value)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var dto = new DocumentProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Code = profile.Code,
            EnterpriseCode = profile.EnterpriseCode,
            ProfileType = profile.ProfileType,
            Description = profile.Description,
            ParentProfileId = profile.ParentProfileId,
            ParentProfileName = parentProfileName,
            Path = profile.Path,
            IsTemplate = profile.IsTemplate,
            RetentionPeriodMonths = profile.RetentionPeriodMonths,
            Status = profile.Status,
            OpenedAt = profile.OpenedAt,
            ClosedAt = profile.ClosedAt,
            DocumentCount = documentCount,
            ChildProfileCount = childProfileCount,
            MetadataFieldCount = metadataFieldCount,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            CreatedBy = profile.CreatedBy
        };

        if (includeMetadataFields)
        {
            var fields = await _context.ProfileMetadataFields
                .Where(f => f.ProfileId == profile.Id && !f.IsDeleted)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync(cancellationToken);

            dto.MetadataFields = fields.Select(f => new ProfileMetadataFieldDto
            {
                Id = f.Id,
                ProfileId = f.ProfileId,
                FieldName = f.FieldName,
                DisplayLabel = f.DisplayLabel,
                DataType = f.DataType,
                IsRequired = f.IsRequired,
                DefaultValue = f.DefaultValue,
                Placeholder = f.Placeholder,
                SelectOptions = !string.IsNullOrEmpty(f.SelectOptions)
                    ? JsonSerializer.Deserialize<List<string>>(f.SelectOptions)
                    : null,
                ValidationPattern = f.ValidationPattern,
                ValidationMessage = f.ValidationMessage,
                MinValue = f.MinValue,
                MaxValue = f.MaxValue,
                MaxLength = f.MaxLength,
                DisplayOrder = f.DisplayOrder,
                IsVisibleInList = f.IsVisibleInList,
                IsSearchable = f.IsSearchable,
                IsEnabled = f.IsEnabled,
                HelpText = f.HelpText,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();
        }

        return dto;
    }
}
