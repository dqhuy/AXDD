namespace AXDD.Services.Logging.Api.Domain.Enums;

/// <summary>
/// Defines the severity level of a log entry
/// </summary>
public enum AuditLogLevel
{
    /// <summary>
    /// Trace level - very detailed logs
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Debug level - debugging information
    /// </summary>
    Debug = 1,

    /// <summary>
    /// Information level - general informational messages
    /// </summary>
    Info = 2,

    /// <summary>
    /// Warning level - warning messages for potentially harmful situations
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Error level - error messages
    /// </summary>
    Error = 4,

    /// <summary>
    /// Critical level - critical error messages
    /// </summary>
    Critical = 5
}
