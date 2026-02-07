using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Request to create a new report
/// </summary>
public class CreateReportRequest
{
    public Guid EnterpriseId { get; set; }
    public string EnterpriseName { get; set; } = string.Empty;
    public string EnterpriseCode { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public ReportPeriod ReportPeriod { get; set; }
    public int Year { get; set; }
    public int? Month { get; set; }
    public string DataJson { get; set; } = string.Empty;
    public List<string> Attachments { get; set; } = [];
}
