using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Enums;

namespace AXDD.Services.Notification.Api.Domain.Entities;

/// <summary>
/// Represents a notification template for consistent messaging
/// </summary>
public class NotificationTemplate : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique template key (e.g., "REPORT_APPROVED", "ENTERPRISE_CREATED")
    /// </summary>
    public string TemplateKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject/title of the notification
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the body template with {{placeholders}}
    /// </summary>
    public string BodyTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel type for this template
    /// </summary>
    public NotificationChannelType ChannelType { get; set; }

    /// <summary>
    /// Gets or sets whether the template is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the description of this template
    /// </summary>
    public string? Description { get; set; }
}
