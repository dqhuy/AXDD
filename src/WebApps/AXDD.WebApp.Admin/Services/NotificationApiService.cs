using AXDD.WebApp.Admin.Models.ApiModels;
using System.Web;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Interface for notification API service
/// </summary>
public interface INotificationApiService
{
    /// <summary>
    /// Get notifications with pagination
    /// </summary>
    Task<ApiResponse<PagedResult<NotificationDto>>> GetNotificationsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        bool? isRead = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get unread notification count
    /// </summary>
    Task<ApiResponse<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark notification as read
    /// </summary>
    Task<ApiResponse<bool>> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    Task<ApiResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete notification
    /// </summary>
    Task<ApiResponse<bool>> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Notification API service implementation
/// </summary>
public class NotificationApiService : INotificationApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<NotificationApiService> _logger;

    public NotificationApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<NotificationApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.AddBearerToken(token);
        }
    }

    public async Task<ApiResponse<PagedResult<NotificationDto>>> GetNotificationsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();
            if (isRead.HasValue) queryParams["isRead"] = isRead.Value.ToString();

            var url = $"/api/v1/notifications?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<NotificationDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<NotificationDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return new ApiResponse<PagedResult<NotificationDto>>
            {
                Success = false,
                Message = "Unable to retrieve notifications",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<int>>("/api/v1/notifications/unread-count", cancellationToken);

            return response ?? new ApiResponse<int>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return new ApiResponse<int>
            {
                Success = false,
                Message = "Unable to retrieve unread count",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync<object, ApiResponse<bool>>(
                $"/api/v1/notifications/{id}/mark-read",
                new { },
                cancellationToken);

            return response ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to mark notification as read",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync<object, ApiResponse<bool>>(
                "/api/v1/notifications/mark-all-read",
                new { },
                cancellationToken);

            return response ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to mark all notifications as read",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var success = await _httpClient.DeleteAsyncWithResult($"/api/v1/notifications/{id}", cancellationToken);

            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Notification deleted successfully" : "Failed to delete notification"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to delete notification",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
