using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Report.Api.Application.DTOs;

namespace AXDD.Services.Report.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for managing enterprise reports
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Submits a new report
    /// </summary>
    /// <param name="request">Report creation request</param>
    /// <param name="userId">ID of the user submitting the report</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created report details</returns>
    Task<Result<ReportDto>> SubmitReportAsync(
        CreateReportRequest request,
        string userId,
        CancellationToken ct);

    /// <summary>
    /// Gets filtered and paginated reports
    /// </summary>
    /// <param name="filterParams">Filter parameters</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of reports</returns>
    Task<Result<PagedResult<ReportListDto>>> GetReportsAsync(
        ReportFilterParams filterParams,
        CancellationToken ct);

    /// <summary>
    /// Gets a report by its ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Report details</returns>
    Task<Result<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Approves a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="request">Approval request</param>
    /// <param name="reviewerId">ID of the reviewer (staff member)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated report details</returns>
    Task<Result<ReportDto>> ApproveReportAsync(
        Guid id,
        ApproveReportRequest request,
        Guid reviewerId,
        CancellationToken ct);

    /// <summary>
    /// Rejects a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="request">Rejection request</param>
    /// <param name="reviewerId">ID of the reviewer (staff member)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated report details</returns>
    Task<Result<ReportDto>> RejectReportAsync(
        Guid id,
        RejectReportRequest request,
        Guid reviewerId,
        CancellationToken ct);

    /// <summary>
    /// Gets all pending reports
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of pending reports</returns>
    Task<Result<List<ReportListDto>>> GetPendingReportsAsync(CancellationToken ct);

    /// <summary>
    /// Gets reports submitted by a specific user
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of user's reports</returns>
    Task<Result<PagedResult<ReportListDto>>> GetMyReportsAsync(
        string username,
        int pageNumber,
        int pageSize,
        CancellationToken ct);
}
