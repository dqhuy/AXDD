namespace AXDD.BuildingBlocks.Common.Exceptions;

/// <summary>
/// Exception thrown when access to a resource is forbidden
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ForbiddenException class
    /// </summary>
    public ForbiddenException()
        : base("Access to this resource is forbidden")
    {
    }

    /// <summary>
    /// Initializes a new instance of the ForbiddenException class
    /// </summary>
    /// <param name="message">The error message</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ForbiddenException class with an inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
