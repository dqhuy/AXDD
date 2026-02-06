using System.Text.RegularExpressions;

namespace AXDD.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Value object representing an email address
/// </summary>
public sealed record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the email address value
    /// </summary>
    public string Value { get; init; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Email instance
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <returns>An Email instance</returns>
    /// <exception cref="ArgumentException">Thrown when email format is invalid</exception>
    public static Email Create(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));

        var normalized = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
        {
            throw new ArgumentException("Invalid email address format", nameof(email));
        }

        if (normalized.Length > 254) // RFC 5321
        {
            throw new ArgumentException("Email address exceeds maximum length of 254 characters", nameof(email));
        }

        return new Email(normalized);
    }

    /// <summary>
    /// Tries to create a new Email instance
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <param name="result">The created Email if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string? email, out Email? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            result = Create(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the domain part of the email address
    /// </summary>
    public string Domain => Value.Split('@')[1];

    /// <summary>
    /// Gets the local part of the email address (before @)
    /// </summary>
    public string LocalPart => Value.Split('@')[0];

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
