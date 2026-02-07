using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking system activities
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the timestamp when the log was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the log level
    /// </summary>
    public AuditLogLevel Level { get; set; } = AuditLogLevel.Info;

    /// <summary>
    /// Gets or sets the user ID who performed the action
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the username who performed the action
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the user role
    /// </summary>
    public string? UserRole { get; set; }

    /// <summary>
    /// Gets or sets the name of the service that generated the log
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the action name (e.g., CreateEnterprise)
    /// </summary>
    public string? ActionName { get; set; }

    /// <summary>
    /// Gets or sets the type of entity being acted upon
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Gets or sets the ID of the entity being acted upon
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method (GET, POST, PUT, DELETE, etc.)
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the request path
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the request
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent string
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the request body (optional, for debugging)
    /// </summary>
    public string? RequestBody { get; set; }

    /// <summary>
    /// Gets or sets the response body (optional, for debugging)
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the duration in milliseconds
    /// </summary>
    public long? DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the log message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exception message if an error occurred
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// Gets or sets the exception stack trace
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the correlation ID for tracing requests across services
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
