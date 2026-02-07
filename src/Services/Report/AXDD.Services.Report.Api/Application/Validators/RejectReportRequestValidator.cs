using AXDD.Services.Report.Api.Application.DTOs;
using FluentValidation;

namespace AXDD.Services.Report.Api.Application.Validators;

/// <summary>
/// Validator for RejectReportRequest
/// </summary>
public class RejectReportRequestValidator : AbstractValidator<RejectReportRequest>
{
    public RejectReportRequestValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .WithMessage("Rejection reason is required.")
            .MaximumLength(1000)
            .WithMessage("Rejection reason must not exceed 1000 characters.");

        RuleFor(x => x.ReviewerNotes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.ReviewerNotes))
            .WithMessage("Reviewer notes must not exceed 2000 characters.");
    }
}
