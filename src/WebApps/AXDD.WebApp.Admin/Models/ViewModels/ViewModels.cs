using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AXDD.WebApp.Admin.Models.ViewModels;

/// <summary>
/// Login view model
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Dashboard view model
/// </summary>
public class DashboardViewModel
{
    public int TotalEnterprises { get; set; }
    public int ActiveEnterprises { get; set; }
    public int PendingReports { get; set; }
    public int TotalDocuments { get; set; }
    public int UnreadNotifications { get; set; }
    public List<EnterpriseTypeChart> EnterprisesByType { get; set; } = new();
    public List<ReportStatusChart> ReportsByStatus { get; set; } = new();
    public List<RecentActivityItem> RecentActivities { get; set; } = new();
}

public class EnterpriseTypeChart
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportStatusChart
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentActivityItem
{
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Enterprise list view model
/// </summary>
public class EnterpriseListViewModel
{
    public List<EnterpriseItemViewModel> Enterprises { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }
    public string? TypeFilter { get; set; }
}

public class EnterpriseItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string? IndustrialZone { get; set; }
    public string EnterpriseType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Enterprise create/edit view model
/// </summary>
public class EnterpriseFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Enterprise name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    [Display(Name = "Enterprise Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tax code is required")]
    [StringLength(50, ErrorMessage = "Tax code cannot exceed 50 characters")]
    [Display(Name = "Tax Code")]
    public string TaxCode { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Legal Representative")]
    public string? LegalRepresentative { get; set; }

    [StringLength(500)]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    [Phone]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Url]
    [StringLength(200)]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    [StringLength(200)]
    [Display(Name = "Industrial Zone")]
    public string? IndustrialZone { get; set; }

    [Required(ErrorMessage = "Enterprise type is required")]
    [Display(Name = "Enterprise Type")]
    public string EnterpriseType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Active";

    public bool IsEditMode => Id.HasValue;
}

/// <summary>
/// Enterprise details view model
/// </summary>
public class EnterpriseDetailsViewModel
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
    public int TotalDocuments { get; set; }
    public int TotalReports { get; set; }
}

/// <summary>
/// Document list view model
/// </summary>
public class DocumentListViewModel
{
    public List<DocumentItemViewModel> Documents { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public Guid? EnterpriseId { get; set; }
    public string? DocumentType { get; set; }
}

public class DocumentItemViewModel
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string FileSizeFormatted => FormatFileSize(FileSize);

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Document upload view model
/// </summary>
public class DocumentUploadViewModel
{
    [Required(ErrorMessage = "Please select an enterprise")]
    [Display(Name = "Enterprise")]
    public Guid EnterpriseId { get; set; }

    [Required(ErrorMessage = "Please select a file")]
    [Display(Name = "File")]
    public IFormFile File { get; set; } = null!;

    [Required(ErrorMessage = "Document type is required")]
    [Display(Name = "Document Type")]
    public string DocumentType { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    public List<SelectListItem> Enterprises { get; set; } = new();
    public List<SelectListItem> DocumentTypes { get; set; } = new();
}

/// <summary>
/// Report list view model
/// </summary>
public class ReportListViewModel
{
    public List<ReportItemViewModel> Reports { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public string? StatusFilter { get; set; }
    public string? TypeFilter { get; set; }
}

public class ReportItemViewModel
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
    public string PeriodDisplay => Quarter.HasValue ? $"Q{Quarter}/{Year}" :
                                   Month.HasValue ? $"{Month:D2}/{Year}" :
                                   $"{Year}";
}

/// <summary>
/// Report details view model
/// </summary>
public class ReportDetailsViewModel
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
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComments { get; set; }
    public string? Data { get; set; }
}

/// <summary>
/// Report approval view model
/// </summary>
public class ReportApprovalViewModel
{
    public Guid ReportId { get; set; }
    
    [Required(ErrorMessage = "Comments are required")]
    [StringLength(1000)]
    [Display(Name = "Review Comments")]
    public string Comments { get; set; } = string.Empty;

