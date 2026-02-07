using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Notification.Api.Application.DTOs;
using AXDD.Services.Notification.Api.Application.Services.Interfaces;
using AXDD.Services.Notification.Api.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AXDD.Services.Notification.Api.Application.Services;

/// <summary>
/// Service for notification management
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly INotificationHubService _hubService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        INotificationHubService hubService,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _hubService = hubService;
        _logger = logger;
    }

    public async Task<Result<NotificationDto>> SendNotificationAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create notification entity
            var notification = new NotificationEntity
            {
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                IsRead = false,
                RelatedEntityType = request.RelatedEntityType,
                RelatedEntityId = request.RelatedEntityId,
                ActionUrl = request.ActionUrl,
                Data = request.Data
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send real-time notification via SignalR
            try
            {
                await _hubService.SendToUserAsync(request.UserId, request.Title, request.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send real-time notification to user {UserId}", request.UserId);
            }

            // Send email if requested
            if (request.SendEmail)
            {
                try
                {
                    // TODO: Get user email from user service
                    // For now, we skip email sending if user email is not provided
                    _logger.LogInformation("Email notification requested but user email lookup not implemented");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send email notification to user {UserId}", request.UserId);
                }
            }

            return Result<NotificationDto>.Success(MapToDto(notification));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", request.UserId);
            return Result<NotificationDto>.Failure($"Failed to send notification: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<NotificationListDto>>> GetMyNotificationsAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedResult = await _notificationRepository.GetByUserIdAsync(
                userId, pageNumber, pageSize, cancellationToken);

            var dtos = pagedResult.Items.Select(MapToListDto).ToList();

            var result = new PagedResult<NotificationListDto>
            {
                Items = dtos,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount
            };

            return Result<PagedResult<NotificationListDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
            return Result<PagedResult<NotificationListDto>>.Failure($"Failed to get notifications: {ex.Message}");
        }
    }

    public async Task<Result<NotificationDto>> GetNotificationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
            if (notification == null)
            {
                return Result<NotificationDto>.Failure("Notification not found");
            }

            return Result<NotificationDto>.Success(MapToDto(notification));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {Id}", id);
            return Result<NotificationDto>.Failure($"Failed to get notification: {ex.Message}");
        }
    }

    public async Task<Result<bool>> MarkAsReadAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _notificationRepository.MarkAsReadAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {Id} as read", id);
            return Result<bool>.Failure($"Failed to mark notification as read: {ex.Message}");
        }
    }

    public async Task<Result<bool>> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            return Result<bool>.Failure($"Failed to mark all notifications as read: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteNotificationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
            if (notification == null)
            {
                return Result<bool>.Failure("Notification not found");
            }

            _notificationRepository.Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {Id}", id);
            return Result<bool>.Failure($"Failed to delete notification: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
            return Result<int>.Failure($"Failed to get unread count: {ex.Message}");
        }
    }

    private static NotificationDto MapToDto(NotificationEntity entity)
    {
        return new NotificationDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Title = entity.Title,
            Message = entity.Message,
            Type = entity.Type,
            IsRead = entity.IsRead,
            ReadAt = entity.ReadAt,
            RelatedEntityType = entity.RelatedEntityType,
            RelatedEntityId = entity.RelatedEntityId,
            ActionUrl = entity.ActionUrl,
            Data = entity.Data,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static NotificationListDto MapToListDto(NotificationEntity entity)
    {
        return new NotificationListDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Message = entity.Message,
            Type = entity.Type,
            IsRead = entity.IsRead,
            RelatedEntityType = entity.RelatedEntityType,
            RelatedEntityId = entity.RelatedEntityId,
            ActionUrl = entity.ActionUrl,
            CreatedAt = entity.CreatedAt
        };
    }
}
