namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Request to reject a report
/// </summary>
public class RejectReportRequest
{
    /// <summary>
    /// Required reason for rejection
    /// </summary>
    public string RejectionReason { get; set; } = string.Empty;

    /// <summary>
    /// Optional additional notes from the reviewer
    /// </summary>
    public string? ReviewerNotes { get; set; }
}