    public bool Approve { get; set; }
}

/// <summary>
/// Notification list view model
/// </summary>
public class NotificationListViewModel
{
    public List<NotificationItemViewModel> Notifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class NotificationItemViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
    public string Icon => Type switch
    {
        "Info" => "fas fa-info-circle",
        "Warning" => "fas fa-exclamation-triangle",
        "Error" => "fas fa-times-circle",
        "Success" => "fas fa-check-circle",
        _ => "fas fa-bell"
    };
    public string ColorClass => Type switch
    {
        "Info" => "text-info",
        "Warning" => "text-warning",
        "Error" => "text-danger",
        "Success" => "text-success",
        _ => "text-secondary"
    };
}

// ============================================
// Document Profile Management ViewModels
// ============================================

/// <summary>
/// Document Profile List View Model
/// </summary>
public class DocumentProfileListViewModel
{
    public List<DocumentProfileItemViewModel> Profiles { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    // Filters
    public string? EnterpriseCode { get; set; }
    public Guid? ParentProfileId { get; set; }
    public string? ParentProfileName { get; set; }
    public string? ProfileType { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsTemplate { get; set; }
    
    // View mode
    public string ViewMode { get; set; } = "grid"; // grid or list
    
    // Breadcrumb navigation
    public List<BreadcrumbItem> Breadcrumbs { get; set; } = new();
}

public class BreadcrumbItem
{
    public Guid? ProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Document Profile Item View Model
/// </summary>
public class DocumentProfileItemViewModel
{
    public Guid Id { get; set; }
    public string ProfileCode { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EnterpriseCode { get; set; } = string.Empty;
    public Guid? ParentProfileId { get; set; }
    public string ProfileType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsTemplate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int DocumentCount { get; set; }
    public int ChildProfileCount { get; set; }
    
    public string Icon => ProfileType switch
    {
        "Folder" => "fas fa-folder",
        "Project" => "fas fa-briefcase",
        "Contract" => "fas fa-file-contract",
        "Report" => "fas fa-file-chart-line",
        _ => "fas fa-folder"
    };
    
    public string ColorClass => Status switch
    {
        "Open" => "text-success",
        "Closed" => "text-secondary",
        "Archived" => "text-muted",
        _ => "text-info"
    };
    
    public string BadgeClass => Status switch
    {
        "Open" => "badge-success",
        "Closed" => "badge-secondary",
        "Archived" => "badge-dark",
        _ => "badge-info"
    };
}

/// <summary>
/// Document Profile Form View Model (Create/Edit)
/// </summary>
public class DocumentProfileFormViewModel
{
    public Guid? Id { get; set; }
    
    [Required(ErrorMessage = "Mã hồ sơ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Mã hồ sơ không được vượt quá 50 ký tự")]
    [Display(Name = "Mã hồ sơ")]
    public string ProfileCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Tên hồ sơ là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên hồ sơ không được vượt quá 200 ký tự")]
    [Display(Name = "Tên hồ sơ")]
    public string ProfileName { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Mã doanh nghiệp là bắt buộc")]
    [Display(Name = "Doanh nghiệp")]
    public string EnterpriseCode { get; set; } = string.Empty;
    
    [Display(Name = "Hồ sơ cha")]
    public Guid? ParentProfileId { get; set; }
    
    [Required(ErrorMessage = "Loại hồ sơ là bắt buộc")]
    [Display(Name = "Loại hồ sơ")]
    public string ProfileType { get; set; } = string.Empty;
    
    [Display(Name = "Là mẫu hồ sơ")]
    public bool IsTemplate { get; set; }
    
    public bool IsEditMode => Id.HasValue;
    
    // For dropdowns
    public List<SelectListItem> Enterprises { get; set; } = new();
    public List<SelectListItem> ParentProfiles { get; set; } = new();
    public List<SelectListItem> ProfileTypes { get; set; } = new();
}

/// <summary>
/// Document Profile Details View Model
/// </summary>
public class DocumentProfileDetailsViewModel
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
    
    // Related data
    public List<DocumentProfileItemViewModel> ChildProfiles { get; set; } = new();
    public List<ProfileDocumentItemViewModel> Documents { get; set; } = new();
    public List<ProfileMetadataFieldViewModel> MetadataFields { get; set; } = new();
    
    // Breadcrumb navigation
    public List<BreadcrumbItem> Breadcrumbs { get; set; } = new();
}

/// <summary>
/// Profile Metadata Field View Model
/// </summary>
public class ProfileMetadataFieldViewModel
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
    
    public string FieldTypeIcon => FieldType switch
    {
        "Text" => "fas fa-font",
        "Number" => "fas fa-hashtag",
        "Date" => "fas fa-calendar",
        "Select" => "fas fa-list",
        "Checkbox" => "fas fa-check-square",
        "TextArea" => "fas fa-align-left",
        _ => "fas fa-question"
    };
}

/// <summary>
/// Profile Metadata Field Form View Model
/// </summary>
public class ProfileMetadataFieldFormViewModel
{
    public Guid? Id { get; set; }
    
    [Required(ErrorMessage = "ID hồ sơ là bắt buộc")]
    public Guid ProfileId { get; set; }
    
    [Required(ErrorMessage = "Tên trường là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên trường không được vượt quá 100 ký tự")]
    [Display(Name = "Tên trường (Field Name)")]
    public string FieldName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Nhãn hiển thị là bắt buộc")]
    [StringLength(200, ErrorMessage = "Nhãn hiển thị không được vượt quá 200 ký tự")]
    [Display(Name = "Nhãn hiển thị")]
    public string FieldLabel { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Loại trường là bắt buộc")]
    [Display(Name = "Loại trường")]
    public string FieldType { get; set; } = string.Empty;
    
    [Display(Name = "Bắt buộc")]
    public bool IsRequired { get; set; }
    
    [Display(Name = "Thứ tự hiển thị")]
    public int DisplayOrder { get; set; }
    
    [StringLength(500, ErrorMessage = "Quy tắc xác thực không được vượt quá 500 ký tự")]
    [Display(Name = "Quy tắc xác thực")]
    public string? ValidationRules { get; set; }
    
    [StringLength(200, ErrorMessage = "Giá trị mặc định không được vượt quá 200 ký tự")]
    [Display(Name = "Giá trị mặc định")]
    public string? DefaultValue { get; set; }
    
    [StringLength(1000, ErrorMessage = "Tùy chọn không được vượt quá 1000 ký tự")]
    [Display(Name = "Tùy chọn (JSON)")]
    public string? Options { get; set; }
    
    public bool IsEditMode => Id.HasValue;
    
    // For dropdown
    public List<SelectListItem> FieldTypes { get; set; } = new();
}

/// <summary>
/// Profile Document Item View Model
/// </summary>
public class ProfileDocumentItemViewModel
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
    public string? FilePath { get; set; }
    
    public string FileSizeFormatted => FormatFileSize(FileSize);
    
    public string Icon => GetFileIcon(FileType);
    
    public string StatusBadgeClass => Status switch
    {
        "Active" => "badge-success",
        "Expired" => "badge-danger",
        "Archived" => "badge-secondary",
        _ => "badge-info"
    };
    
    public bool IsExpiring => ExpiryDate.HasValue && 
                              ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) &&
                              ExpiryDate.Value > DateTime.UtcNow;
    
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
    
    private static string GetFileIcon(string fileType)
    {
        var extension = fileType?.ToLower();
        return extension switch
        {
            ".pdf" => "fas fa-file-pdf text-danger",
            ".doc" or ".docx" => "fas fa-file-word text-primary",
            ".xls" or ".xlsx" => "fas fa-file-excel text-success",
            ".ppt" or ".pptx" => "fas fa-file-powerpoint text-warning",
            ".jpg" or ".jpeg" or ".png" or ".gif" => "fas fa-file-image text-info",
            ".zip" or ".rar" or ".7z" => "fas fa-file-archive text-secondary",
            ".txt" => "fas fa-file-alt text-muted",
            _ => "fas fa-file text-muted"
        };
    }
}

/// <summary>
/// Profile Document Form View Model
/// </summary>
public class ProfileDocumentFormViewModel
{
    public Guid? Id { get; set; }
    
    [Required(ErrorMessage = "ID hồ sơ là bắt buộc")]
    public Guid ProfileId { get; set; }
    
    [Required(ErrorMessage = "Tài liệu là bắt buộc")]
    [Display(Name = "Tài liệu")]
    public Guid DocumentId { get; set; }
    
    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    
    [Display(Name = "Thứ tự hiển thị")]
    public int DisplayOrder { get; set; }
    
    [Display(Name = "Ngày hết hạn")]
    public DateTime? ExpiryDate { get; set; }
    
    [Display(Name = "Trạng thái")]
    public string Status { get; set; } = "Active";
    
    public bool IsEditMode => Id.HasValue;
    
    // For dropdown
    public List<SelectListItem> Documents { get; set; } = new();
    public List<SelectListItem> Statuses { get; set; } = new();
    
    // Metadata values
    public Dictionary<Guid, string> MetadataValues { get; set; } = new();
}

/// <summary>
/// Document Metadata List View Model
/// </summary>
public class DocumentMetadataListViewModel
{
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public List<DocumentMetadataItemViewModel> Metadata { get; set; } = new();
}

public class DocumentMetadataItemViewModel
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? Value { get; set; }
    public string? Options { get; set; }
}
