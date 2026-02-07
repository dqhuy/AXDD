using AXDD.Services.Notification.Api.Domain.Enums;

namespace AXDD.Services.Notification.Api.Application.DTOs;

/// <summary>
/// DTO for notification template
/// </summary>
public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public NotificationChannelType ChannelType { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
