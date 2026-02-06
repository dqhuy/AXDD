namespace AXDD.BuildingBlocks.Common.Results;

/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
/// <typeparam name="T">Type of the result value</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the result value if successful
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error message if failed
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the collection of validation errors if any
    /// </summary>
    public IReadOnlyCollection<string>? Errors { get; }

    private Result(bool isSuccess, T? value, string? error, IReadOnlyCollection<string>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful result with a value
    /// </summary>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null, null);
    }

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    public static Result<T> Failure(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error, nameof(error));
        return new Result<T>(false, default, error, null);
    }

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result<T> Failure(IReadOnlyCollection<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));
        
        if (errors.Count == 0)
        {
            throw new ArgumentException("At least one error is required", nameof(errors));
        }

        return new Result<T>(false, default, errors.First(), errors);
    }

    /// <summary>
    /// Executes an action based on success or failure
    /// </summary>
    public Result<T> Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (IsSuccess && Value != null)
        {
            onSuccess(Value);
        }
        else if (Error != null)
        {
            onFailure(Error);
        }

        return this;
    }

    /// <summary>
    /// Maps the result value to a new type
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess && Value != null
            ? Result<TNew>.Success(mapper(Value))
            : Result<TNew>.Failure(Error ?? "Unknown error");
    }
}

/// <summary>
/// Represents the result of an operation without a return value
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if failed
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the collection of validation errors if any
    /// </summary>
    public IReadOnlyCollection<string>? Errors { get; }

    private Result(bool isSuccess, string? error, IReadOnlyCollection<string>? errors)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success()
    {
        return new Result(true, null, null);
    }

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    public static Result Failure(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error, nameof(error));
        return new Result(false, error, null);
    }

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result Failure(IReadOnlyCollection<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));
        
        if (errors.Count == 0)
        {
            throw new ArgumentException("At least one error is required", nameof(errors));
        }

        return new Result(false, errors.First(), errors);
    }
}
