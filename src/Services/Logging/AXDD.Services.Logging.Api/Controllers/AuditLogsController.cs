using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Logging.Api.Controllers;

/// <summary>
/// Controller for audit log operations
/// </summary>
[ApiController]
[Route("api/v1/logs/audit")]
[Produces("application/json")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(IAuditLogService auditLogService, ILogger<AuditLogsController> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Gets audit logs with filtering and pagination
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of audit logs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetLogsAsync([FromQuery] LogFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetLogsAsync(filter, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets an audit log by ID
    /// </summary>
    /// <param name="id">Log ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audit log details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditLogDto>> GetLogByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var log = await _auditLogService.GetLogByIdAsync(id, cancellationToken);
        return Ok(log);
    }

    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    /// <param name="request">Audit log creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created audit log</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuditLogDto>> CreateLogAsync([FromBody] CreateAuditLogRequest request, CancellationToken cancellationToken)
    {
        var log = await _auditLogService.CreateLogAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetLogByIdAsync), new { id = log.Id }, log);
    }

    /// <summary>
    /// Gets audit logs by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of audit logs for the user</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetLogsByUserAsync(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _auditLogService.GetLogsByUserAsync(userId, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets audit logs by service name
    /// </summary>
    /// <param name="serviceName">Service name</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of audit logs for the service</returns>
    [HttpGet("service/{serviceName}")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetLogsByServiceAsync(
        string serviceName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _auditLogService.GetLogsByServiceAsync(serviceName, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets audit logs by correlation ID for tracing requests across services
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of audit logs with the same correlation ID</returns>
    [HttpGet("trace/{correlationId}")]
    [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AuditLogDto>>> GetLogsByCorrelationIdAsync(
        string correlationId,
        CancellationToken cancellationToken)
    {
        var logs = await _auditLogService.GetLogsByCorrelationIdAsync(correlationId, cancellationToken);
        return Ok(logs);
    }

    /// <summary>
    /// Deletes audit logs older than the specified number of days
    /// </summary>
    /// <param name="olderThanDays">Delete logs older than this many days</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of logs deleted</returns>
    [HttpDelete("cleanup")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> DeleteOldLogsAsync(
        [FromQuery] int olderThanDays,
        CancellationToken cancellationToken)
    {
        if (olderThanDays < 1)
            return BadRequest("olderThanDays must be at least 1");

        var deletedCount = await _auditLogService.DeleteOldLogsAsync(olderThanDays, cancellationToken);
        _logger.LogInformation("Deleted {Count} audit logs older than {Days} days", deletedCount, olderThanDays);
        return Ok(deletedCount);
    }
}
