using System.Text.RegularExpressions;

namespace AXDD.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Value object representing a Vietnamese Tax Identification Number (Mã số thuế)
/// </summary>
public sealed record TaxCode
{
    private static readonly Regex TaxCodeRegex = new(@"^\d{10}(-\d{3})?$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the tax code value
    /// </summary>
    public string Value { get; init; }

    private TaxCode(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new TaxCode instance
    /// </summary>
    /// <param name="taxCode">The tax code string</param>
    /// <returns>A TaxCode instance</returns>
    /// <exception cref="ArgumentException">Thrown when tax code format is invalid</exception>
    public static TaxCode Create(string taxCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(taxCode, nameof(taxCode));

        var normalized = taxCode.Trim().Replace(" ", "");

        if (!TaxCodeRegex.IsMatch(normalized))
        {
            throw new ArgumentException(
                "Invalid Vietnamese tax code format. Expected format: 0123456789 or 0123456789-001",
                nameof(taxCode));
        }

        return new TaxCode(normalized);
    }

    /// <summary>
    /// Tries to create a new TaxCode instance
    /// </summary>
    /// <param name="taxCode">The tax code string</param>
    /// <param name="result">The created TaxCode if successful</param>
    /// <returns>True if creation was successful, false otherwise</returns>
    public static bool TryCreate(string? taxCode, out TaxCode? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(taxCode))
            return false;

        try
        {
            result = Create(taxCode);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the main tax code (10 digits) without branch code
    /// </summary>
    public string MainCode => Value.Length > 10 ? Value[..10] : Value;

    /// <summary>
    /// Gets the branch code (3 digits) if exists
    /// </summary>
    public string? BranchCode => Value.Length > 10 ? Value[11..] : null;

    /// <summary>
    /// Checks if this is a branch tax code (has branch suffix)
    /// </summary>
    public bool IsBranchCode => Value.Length > 10;

    public override string ToString() => Value;

    public static implicit operator string(TaxCode taxCode) => taxCode.Value;
}
