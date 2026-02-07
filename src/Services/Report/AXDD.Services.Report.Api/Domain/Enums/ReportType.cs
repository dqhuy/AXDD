namespace AXDD.Services.Report.Api.Domain.Enums;

/// <summary>
/// Types of reports that can be submitted
/// </summary>
public enum ReportType
{
    /// <summary>
    /// Labor-related reports (employees, safety, etc.)
    /// </summary>
    Labor = 1,

    /// <summary>
    /// Environmental impact reports
    /// </summary>
    Environment = 2,

    /// <summary>
    /// Production capacity and output reports
    /// </summary>
    Production = 3,

    /// <summary>
    /// Financial reports
    /// </summary>
    Financial = 4,

    /// <summary>
    /// Other types of reports
    /// </summary>
    Other = 5
}
