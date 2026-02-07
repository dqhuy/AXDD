using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Report.Api.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Domain.Repositories;

/// <summary>
/// Repository interface for report template-specific queries
/// </summary>
public interface IReportTemplateRepository : IRepository<ReportTemplate>
{
    /// <summary>
    /// Gets all active templates
    /// </summary>
    Task<IReadOnlyList<ReportTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a template by report type
    /// </summary>
    Task<ReportTemplate?> GetByReportTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets templates by report type (including inactive)
    /// </summary>
    Task<IReadOnlyList<ReportTemplate>> GetAllByReportTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a template name exists
    /// </summary>
    Task<bool> TemplateNameExistsAsync(
        string templateName,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}
