using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AXDD.BuildingBlocks.Common.Validation;

/// <summary>
/// Validates Vietnamese Tax Identification Numbers (Mã số thuế)
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class TaxCodeAttribute : ValidationAttribute
{
    private static readonly Regex TaxCodeRegex = new(@"^\d{10}(-\d{3})?$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the TaxCodeAttribute class
    /// </summary>
    public TaxCodeAttribute()
        : base("Invalid Vietnamese tax code format. Expected format: 0123456789 or 0123456789-001")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var taxCode = value.ToString()!;
        var normalized = taxCode.Trim().Replace(" ", "");

        if (!TaxCodeRegex.IsMatch(normalized))
        {
            return new ValidationResult(
                ErrorMessage ?? "Invalid Vietnamese tax code format",
                new[] { validationContext.MemberName ?? "TaxCode" });
        }

        return ValidationResult.Success;
    }
}
