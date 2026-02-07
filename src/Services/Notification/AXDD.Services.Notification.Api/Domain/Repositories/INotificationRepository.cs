using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Notification.Api.Domain.Entities;

namespace AXDD.Services.Notification.Api.Domain.Repositories;

/// <summary>
/// Repository interface for notification-specific queries
/// </summary>
public interface INotificationRepository : IRepository<NotificationEntity>
{
    /// <summary>
    /// Gets paginated notifications for a specific user
    /// </summary>
    Task<PagedResult<NotificationEntity>> GetByUserIdAsync(
        Guid userId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all unread notifications for a specific user
    /// </summary>
    Task<List<NotificationEntity>> GetUnreadByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of unread notifications for a specific user
    /// </summary>
    Task<int> GetUnreadCountAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    Task MarkAsReadAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications as read for a specific user
    /// </summary>
    Task MarkAllAsReadAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);
}
