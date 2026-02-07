using AXDD.Services.Notification.Api.Application.DTOs;
using FluentValidation;

namespace AXDD.Services.Notification.Api.Application.Validators;

/// <summary>
/// Validator for CreateTemplateRequest
/// </summary>
public class CreateTemplateRequestValidator : AbstractValidator<CreateTemplateRequest>
{
    public CreateTemplateRequestValidator()
    {
        RuleFor(x => x.TemplateKey)
            .NotEmpty()
            .WithMessage("Template key is required")
            .MaximumLength(100)
            .WithMessage("Template key must not exceed 100 characters")
            .Matches("^[A-Z0-9_]+$")
            .WithMessage("Template key must contain only uppercase letters, numbers, and underscores");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject is required")
            .MaximumLength(200)
            .WithMessage("Subject must not exceed 200 characters");

        RuleFor(x => x.BodyTemplate)
            .NotEmpty()
            .WithMessage("Body template is required");

        RuleFor(x => x.ChannelType)
            .IsInEnum()
            .WithMessage("Invalid channel type");
    }
}
