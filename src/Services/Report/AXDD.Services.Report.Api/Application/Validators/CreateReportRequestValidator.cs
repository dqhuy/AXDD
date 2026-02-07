using AXDD.Services.Report.Api.Application.DTOs;
using AXDD.Services.Report.Api.Domain.Enums;
using FluentValidation;

namespace AXDD.Services.Report.Api.Application.Validators;

/// <summary>
/// Validator for CreateReportRequest
/// </summary>
public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.EnterpriseId)
            .NotEmpty()
            .WithMessage("Enterprise ID is required.");

        RuleFor(x => x.EnterpriseName)
            .NotEmpty()
            .WithMessage("Enterprise name is required.")
            .MaximumLength(200)
            .WithMessage("Enterprise name must not exceed 200 characters.");

        RuleFor(x => x.EnterpriseCode)
            .NotEmpty()
            .WithMessage("Enterprise code is required.")
            .MaximumLength(50)
            .WithMessage("Enterprise code must not exceed 50 characters.");

        RuleFor(x => x.ReportType)
            .IsInEnum()
            .WithMessage("Invalid report type.");

        RuleFor(x => x.ReportPeriod)
            .IsInEnum()
            .WithMessage("Invalid report period.");

        RuleFor(x => x.Year)
            .GreaterThan(2000)
            .WithMessage("Year must be greater than 2000.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
            .WithMessage($"Year must not exceed {DateTime.UtcNow.Year + 1}.");

        RuleFor(x => x.Month)
            .Must((request, month) => ValidateMonth(request.ReportPeriod, month))
            .WithMessage("Month is required for monthly reports and must be between 1 and 12.");

        RuleFor(x => x.DataJson)
            .NotEmpty()
            .WithMessage("Report data is required.");
    }

    private static bool ValidateMonth(ReportPeriod reportPeriod, int? month)
    {
        if (reportPeriod == ReportPeriod.Monthly)
        {
            return month.HasValue && month.Value >= 1 && month.Value <= 12;
        }

        // For non-monthly reports, month should be null
        return !month.HasValue;
    }
}
