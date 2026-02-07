using AXDD.Services.Notification.Api.Application.Services.Interfaces;
using AXDD.Services.Notification.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AXDD.Services.Notification.Api.Application.Services;

/// <summary>
/// Service for sending real-time notifications via SignalR
/// </summary>
public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationHubService> _logger;

    public NotificationHubService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendToUserAsync(Guid userId, string title, string message)
    {
        try
        {
            var groupName = $"user_{userId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                title,
                message,
                timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Real-time notification sent to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send real-time notification to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendToGroupAsync(string groupName, string title, string message)
    {
        try
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                title,
                message,
                timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Real-time notification sent to group {GroupName}", groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send real-time notification to group {GroupName}", groupName);
            throw;
        }
    }
}
