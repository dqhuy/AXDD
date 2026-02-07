using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AXDD.BuildingBlocks.Common.Validation;

/// <summary>
/// Validates Vietnamese phone numbers
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class VietnamesePhoneNumberAttribute : ValidationAttribute
{
    private static readonly Regex PhoneRegex = new(@"^(0|\+84)(3|5|7|8|9)\d{8}$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the VietnamesePhoneNumberAttribute class
    /// </summary>
    public VietnamesePhoneNumberAttribute()
        : base("Invalid Vietnamese phone number format. Expected format: 0912345678 or +84912345678")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var phoneNumber = value.ToString()!;
        var normalized = NormalizePhoneNumber(phoneNumber);

        if (!PhoneRegex.IsMatch(normalized))
        {
            return new ValidationResult(
                ErrorMessage ?? "Invalid Vietnamese phone number format",
                new[] { validationContext.MemberName ?? "PhoneNumber" });
        }

        return ValidationResult.Success;
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        var normalized = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        if (normalized.StartsWith("+84"))
        {
            normalized = "0" + normalized[3..];
        }
        
        return normalized;
    }
}
