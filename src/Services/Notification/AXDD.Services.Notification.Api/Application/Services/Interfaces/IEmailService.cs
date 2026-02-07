using AXDD.BuildingBlocks.Common.Results;

namespace AXDD.Services.Notification.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for email operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email to a recipient
    /// </summary>
    Task<Result<bool>> SendEmailAsync(
        string to, 
        string subject, 
        string body, 
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email using a template with placeholders
    /// </summary>
    Task<Result<bool>> SendEmailWithTemplateAsync(
        string to, 
        string templateKey, 
        Dictionary<string, string> placeholders, 
        CancellationToken cancellationToken = default);
}
