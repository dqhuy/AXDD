using AXDD.Services.Report.Api.Application.DTOs;
using FluentValidation;

namespace AXDD.Services.Report.Api.Application.Validators;

/// <summary>
/// Validator for CreateTemplateRequest
/// </summary>
public class CreateTemplateRequestValidator : AbstractValidator<CreateTemplateRequest>
{
    public CreateTemplateRequestValidator()
    {
        RuleFor(x => x.ReportType)
            .IsInEnum()
            .WithMessage("Invalid report type.");

        RuleFor(x => x.TemplateName)
            .NotEmpty()
            .WithMessage("Template name is required.")
            .MaximumLength(200)
            .WithMessage("Template name must not exceed 200 characters.");

        RuleFor(x => x.FieldsJson)
            .NotEmpty()
            .WithMessage("Fields JSON is required.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Version)
            .GreaterThan(0)
            .WithMessage("Version must be greater than 0.");
    }
}
