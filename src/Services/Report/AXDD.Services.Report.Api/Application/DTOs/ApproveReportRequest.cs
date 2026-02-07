namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Request to approve a report
/// </summary>
public class ApproveReportRequest
{
    /// <summary>
    /// Optional notes from the reviewer
    /// </summary>
    public string? ReviewerNotes { get; set; }
}
