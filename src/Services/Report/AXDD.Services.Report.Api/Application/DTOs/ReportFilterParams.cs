using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Filter parameters for report queries
/// </summary>
public class ReportFilterParams
{
    public ReportStatus? Status { get; set; }
    public ReportType? ReportType { get; set; }
    public Guid? EnterpriseId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? SearchTerm { get; set; }
    public int? Year { get; set; }
    public ReportPeriod? ReportPeriod { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;
}
