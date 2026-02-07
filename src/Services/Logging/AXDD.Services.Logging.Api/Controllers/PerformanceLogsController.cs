using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Logging.Api.Controllers;

/// <summary>
/// Controller for performance log operations
/// </summary>
[ApiController]
[Route("api/v1/logs/performance")]
[Produces("application/json")]
public class PerformanceLogsController : ControllerBase
{
    private readonly IPerformanceLogService _performanceLogService;

    public PerformanceLogsController(IPerformanceLogService performanceLogService)
    {
        _performanceLogService = performanceLogService;
    }

    /// <summary>
    /// Gets performance logs with filtering and pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="serviceName">Optional service name filter</param>
    /// <param name="dateFrom">Optional start date filter</param>
    /// <param name="dateTo">Optional end date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of performance logs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PerformanceLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PerformanceLogDto>>> GetPerformanceLogsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? serviceName = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _performanceLogService.GetPerformanceLogsAsync(pageNumber, pageSize, serviceName, dateFrom, dateTo, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Logs performance metrics
    /// </summary>
    /// <param name="request">Performance log creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created performance log</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PerformanceLogDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PerformanceLogDto>> LogPerformanceAsync(
        [FromBody] CreatePerformanceLogRequest request,
        CancellationToken cancellationToken)
    {
        var performance = await _performanceLogService.LogPerformanceAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetPerformanceLogsAsync), null, performance);
    }

    /// <summary>
    /// Gets service statistics
    /// </summary>
    /// <param name="serviceName">Service name</param>
    /// <param name="startDate">Optional start date (default: 7 days ago)</param>
    /// <param name="endDate">Optional end date (default: now)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Service statistics including average duration, error rate, etc.</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(LogStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LogStatisticsDto>> GetServiceStatisticsAsync(
        [FromQuery] string serviceName,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            return BadRequest("serviceName is required");

        var statistics = await _performanceLogService.GetServiceStatisticsAsync(serviceName, startDate, endDate, cancellationToken);
        return Ok(statistics);
    }

    /// <summary>
    /// Gets slow requests above the specified threshold
    /// </summary>
    /// <param name="thresholdMs">Duration threshold in milliseconds (default: 1000)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of slow requests</returns>
    [HttpGet("slow")]
    [ProducesResponseType(typeof(PagedResult<PerformanceLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PerformanceLogDto>>> GetSlowRequestsAsync(
        [FromQuery] long thresholdMs = 1000,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _performanceLogService.GetSlowRequestsAsync(thresholdMs, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }
}
