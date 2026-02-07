using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Notification.Api.Application.DTOs;
using AXDD.Services.Notification.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Notification.Api.Controllers;

/// <summary>
/// Controller for notification management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Sends a notification to a user
    /// </summary>
    /// <param name="request">The notification details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> SendNotification(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.SendNotificationAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<NotificationDto>.Failure(result.Error ?? "Failed to send notification"));
        }

        return Ok(ApiResponse<NotificationDto>.Success(result.Value!));
    }

    /// <summary>
    /// Gets paginated notifications for the authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NotificationListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NotificationListDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationListDto>>>> GetMyNotifications(
        [FromQuery] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        pageSize = Math.Min(pageSize, 100);

        var result = await _notificationService.GetMyNotificationsAsync(
            userId, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<PagedResult<NotificationListDto>>.Failure(
                result.Error ?? "Failed to retrieve notifications"));
        }

        return Ok(ApiResponse<PagedResult<NotificationListDto>>.Success(result.Value!));
    }

    /// <summary>
    /// Gets a notification by ID
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.GetNotificationByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<NotificationDto>.NotFound(result.Error ?? "Notification not found"));
        }

        return Ok(ApiResponse<NotificationDto>.Success(result.Value!));
    }

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.MarkAsReadAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<bool>.NotFound(result.Error ?? "Notification not found"));
        }

        return Ok(ApiResponse<bool>.Success(result.Value));
    }

    /// <summary>
    /// Marks all notifications as read for the authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("read-all")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<bool>.Failure(result.Error ?? "Failed to mark all as read"));
        }

        return Ok(ApiResponse<bool>.Success(result.Value));
    }

    /// <summary>
    /// Deletes a notification
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.DeleteNotificationAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<bool>.NotFound(result.Error ?? "Notification not found"));
        }

        return Ok(ApiResponse<bool>.Success(result.Value));
    }

    /// <summary>
    /// Gets the count of unread notifications for the authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<int>.Failure(result.Error ?? "Failed to get unread count"));
        }

        return Ok(ApiResponse<int>.Success(result.Value));
    }
}
