using AXDD.Services.Enterprise.Api.Application.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AXDD.Services.Enterprise.Api.Application.Validators;

/// <summary>
/// Validator for CreateEnterpriseRequest
/// </summary>
public partial class CreateEnterpriseRequestValidator : AbstractValidator<CreateEnterpriseRequest>
{
    public CreateEnterpriseRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Enterprise code is required")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Code must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Enterprise name is required")
            .MaximumLength(500).WithMessage("Name must not exceed 500 characters");

        RuleFor(x => x.TaxCode)
            .NotEmpty().WithMessage("Tax code is required")
            .Must(BeValidTaxCode).WithMessage("Tax code must be 10 or 13 digits")
            .Matches(@"^\d{10}$|^\d{13}$").WithMessage("Tax code must contain only digits");

        RuleFor(x => x.IndustryCode)
            .NotEmpty().WithMessage("Industry code is required")
            .MaximumLength(20).WithMessage("Industry code must not exceed 20 characters");

        RuleFor(x => x.IndustryName)
            .NotEmpty().WithMessage("Industry name is required")
            .MaximumLength(500).WithMessage("Industry name must not exceed 500 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(1000).WithMessage("Address must not exceed 1000 characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .Must(BeValidPhoneNumber).WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Website)
            .Must(BeValidUrl).WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrWhiteSpace(x.Website));

        RuleFor(x => x.RegisteredCapital)
            .GreaterThan(0).WithMessage("Registered capital must be greater than 0")
            .When(x => x.RegisteredCapital.HasValue);

        RuleFor(x => x.CharterCapital)
            .GreaterThan(0).WithMessage("Charter capital must be greater than 0")
            .When(x => x.CharterCapital.HasValue);

        RuleFor(x => x.TotalEmployees)
            .GreaterThanOrEqualTo(0).WithMessage("Total employees must be 0 or greater")
            .When(x => x.TotalEmployees.HasValue);

        RuleFor(x => x.VietnamEmployees)
            .GreaterThanOrEqualTo(0).WithMessage("Vietnam employees must be 0 or greater")
            .LessThanOrEqualTo(x => x.TotalEmployees ?? int.MaxValue)
            .WithMessage("Vietnam employees cannot exceed total employees")
            .When(x => x.VietnamEmployees.HasValue);

        RuleFor(x => x.ForeignEmployees)
            .GreaterThanOrEqualTo(0).WithMessage("Foreign employees must be 0 or greater")
            .LessThanOrEqualTo(x => x.TotalEmployees ?? int.MaxValue)
            .WithMessage("Foreign employees cannot exceed total employees")
            .When(x => x.ForeignEmployees.HasValue);
    }

    private static bool BeValidTaxCode(string taxCode)
    {
        if (string.IsNullOrWhiteSpace(taxCode))
            return false;

        return taxCode.Length == 10 || taxCode.Length == 13;
    }

    private static bool BeValidPhoneNumber(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;

        // Vietnamese phone number pattern
        return PhoneRegex().IsMatch(phone);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    [GeneratedRegex(@"^[\d\-\+\(\)\s]+$")]
    private static partial Regex PhoneRegex();
}
