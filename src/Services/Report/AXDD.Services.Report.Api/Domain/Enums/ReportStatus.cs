namespace AXDD.Services.Report.Api.Domain.Enums;

/// <summary>
/// Status of a report submission
/// </summary>
public enum ReportStatus
{
    /// <summary>
    /// Report has been submitted and is awaiting review
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Report is currently being reviewed by staff
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Report has been approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Report has been rejected
    /// </summary>
    Rejected = 4
}
