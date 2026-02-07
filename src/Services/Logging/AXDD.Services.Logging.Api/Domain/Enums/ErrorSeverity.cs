namespace AXDD.Services.Logging.Api.Domain.Enums;

/// <summary>
/// Defines the severity level of an error
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Low severity - minor issues
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium severity - moderate issues
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High severity - serious issues
    /// </summary>
    High = 2,

    /// <summary>
    /// Critical severity - critical system failures
    /// </summary>
    Critical = 3
}
