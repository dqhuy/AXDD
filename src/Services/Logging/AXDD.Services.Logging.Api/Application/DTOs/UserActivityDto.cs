using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Application.DTOs;

/// <summary>
/// DTO for user activity log
/// </summary>
public class UserActivityDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public string ActivityDescription { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? ResourceType { get; set; }
    public Guid? ResourceId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// Request DTO for creating a user activity log
/// </summary>
public class CreateUserActivityRequest
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public string ActivityDescription { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? ResourceType { get; set; }
    public Guid? ResourceId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? AdditionalData { get; set; }
}
