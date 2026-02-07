using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Domain.Entities;

/// <summary>
/// Represents a user activity log entry
/// </summary>
public class UserActivityLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the activity type
    /// </summary>
    public ActivityType ActivityType { get; set; }

    /// <summary>
    /// Gets or sets the activity description
    /// </summary>
    public string ActivityDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the device information
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Gets or sets the resource type being acted upon
    /// </summary>
    public string? ResourceType { get; set; }

    /// <summary>
    /// Gets or sets the resource ID being acted upon
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// Gets or sets the old value (for Update operations)
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value (for Update operations)
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
