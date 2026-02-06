using System.Text.RegularExpressions;

namespace AXDD.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Value object representing a Vietnamese phone number
/// </summary>
public sealed record PhoneNumber
{
    private static readonly Regex PhoneRegex = new(@"^(0|\+84)(3|5|7|8|9)\d{8}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the phone number value
    /// </summary>
    public string Value { get; init; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new PhoneNumber instance
    /// </summary>
    /// <param name="phoneNumber">The phone number string</param>
    /// <returns>A PhoneNumber instance</returns>
    /// <exception cref="ArgumentException">Thrown when phone number format is invalid</exception>
    public static PhoneNumber Create(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

        var normalized = NormalizePhoneNumber(phoneNumber);
        
        if (!PhoneRegex.IsMatch(normalized))
        {
            throw new ArgumentException("Invalid Vietnamese phone number format. Expected format: 0912345678 or +84912345678", nameof(phoneNumber));
        }

        return new PhoneNumber(normalized);
    }

    /// <summary>
    /// Tries to create a new PhoneNumber instance
    /// </summary>
    /// <param name="phoneNumber">The phone number string</param>
    /// <param name="result">The created PhoneNumber if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string? phoneNumber, out PhoneNumber? result)
    {
        result = null;
        
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try
        {
            result = Create(phoneNumber);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        // Remove spaces, dashes, and parentheses
        var normalized = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        // Convert +84 to 0
        if (normalized.StartsWith("+84"))
        {
            normalized = "0" + normalized[3..];
        }
        
        return normalized;
    }

    /// <summary>
    /// Gets the phone number in international format (+84...)
    /// </summary>
    public string ToInternationalFormat()
    {
        return Value.StartsWith("+84") ? Value : "+84" + Value[1..];
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
