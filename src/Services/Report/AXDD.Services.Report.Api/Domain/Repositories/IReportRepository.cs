using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Report.Api.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Domain.Repositories;

/// <summary>
/// Repository interface for enterprise report-specific queries
/// </summary>
public interface IReportRepository : IRepository<EnterpriseReport>
{
    /// <summary>
    /// Gets reports for a specific enterprise
    /// </summary>
    Task<IReadOnlyList<EnterpriseReport>> GetByEnterpriseIdAsync(
        Guid enterpriseId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pending reports
    /// </summary>
    Task<IReadOnlyList<EnterpriseReport>> GetPendingReportsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reports by status
    /// </summary>
    Task<IReadOnlyList<EnterpriseReport>> GetByStatusAsync(
        ReportStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reports submitted by a specific user
    /// </summary>
    Task<IReadOnlyList<EnterpriseReport>> GetBySubmittedByAsync(
        string username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reports by type
    /// </summary>
    Task<IReadOnlyList<EnterpriseReport>> GetByReportTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reports within a date range
    /// </summary>
    Task<IReadOnlyList<EnterpriseReport>> GetByDateRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a report exists for the given enterprise, type, period, and year
    /// </summary>
    Task<bool> ExistsAsync(
        Guid enterpriseId,
        ReportType reportType,
        ReportPeriod reportPeriod,
        int year,
        int? month = null,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}
