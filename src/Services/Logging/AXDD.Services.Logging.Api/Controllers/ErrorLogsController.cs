using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Logging.Api.Controllers;

/// <summary>
/// Controller for error log operations
/// </summary>
[ApiController]
[Route("api/v1/logs/errors")]
[Produces("application/json")]
public class ErrorLogsController : ControllerBase
{
    private readonly IErrorLogService _errorLogService;

    public ErrorLogsController(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
    }

    /// <summary>
    /// Gets error logs with filtering and pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="severity">Optional severity filter</param>
    /// <param name="isResolved">Optional resolution status filter</param>
    /// <param name="dateFrom">Optional start date filter</param>
    /// <param name="dateTo">Optional end date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of error logs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ErrorLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ErrorLogDto>>> GetErrorsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] ErrorSeverity? severity = null,
        [FromQuery] bool? isResolved = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _errorLogService.GetErrorsAsync(pageNumber, pageSize, severity, isResolved, dateFrom, dateTo, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets an error log by ID
    /// </summary>
    /// <param name="id">Error log ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Error log details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ErrorLogDto>> GetErrorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var error = await _errorLogService.GetErrorByIdAsync(id, cancellationToken);
        return Ok(error);
    }

    /// <summary>
    /// Logs a new error
    /// </summary>
    /// <param name="request">Error log creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created error log</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ErrorLogDto>> LogErrorAsync(
        [FromBody] CreateErrorLogRequest request,
        CancellationToken cancellationToken)
    {
        var error = await _errorLogService.LogErrorAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetErrorByIdAsync), new { id = error.Id }, error);
    }

    /// <summary>
    /// Gets unresolved errors
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of unresolved error logs</returns>
    [HttpGet("unresolved")]
    [ProducesResponseType(typeof(PagedResult<ErrorLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ErrorLogDto>>> GetUnresolvedErrorsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _errorLogService.GetUnresolvedErrorsAsync(pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Resolves an error
    /// </summary>
    /// <param name="id">Error log ID</param>
    /// <param name="request">Resolution request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated error log</returns>
    [HttpPut("{id:guid}/resolve")]
    [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ErrorLogDto>> ResolveErrorAsync(
        Guid id,
        [FromBody] ResolveErrorRequest request,
        CancellationToken cancellationToken)
    {
        var error = await _errorLogService.ResolveErrorAsync(id, request, cancellationToken);
        return Ok(error);
    }

    /// <summary>
    /// Gets error logs by service name
    /// </summary>
    /// <param name="serviceName">Service name</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of error logs for the service</returns>
    [HttpGet("service/{serviceName}")]
    [ProducesResponseType(typeof(PagedResult<ErrorLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ErrorLogDto>>> GetErrorsByServiceAsync(
        string serviceName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _errorLogService.GetErrorsByServiceAsync(serviceName, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets critical errors
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of critical error logs</returns>
    [HttpGet("critical")]
    [ProducesResponseType(typeof(PagedResult<ErrorLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ErrorLogDto>>> GetCriticalErrorsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _errorLogService.GetCriticalErrorsAsync(pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }
}
