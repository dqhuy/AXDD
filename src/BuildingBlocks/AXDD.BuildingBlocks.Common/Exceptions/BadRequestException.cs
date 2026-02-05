namespace AXDD.BuildingBlocks.Common.Exceptions;

/// <summary>
/// Exception thrown when a bad request is made
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public IEnumerable<string>? Errors { get; set; }
}
