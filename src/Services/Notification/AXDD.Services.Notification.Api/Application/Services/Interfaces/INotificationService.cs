using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Notification.Api.Application.DTOs;

namespace AXDD.Services.Notification.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for notification management
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to a user
    /// </summary>
    Task<Result<NotificationDto>> SendNotificationAsync(
        SendNotificationRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated notifications for the current user
    /// </summary>
    Task<Result<PagedResult<NotificationListDto>>> GetMyNotificationsAsync(
        Guid userId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a notification by ID
    /// </summary>
    Task<Result<NotificationDto>> GetNotificationByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    Task<Result<bool>> MarkAsReadAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications as read for a user
    /// </summary>
    Task<Result<bool>> MarkAllAsReadAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a notification
    /// </summary>
    Task<Result<bool>> DeleteNotificationAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of unread notifications for a user
    /// </summary>
    Task<Result<int>> GetUnreadCountAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);
}
