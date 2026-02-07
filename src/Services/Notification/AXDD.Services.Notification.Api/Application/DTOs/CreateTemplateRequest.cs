using AXDD.Services.Notification.Api.Domain.Enums;

namespace AXDD.Services.Notification.Api.Application.DTOs;

/// <summary>
/// Request DTO for creating a notification template
/// </summary>
public class CreateTemplateRequest
{
    public string TemplateKey { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public NotificationChannelType ChannelType { get; set; } = NotificationChannelType.InApp;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}
