using AXDD.Services.Notification.Api.Application.DTOs;
using FluentValidation;

namespace AXDD.Services.Notification.Api.Application.Validators;

/// <summary>
/// Validator for SendNotificationRequest
/// </summary>
public class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
{
    public SendNotificationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .MaximumLength(2000)
            .WithMessage("Message must not exceed 2000 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid notification type");

        When(x => !string.IsNullOrEmpty(x.RelatedEntityType), () =>
        {
            RuleFor(x => x.RelatedEntityType)
                .MaximumLength(100)
                .WithMessage("Related entity type must not exceed 100 characters");
        });

        When(x => !string.IsNullOrEmpty(x.ActionUrl), () =>
        {
            RuleFor(x => x.ActionUrl)
                .MaximumLength(500)
                .WithMessage("Action URL must not exceed 500 characters");
        });
    }
}
