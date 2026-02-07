using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Report.Api.Application.DTOs;
using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for managing report templates
/// </summary>
public interface IReportTemplateService
{
    /// <summary>
    /// Gets all active templates
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of active templates</returns>
    Task<Result<List<ReportTemplateDto>>> GetTemplatesAsync(CancellationToken ct);

    /// <summary>
    /// Gets a template by its ID
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Template details</returns>
    Task<Result<ReportTemplateDto>> GetTemplateByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Gets a template by report type
    /// </summary>
    /// <param name="type">Report type</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Template details</returns>
    Task<Result<ReportTemplateDto>> GetTemplateByTypeAsync(ReportType type, CancellationToken ct);

    /// <summary>
    /// Creates a new template
    /// </summary>
    /// <param name="request">Template creation request</param>
    /// <param name="userId">ID of the user creating the template</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created template details</returns>
    Task<Result<ReportTemplateDto>> CreateTemplateAsync(
        CreateTemplateRequest request,
        string userId,
        CancellationToken ct);
}
