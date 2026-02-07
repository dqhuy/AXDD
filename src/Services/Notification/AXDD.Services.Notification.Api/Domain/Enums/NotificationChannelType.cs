namespace AXDD.Services.Notification.Api.Domain.Enums;

/// <summary>
/// Represents the channel type for sending notifications
/// </summary>
public enum NotificationChannelType
{
    /// <summary>
    /// In-app notification only
    /// </summary>
    InApp,

    /// <summary>
    /// Email notification only
    /// </summary>
    Email,

    /// <summary>
    /// Both in-app and email notifications
    /// </summary>
    Both,

    /// <summary>
    /// SMS notification (future implementation)
    /// </summary>
    SMS
}
