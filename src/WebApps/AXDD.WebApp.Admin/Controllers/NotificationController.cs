using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Controller for notification management
/// </summary>
[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationApiService _notificationApiService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationApiService notificationApiService, ILogger<NotificationController> logger)
    {
        _notificationApiService = notificationApiService;
        _logger = logger;
    }

    /// <summary>
    /// List all notifications
    /// </summary>
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _notificationApiService.GetNotificationsAsync(
                pageNumber,
                pageSize,
                null,
                cancellationToken);

            var model = new NotificationListViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (response.Success && response.Data != null)
            {
                model.Notifications = response.Data.Items.Select(n => new NotificationItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ActionUrl = n.ActionUrl
                }).ToList();
                model.TotalCount = response.Data.TotalCount;
            }

            // Get unread count
            var unreadResponse = await _notificationApiService.GetUnreadCountAsync(cancellationToken);
            if (unreadResponse.Success)
            {
                model.UnreadCount = unreadResponse.Data;
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications list");
            TempData["Error"] = "An error occurred while loading notifications";
            return View(new NotificationListViewModel());
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _notificationApiService.MarkAsReadAsync(id, cancellationToken);

            if (response.Success)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = response.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _notificationApiService.MarkAllAsReadAsync(cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "All notifications marked as read";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = response.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get unread notification count (for AJAX)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _notificationApiService.GetUnreadCountAsync(cancellationToken);

            if (response.Success)
            {
                return Json(new { success = true, count = response.Data });
            }

            return Json(new { success = false, message = response.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _notificationApiService.DeleteNotificationAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Notification deleted successfully";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to delete notification";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            TempData["Error"] = "An error occurred while deleting the notification";
            return RedirectToAction(nameof(Index));
        }
    }
}
