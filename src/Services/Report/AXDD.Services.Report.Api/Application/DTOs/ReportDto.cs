using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Full report details DTO
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public string EnterpriseName { get; set; } = string.Empty;
    public string EnterpriseCode { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public string ReportTypeName { get; set; } = string.Empty;
    public ReportPeriod ReportPeriod { get; set; }
    public string ReportPeriodName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int? Month { get; set; }
    public DateTime SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string DataJson { get; set; } = string.Empty;
    public List<string> Attachments { get; set; } = [];
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewerNotes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
