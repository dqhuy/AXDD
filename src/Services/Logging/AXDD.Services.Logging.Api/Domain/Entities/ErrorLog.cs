using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Domain.Entities;

/// <summary>
/// Represents an error log entry
/// </summary>
public class ErrorLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the name of the service where the error occurred
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exception stack trace
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the error severity
    /// </summary>
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Medium;

    /// <summary>
    /// Gets or sets the user ID associated with the error
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the request path where the error occurred
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// Gets or sets the exception type
    /// </summary>
    public string? ExceptionType { get; set; }

    /// <summary>
    /// Gets or sets whether the error has been resolved
    /// </summary>
    public bool IsResolved { get; set; } = false;

    /// <summary>
    /// Gets or sets the user ID who resolved the error
    /// </summary>
    public Guid? ResolvedBy { get; set; }

    /// <summary>
    /// Gets or sets when the error was resolved
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Gets or sets the resolution description
    /// </summary>
    public string? Resolution { get; set; }

    /// <summary>
    /// Gets or sets the correlation ID
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
