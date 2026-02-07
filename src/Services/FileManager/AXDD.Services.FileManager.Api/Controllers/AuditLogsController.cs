using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for audit log operations
/// </summary>
[ApiController]
[Route("api/v1/audit-logs")]
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
    /// Lists audit logs with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] string? userId = null,
        [FromQuery] AuditAction? action = null,
        [FromQuery] string? entityType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new AuditLogQueryDto
        {
            EnterpriseCode = enterpriseCode,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _auditLogService.ListAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets access logs (read actions)
    /// </summary>
    [HttpGet("access")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccessLogs(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _auditLogService.GetAccessLogsAsync(enterpriseCode, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets login logs
    /// </summary>
    [HttpGet("login")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoginLogs(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _auditLogService.GetLoginLogsAsync(enterpriseCode, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets content change logs
    /// </summary>
    [HttpGet("changes")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChangeLogs(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] string? entityType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _auditLogService.GetChangeLogsAsync(enterpriseCode, entityType, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Deletes logs older than a specified date (admin only)
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteOlderThan([FromQuery] DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _auditLogService.DeleteLogsOlderThanAsync(olderThan, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<int>.SuccessResponse(result.Value, $"Deleted {result.Value} audit log entries"));
    }
}
