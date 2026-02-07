namespace AXDD.Services.Notification.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for real-time SignalR notifications
/// </summary>
public interface INotificationHubService
{
    /// <summary>
    /// Sends a real-time notification to a specific user
    /// </summary>
    Task SendToUserAsync(Guid userId, string title, string message);

    /// <summary>
    /// Sends a real-time notification to a group
    /// </summary>
    Task SendToGroupAsync(string groupName, string title, string message);
}
