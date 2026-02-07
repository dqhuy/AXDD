using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Notification.Api.Application.DTOs;

namespace AXDD.Services.Notification.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for notification template management
/// </summary>
public interface INotificationTemplateService
{
    /// <summary>
    /// Gets all notification templates
    /// </summary>
    Task<Result<List<NotificationTemplateDto>>> GetAllTemplatesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a notification template by ID
    /// </summary>
    Task<Result<NotificationTemplateDto>> GetTemplateByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a notification template by key
    /// </summary>
    Task<Result<NotificationTemplateDto>> GetTemplateByKeyAsync(
        string key, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new notification template
    /// </summary>
    Task<Result<NotificationTemplateDto>> CreateTemplateAsync(
        CreateTemplateRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active templates
    /// </summary>
    Task<Result<List<NotificationTemplateDto>>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default);
}
