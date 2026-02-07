using AXDD.Services.Notification.Api.Domain.Enums;

namespace AXDD.Services.Notification.Api.Application.DTOs;

/// <summary>
/// Request DTO for sending a notification
/// </summary>
public class SendNotificationRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? ActionUrl { get; set; }
    public string? Data { get; set; }
    public bool SendEmail { get; set; }
}
