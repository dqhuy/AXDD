namespace AXDD.Services.Report.Api.Domain.Enums;

/// <summary>
/// Reporting period for enterprise reports
/// </summary>
public enum ReportPeriod
{
    /// <summary>
    /// Monthly report
    /// </summary>
    Monthly = 1,

    /// <summary>
    /// First quarter (Q1)
    /// </summary>
    Q1 = 2,

    /// <summary>
    /// Second quarter (Q2)
    /// </summary>
    Q2 = 3,

    /// <summary>
    /// Third quarter (Q3)
    /// </summary>
    Q3 = 4,

    /// <summary>
    /// Fourth quarter (Q4)
    /// </summary>
    Q4 = 5,

    /// <summary>
    /// Annual report
    /// </summary>
    Annual = 6
}
