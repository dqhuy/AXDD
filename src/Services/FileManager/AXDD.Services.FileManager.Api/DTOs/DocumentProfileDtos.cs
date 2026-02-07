namespace AXDD.Services.FileManager.Api.DTOs;

#region Document Profile DTOs

/// <summary>
/// DTO for document profile response
/// </summary>
public class DocumentProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string EnterpriseCode { get; set; } = string.Empty;
    public string ProfileType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentProfileId { get; set; }
    public string? ParentProfileName { get; set; }
    public string Path { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
    public int RetentionPeriodMonths { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int DocumentCount { get; set; }
    public int ChildProfileCount { get; set; }
    public int MetadataFieldCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<ProfileMetadataFieldDto> MetadataFields { get; set; } = new();
}

/// <summary>
/// DTO for creating a document profile
/// </summary>
public class CreateDocumentProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string EnterpriseCode { get; set; } = string.Empty;
    public string ProfileType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentProfileId { get; set; }
    public bool IsTemplate { get; set; }
    public int RetentionPeriodMonths { get; set; }
    public List<CreateProfileMetadataFieldRequest>? MetadataFields { get; set; }
}

/// <summary>
/// DTO for updating a document profile
/// </summary>
public class UpdateDocumentProfileRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? RetentionPeriodMonths { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// DTO for document profile list query
/// </summary>
public class DocumentProfileListQuery
{
    public string? EnterpriseCode { get; set; }
    public Guid? ParentProfileId { get; set; }
    public string? ProfileType { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsTemplate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

#endregion

#region Profile Metadata Field DTOs

/// <summary>
/// DTO for profile metadata field response
/// </summary>
public class ProfileMetadataFieldDto
{
    public Guid Id { get; set; }
    public Guid? ProfileId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? Placeholder { get; set; }
    public List<string>? SelectOptions { get; set; }
    public string? ValidationPattern { get; set; }
    public string? ValidationMessage { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public int? MaxLength { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisibleInList { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsEnabled { get; set; }
    public string? HelpText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a profile metadata field
/// </summary>
public class CreateProfileMetadataFieldRequest
{
    public Guid? ProfileId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public string DataType { get; set; } = "String";
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? Placeholder { get; set; }
    public List<string>? SelectOptions { get; set; }
    public string? ValidationPattern { get; set; }
    public string? ValidationMessage { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public int? MaxLength { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisibleInList { get; set; } = true;
    public bool IsSearchable { get; set; }
    public string? HelpText { get; set; }
}

/// <summary>
/// DTO for updating a profile metadata field
/// </summary>
public class UpdateProfileMetadataFieldRequest
{
    public string? DisplayLabel { get; set; }
    public bool? IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? Placeholder { get; set; }
    public List<string>? SelectOptions { get; set; }
    public string? ValidationPattern { get; set; }
    public string? ValidationMessage { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public int? MaxLength { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsVisibleInList { get; set; }
    public bool? IsSearchable { get; set; }
    public bool? IsEnabled { get; set; }
    public string? HelpText { get; set; }
}

#endregion

#region Document Profile Document DTOs

/// <summary>
/// DTO for document profile document response
/// </summary>
public class DocumentProfileDocumentDto
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public Guid FileMetadataId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<DocumentMetadataValueDto> MetadataValues { get; set; } = new();
}

/// <summary>
/// DTO for adding a document to a profile
/// </summary>
public class AddDocumentToProfileRequest
{
    public Guid ProfileId { get; set; }
    public Guid FileMetadataId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string? Notes { get; set; }
    public List<SetMetadataValueRequest>? MetadataValues { get; set; }
}

/// <summary>
/// DTO for updating a document in a profile
/// </summary>
public class UpdateDocumentProfileDocumentRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string? Status { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Notes { get; set; }
    public List<SetMetadataValueRequest>? MetadataValues { get; set; }
}

/// <summary>
/// DTO for document profile document list query
/// </summary>
public class DocumentProfileDocumentListQuery
{
    public Guid? ProfileId { get; set; }
    public string? DocumentType { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? IssueDateFrom { get; set; }
    public DateTime? IssueDateTo { get; set; }
    public DateTime? ExpiryDateFrom { get; set; }
    public DateTime? ExpiryDateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

#endregion

#region Document Metadata Value DTOs

/// <summary>
/// DTO for document metadata value response
/// </summary>
public class DocumentMetadataValueDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid MetadataFieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string? StringValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
    public object? JsonValue { get; set; }
    public object? DisplayValue { get; set; }
}

/// <summary>
/// DTO for setting a metadata value
/// </summary>
public class SetMetadataValueRequest
{
    public Guid MetadataFieldId { get; set; }
    public string? StringValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BooleanValue { get; set; }
    public object? JsonValue { get; set; }
}

#endregion
