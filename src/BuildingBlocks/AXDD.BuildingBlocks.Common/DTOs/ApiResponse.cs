namespace AXDD.BuildingBlocks.Common.DTOs;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets the collection of error messages
    /// </summary>
    public IReadOnlyList<string>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the response
    /// </summary>
    public IDictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="message">The success message</param>
    /// <param name="statusCode">The HTTP status code (default: 200)</param>
    /// <returns>A successful API response</returns>
    public static ApiResponse<T> Success(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">Additional error details</param>
    /// <param name="statusCode">The HTTP status code (default: 400)</param>
    /// <returns>An error API response</returns>
    public static ApiResponse<T> Failure(string message, IReadOnlyList<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>A not found API response</returns>
    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        return Failure(message, null, 404);
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>A validation error API response</returns>
    public static ApiResponse<T> ValidationError(IDictionary<string, string[]> errors)
    {
        var errorMessages = errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")).ToList();
        return Failure("Validation failed", errorMessages, 400);
    }

    // Legacy methods for backward compatibility
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success") => Success(data, message);
    public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string>? errors = null) => Failure(message, errors?.ToList());
}

/// <summary>
/// Standard API response wrapper without data
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of error messages
    /// </summary>
    public IReadOnlyList<string>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the response
    /// </summary>
    public IDictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Creates a successful response without data
    /// </summary>
    /// <param name="message">The success message</param>
    /// <param name="statusCode">The HTTP status code (default: 200)</param>
    /// <returns>A successful API response</returns>
    public static ApiResponse Success(string message = "Success", int statusCode = 200)
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response without data
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errors">Additional error details</param>
    /// <param name="statusCode">The HTTP status code (default: 400)</param>
    /// <returns>An error API response</returns>
    public static ApiResponse Failure(string message, IReadOnlyList<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>A not found API response</returns>
    public static ApiResponse NotFound(string message = "Resource not found")
    {
        return Failure(message, null, 404);
    }
}


