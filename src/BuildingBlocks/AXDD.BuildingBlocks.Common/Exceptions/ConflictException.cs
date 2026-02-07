namespace AXDD.BuildingBlocks.Common.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate resource)
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ConflictException class
    /// </summary>
    /// <param name="message">The error message</param>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConflictException class
    /// </summary>
    /// <param name="resourceName">The name of the resource</param>
    /// <param name="key">The key of the resource that caused the conflict</param>
    public ConflictException(string resourceName, object key)
        : base($"Resource \"{resourceName}\" with key ({key}) already exists")
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConflictException class with an inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
