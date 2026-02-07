using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Report.Api.Application.DTOs;
using AXDD.Services.Report.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Report.Api.Controllers;

/// <summary>
/// Controller for managing enterprise reports
/// </summary>
[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Submit a new report
    /// </summary>
    /// <param name="request">Report submission request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created report</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitReportAsync(
        [FromBody] CreateReportRequest request,
        CancellationToken ct)
    {
        // TODO: Get actual user ID from authentication
        var userId = "system";

        var result = await _reportService.SubmitReportAsync(request, userId, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetReportByIdAsync),
            new { id = result.Value!.Id },
            result.Value);
    }

    /// <summary>
    /// Get reports with filtering and pagination
    /// </summary>
    /// <param name="filterParams">Filter parameters</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of reports</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ReportListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportsAsync(
        [FromQuery] ReportFilterParams filterParams,
        CancellationToken ct)
    {
        var result = await _reportService.GetReportsAsync(filterParams, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a report by ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Report details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _reportService.GetReportByIdAsync(id, ct);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Approve a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="request">Approval request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated report</returns>
    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveReportAsync(
        Guid id,
        [FromBody] ApproveReportRequest request,
        CancellationToken ct)
    {
        // TODO: Get actual reviewer ID from authentication
        var reviewerId = Guid.NewGuid();

        var result = await _reportService.ApproveReportAsync(id, request, reviewerId, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Reject a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="request">Rejection request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated report</returns>
    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectReportAsync(
        Guid id,
        [FromBody] RejectReportRequest request,
        CancellationToken ct)
    {
        // TODO: Get actual reviewer ID from authentication
        var reviewerId = Guid.NewGuid();

        var result = await _reportService.RejectReportAsync(id, request, reviewerId, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all pending reports
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of pending reports</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<ReportListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingReportsAsync(CancellationToken ct)
    {
        var result = await _reportService.GetPendingReportsAsync(ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get reports submitted by the current user
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of user's reports</returns>
    [HttpGet("my-reports")]
    [ProducesResponseType(typeof(PagedResult<ReportListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReportsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        // TODO: Get actual username from authentication
        var username = "system";

        var result = await _reportService.GetMyReportsAsync(username, pageNumber, pageSize, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}
