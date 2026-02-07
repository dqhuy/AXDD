using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Domain.Entities;

/// <summary>
/// Represents a report submitted by an enterprise
/// </summary>
public class EnterpriseReport : AuditableEntity
{
    /// <summary>
    /// Gets or sets the enterprise ID that submitted the report
    /// </summary>
    public Guid EnterpriseId { get; set; }

    /// <summary>
    /// Gets or sets the type of report
    /// </summary>
    public ReportType ReportType { get; set; }

    /// <summary>
    /// Gets or sets the reporting period
    /// </summary>
    public ReportPeriod ReportPeriod { get; set; }

    /// <summary>
    /// Gets or sets the year of the report (e.g., 2024)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the month for monthly reports (1-12, null for non-monthly)
    /// </summary>
    public int? Month { get; set; }

    /// <summary>
    /// Gets or sets the date when the report was submitted
    /// </summary>
    public DateTime SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the username of the person who submitted the report
    /// </summary>
    public string SubmittedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the report
    /// </summary>
    public ReportStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the report data as JSON
    /// </summary>
    public string DataJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of file URLs for attachments
    /// </summary>
    public List<string> Attachments { get; set; } = [];

    /// <summary>
    /// Gets or sets the user ID of the reviewer (staff member)
    /// </summary>
    public Guid? ReviewedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the report was reviewed
    /// </summary>
    public DateTime? ReviewedDate { get; set; }

    /// <summary>
    /// Gets or sets the reviewer's notes
    /// </summary>
    public string? ReviewerNotes { get; set; }

    /// <summary>
    /// Gets or sets the reason for rejection (if status is Rejected)
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Gets or sets the enterprise name (denormalized for display and querying)
    /// </summary>
    public string EnterpriseName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the enterprise code (denormalized for display)
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;
}
