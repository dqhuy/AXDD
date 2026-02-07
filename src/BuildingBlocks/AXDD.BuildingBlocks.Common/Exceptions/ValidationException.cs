namespace AXDD.BuildingBlocks.Common.Exceptions;

/// <summary>
/// Exception thrown when a validation error occurs
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationException class
    /// </summary>
    public ValidationException()
        : base("One or more validation errors occurred")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the ValidationException class with validation errors
    /// </summary>
    /// <param name="errors">The validation errors</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    /// <summary>
    /// Initializes a new instance of the ValidationException class with a single validation error
    /// </summary>
    /// <param name="field">The field name</param>
    /// <param name="error">The error message</param>
    public ValidationException(string field, string error)
        : base("One or more validation errors occurred")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }
}
