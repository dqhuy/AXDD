using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Notification.Api.Application.DTOs;
using AXDD.Services.Notification.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Notification.Api.Controllers;

/// <summary>
/// Controller for notification template management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class NotificationTemplatesController : ControllerBase
{
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<NotificationTemplatesController> _logger;

    public NotificationTemplatesController(
        INotificationTemplateService templateService,
        ILogger<NotificationTemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all notification templates
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationTemplateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationTemplateDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<List<NotificationTemplateDto>>>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var result = await _templateService.GetAllTemplatesAsync(cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<List<NotificationTemplateDto>>.Failure(
                result.Error ?? "Failed to retrieve templates"));
        }

        return Ok(ApiResponse<List<NotificationTemplateDto>>.Success(result.Value!));
    }

    /// <summary>
    /// Gets a notification template by ID
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NotificationTemplateDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _templateService.GetTemplateByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<NotificationTemplateDto>.NotFound(
                result.Error ?? "Template not found"));
        }

        return Ok(ApiResponse<NotificationTemplateDto>.Success(result.Value!));
    }

    /// <summary>
    /// Gets a notification template by its key
    /// </summary>
    /// <param name="key">Template key (e.g., REPORT_APPROVED)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("by-key/{key}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NotificationTemplateDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> GetByKey(
        string key,
        CancellationToken cancellationToken = default)
    {
        var result = await _templateService.GetTemplateByKeyAsync(key, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<NotificationTemplateDto>.NotFound(
                result.Error ?? "Template not found"));
        }

        return Ok(ApiResponse<NotificationTemplateDto>.Success(result.Value!));
    }

    /// <summary>
    /// Creates a new notification template (admin only)
    /// </summary>
    /// <param name="request">Template creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationTemplateDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<NotificationTemplateDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<NotificationTemplateDto>>> Create(
        [FromBody] CreateTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _templateService.CreateTemplateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<NotificationTemplateDto>.Failure(
                result.Error ?? "Failed to create template"));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            ApiResponse<NotificationTemplateDto>.Success(result.Value!));
    }

    /// <summary>
    /// Gets all active notification templates
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationTemplateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationTemplateDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<List<NotificationTemplateDto>>>> GetActive(
        CancellationToken cancellationToken = default)
    {
        var result = await _templateService.GetActiveTemplatesAsync(cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<List<NotificationTemplateDto>>.Failure(
                result.Error ?? "Failed to retrieve active templates"));
        }

        return Ok(ApiResponse<List<NotificationTemplateDto>>.Success(result.Value!));
    }
}
