using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Notification.Api.Application.DTOs;
using AXDD.Services.Notification.Api.Application.Services.Interfaces;
using AXDD.Services.Notification.Api.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AXDD.Services.Notification.Api.Application.Services;

/// <summary>
/// Service for notification template management
/// </summary>
public class NotificationTemplateService : INotificationTemplateService
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationTemplateService> _logger;

    public NotificationTemplateService(
        INotificationTemplateRepository templateRepository,
        IUnitOfWork unitOfWork,
        ILogger<NotificationTemplateService> logger)
    {
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<NotificationTemplateDto>>> GetAllTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var templates = await _templateRepository.GetAllAsync(cancellationToken);
            var dtos = templates.Select(MapToDto).ToList();
            return Result<List<NotificationTemplateDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all notification templates");
            return Result<List<NotificationTemplateDto>>.Failure($"Failed to get templates: {ex.Message}");
        }
    }

    public async Task<Result<NotificationTemplateDto>> GetTemplateByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _templateRepository.GetByIdAsync(id, cancellationToken);
            if (template == null)
            {
                return Result<NotificationTemplateDto>.Failure("Template not found");
            }

            return Result<NotificationTemplateDto>.Success(MapToDto(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {Id}", id);
            return Result<NotificationTemplateDto>.Failure($"Failed to get template: {ex.Message}");
        }
    }

    public async Task<Result<NotificationTemplateDto>> GetTemplateByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _templateRepository.GetByKeyAsync(key, cancellationToken);
            if (template == null)
            {
                return Result<NotificationTemplateDto>.Failure($"Template with key '{key}' not found");
            }

            return Result<NotificationTemplateDto>.Success(MapToDto(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template by key {Key}", key);
            return Result<NotificationTemplateDto>.Failure($"Failed to get template: {ex.Message}");
        }
    }

    public async Task<Result<NotificationTemplateDto>> CreateTemplateAsync(
        CreateTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if template key already exists
            var existing = await _templateRepository.GetByKeyAsync(request.TemplateKey, cancellationToken);
            if (existing != null)
            {
                return Result<NotificationTemplateDto>.Failure($"Template with key '{request.TemplateKey}' already exists");
            }

            var template = new NotificationTemplate
            {
                TemplateKey = request.TemplateKey,
                Subject = request.Subject,
                BodyTemplate = request.BodyTemplate,
                ChannelType = request.ChannelType,
                IsActive = request.IsActive,
                Description = request.Description
            };

            await _templateRepository.AddAsync(template, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<NotificationTemplateDto>.Success(MapToDto(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification template");
            return Result<NotificationTemplateDto>.Failure($"Failed to create template: {ex.Message}");
        }
    }

    public async Task<Result<List<NotificationTemplateDto>>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var templates = await _templateRepository.GetActiveTemplatesAsync(cancellationToken);
            var dtos = templates.Select(MapToDto).ToList();
            return Result<List<NotificationTemplateDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active notification templates");
            return Result<List<NotificationTemplateDto>>.Failure($"Failed to get active templates: {ex.Message}");
        }
    }

    private static NotificationTemplateDto MapToDto(NotificationTemplate entity)
    {
        return new NotificationTemplateDto
        {
            Id = entity.Id,
            TemplateKey = entity.TemplateKey,
            Subject = entity.Subject,
            BodyTemplate = entity.BodyTemplate,
            ChannelType = entity.ChannelType,
            IsActive = entity.IsActive,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
