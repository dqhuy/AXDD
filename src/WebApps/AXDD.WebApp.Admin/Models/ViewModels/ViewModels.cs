using System.ComponentModel.DataAnnotations;

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

public class SelectListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
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
