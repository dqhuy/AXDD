using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Notification.Api.Domain.Entities;

namespace AXDD.Services.Notification.Api.Domain.Repositories;

/// <summary>
/// Repository interface for notification template queries
/// </summary>
public interface INotificationTemplateRepository : IRepository<NotificationTemplate>
{
    /// <summary>
    /// Gets a notification template by its key
    /// </summary>
    Task<NotificationTemplate?> GetByKeyAsync(
        string key, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active notification templates
    /// </summary>
    Task<List<NotificationTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default);
}
