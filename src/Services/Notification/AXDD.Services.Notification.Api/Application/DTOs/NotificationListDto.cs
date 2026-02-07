using AXDD.Services.Notification.Api.Domain.Enums;

namespace AXDD.Services.Notification.Api.Application.DTOs;

/// <summary>
/// DTO for notification list items (summary view)
/// </summary>
public class NotificationListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
