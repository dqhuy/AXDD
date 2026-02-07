using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Notification.Api.Application.Services.Interfaces;
using AXDD.Services.Notification.Api.Domain.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AXDD.Services.Notification.Api.Application.Services;

/// <summary>
/// Service for sending emails using MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration configuration,
        INotificationTemplateRepository templateRepository,
        ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _templateRepository = templateRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var useSsl = bool.Parse(_configuration["EmailSettings:UseSsl"] ?? "true");
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@axdd.gov.vn";
            var fromName = _configuration["EmailSettings:FromName"] ?? "AXDD Platform";
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            if (string.IsNullOrEmpty(smtpServer))
            {
                _logger.LogWarning("SMTP server not configured, skipping email send");
                return Result<bool>.Success(false);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, smtpPort, 
                useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, 
                cancellationToken);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return Result<bool>.Failure($"Failed to send email: {ex.Message}");
        }
    }

    public async Task<Result<bool>> SendEmailWithTemplateAsync(
        string to,
        string templateKey,
        Dictionary<string, string> placeholders,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _templateRepository.GetByKeyAsync(templateKey, cancellationToken);
            if (template == null)
            {
                return Result<bool>.Failure($"Template '{templateKey}' not found");
            }

            if (!template.IsActive)
            {
                return Result<bool>.Failure($"Template '{templateKey}' is not active");
            }

            // Replace placeholders in subject and body
            var subject = ReplacePlaceholders(template.Subject, placeholders);
            var body = ReplacePlaceholders(template.BodyTemplate, placeholders);

            return await SendEmailAsync(to, subject, body, true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with template {TemplateKey} to {To}", templateKey, to);
            return Result<bool>.Failure($"Failed to send email with template: {ex.Message}");
        }
    }

    private static string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
    {
        var result = template;
        foreach (var placeholder in placeholders)
        {
            result = result.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }
        return result;
    }
}
