namespace AXDD.Services.Logging.Api.Domain.Exceptions;

/// <summary>
/// Exception thrown when a log entry is not found
/// </summary>
public class LogNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogNotFoundException"/> class
    /// </summary>
    /// <param name="logId">The ID of the log that was not found</param>
    public LogNotFoundException(Guid logId)
        : base($"Log with ID '{logId}' was not found.")
    {
        LogId = logId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogNotFoundException"/> class
    /// </summary>
    /// <param name="message">The exception message</param>
    public LogNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Gets the ID of the log that was not found
    /// </summary>
    public Guid? LogId { get; }
}
