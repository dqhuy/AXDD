namespace AXDD.WebApp.Admin.Models.ApiModels;

/// <summary>
/// API response wrapper
/// </summary>
/// <typeparam name="T">Type of data</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Login response model
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfo User { get; set; } = null!;
}

/// <summary>
/// User information
/// </summary>
public class UserInfo
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// Enterprise API model
/// </summary>
public class EnterpriseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string? LegalRepresentative { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? IndustrialZone { get; set; }
    public string EnterpriseType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Document API model
/// </summary>
public class DocumentDto
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid UploadedBy { get; set; }
}

/// <summary>
/// Report API model
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public string EnterpriseName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public int Year { get; set; }
    public int? Quarter { get; set; }
    public int? Month { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public Guid SubmittedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? ReviewComments { get; set; }
    public string? Data { get; set; }
}

/// <summary>
/// Notification API model
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
}

/// <summary>
/// Paginated list response
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Dashboard statistics
/// </summary>
public class DashboardStats
{
    public int TotalEnterprises { get; set; }
    public int ActiveEnterprises { get; set; }
    public int PendingReports { get; set; }
    public int TotalDocuments { get; set; }
    public int UnreadNotifications { get; set; }
    public List<EnterprisesByTypeDto> EnterprisesByType { get; set; } = new();
    public List<ReportsByStatusDto> ReportsByStatus { get; set; } = new();
}

public class EnterprisesByTypeDto
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportsByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

// ============================================
// Document Profile Management API Models
// ============================================

/// <summary>
/// Document Profile DTO
/// </summary>
public class DocumentProfileDto
{
    public Guid Id { get; set; }
    public string ProfileCode { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EnterpriseCode { get; set; } = string.Empty;
    public Guid? ParentProfileId { get; set; }
    public string? ParentProfileName { get; set; }
    public string ProfileType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public int DocumentCount { get; set; }
    public int ChildProfileCount { get; set; }
}

/// <summary>
/// Create Document Profile Request
/// </summary>
public class CreateDocumentProfileRequest
{
    public string ProfileCode { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EnterpriseCode { get; set; } = string.Empty;
    public Guid? ParentProfileId { get; set; }
    public string ProfileType { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
}

/// <summary>
/// Update Document Profile Request
/// </summary>
public class UpdateDocumentProfileRequest
{
    public string ProfileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ProfileType { get; set; } = string.Empty;
}

/// <summary>
/// Profile Hierarchy DTO
/// </summary>
public class ProfileHierarchyDto
{
    public Guid Id { get; set; }
    public string ProfileCode { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string ProfileType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DocumentCount { get; set; }
    public List<ProfileHierarchyDto> Children { get; set; } = new();
}

/// <summary>
/// Profile Metadata Field DTO
/// </summary>
public class ProfileMetadataFieldDto
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? ValidationRules { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create Profile Metadata Field Request
/// </summary>
public class CreateProfileMetadataFieldRequest
{
    public Guid ProfileId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? ValidationRules { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
}

/// <summary>
/// Update Profile Metadata Field Request
/// </summary>
public class UpdateProfileMetadataFieldRequest
{
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? ValidationRules { get; set; }
    public string? DefaultValue { get; set; }
    public string? Options { get; set; }
}

/// <summary>
/// Document Profile Document DTO
/// </summary>
public class DocumentProfileDocumentDto
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string DocumentCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public Guid AddedBy { get; set; }
    public string? FilePath { get; set; }
}

/// <summary>
/// Add Document to Profile Request
/// </summary>
public class AddDocumentToProfileRequest
{
    public Guid ProfileId { get; set; }
    public Guid DocumentId { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>
/// Update Document Profile Document Request
/// </summary>
public class UpdateDocumentProfileDocumentRequest
{
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Document Metadata Value DTO
/// </summary>
public class DocumentMetadataValueDto
{
    public Guid DocumentId { get; set; }
    public Guid FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string? Value { get; set; }
}

/// <summary>
/// Set Metadata Value Request
/// </summary>
public class SetMetadataValueRequest
{
    public Guid FieldId { get; set; }
    public string? Value { get; set; }
}

/// <summary>
/// Reorder Request
/// </summary>
public class ReorderRequest
{
    public List<Guid> Ids { get; set; } = new();
}

/// <summary>
/// Copy Fields Request
/// </summary>
public class CopyFieldsRequest
{
    public Guid SourceProfileId { get; set; }
    public Guid TargetProfileId { get; set; }
}

/// <summary>
/// Move Document Request
/// </summary>
public class MoveDocumentRequest
{
    public Guid TargetProfileId { get; set; }
}

/// <summary>
/// Copy Document Request
/// </summary>
public class CopyDocumentRequest
{
    public Guid TargetProfileId { get; set; }
}
