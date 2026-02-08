using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents an audit log entry for system logging
/// </summary>
public class AuditLog : BaseEntity
{
    public AuditLog()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the user ID who performed the action
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user name who performed the action
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the action performed
    /// </summary>
    public AuditAction Action { get; set; }

    /// <summary>
    /// Gets or sets the entity type (File, Folder, etc.)
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity ID
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity name (snapshot at audit time)
    /// </summary>
    public string? EntityName { get; set; }

    /// <summary>
    /// Gets or sets the old value (for updates)
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value (for updates)
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the action
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the enterprise code
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;
}
