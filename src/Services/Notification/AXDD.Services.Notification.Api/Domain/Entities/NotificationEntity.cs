using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Enums;

namespace AXDD.Services.Notification.Api.Domain.Entities;

/// <summary>
/// Represents a notification in the system
/// </summary>
public class NotificationEntity : AuditableEntity
{
    /// <summary>
    /// Gets or sets the user ID who will receive the notification
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification type
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Gets or sets whether the notification has been read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was read
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Gets or sets the type of related entity (e.g., "Enterprise", "Report", "Document")
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the related entity
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Gets or sets the action URL to navigate to
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets additional JSON data for the notification
    /// </summary>
    public string? Data { get; set; }
}
