using AXDD.WebApp.Admin.Models;
using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Home controller for dashboard
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly IEnterpriseApiService _enterpriseApiService;
    private readonly IReportApiService _reportApiService;
    private readonly IDocumentApiService _documentApiService;
    private readonly INotificationApiService _notificationApiService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IEnterpriseApiService enterpriseApiService,
        IReportApiService reportApiService,
        IDocumentApiService documentApiService,
        INotificationApiService notificationApiService,
        ILogger<HomeController> logger)
    {
        _enterpriseApiService = enterpriseApiService;
        _reportApiService = reportApiService;
        _documentApiService = documentApiService;
        _notificationApiService = notificationApiService;
        _logger = logger;
    }

    /// <summary>
    /// Dashboard page
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        try
        {
            var model = new DashboardViewModel();

            // Get enterprise statistics
            var enterprisesResponse = await _enterpriseApiService.GetEnterprisesAsync(
                pageNumber: 1,
                pageSize: 1,
                cancellationToken: cancellationToken);

            if (enterprisesResponse.Success && enterprisesResponse.Data != null)
            {
                model.TotalEnterprises = enterprisesResponse.Data.TotalCount;
            }

            // Get active enterprises count
            var activeEnterprisesResponse = await _enterpriseApiService.GetEnterprisesAsync(
                pageNumber: 1,
                pageSize: 1,
                status: "Active",
                cancellationToken: cancellationToken);

            if (activeEnterprisesResponse.Success && activeEnterprisesResponse.Data != null)
            {
                model.ActiveEnterprises = activeEnterprisesResponse.Data.TotalCount;
            }

            // Get pending reports count
            var pendingReportsResponse = await _reportApiService.GetReportsAsync(
                pageNumber: 1,
                pageSize: 1,
                status: "Pending",
                cancellationToken: cancellationToken);

            if (pendingReportsResponse.Success && pendingReportsResponse.Data != null)
            {
                model.PendingReports = pendingReportsResponse.Data.TotalCount;
            }

            // Get total documents count
            var documentsResponse = await _documentApiService.GetDocumentsAsync(
                pageNumber: 1,
                pageSize: 1,
                cancellationToken: cancellationToken);

            if (documentsResponse.Success && documentsResponse.Data != null)
            {
                model.TotalDocuments = documentsResponse.Data.TotalCount;
            }

            // Get unread notifications count
            var unreadCountResponse = await _notificationApiService.GetUnreadCountAsync(cancellationToken);
            if (unreadCountResponse.Success)
            {
                model.UnreadNotifications = unreadCountResponse.Data;
            }

            // Get report statistics for charts
            var reportStatsResponse = await _reportApiService.GetReportStatisticsAsync(cancellationToken);
            if (reportStatsResponse.Success && reportStatsResponse.Data != null)
            {
                model.ReportsByStatus = reportStatsResponse.Data
                    .Select(r => new ReportStatusChart { Status = r.Status, Count = r.Count })
                    .ToList();
            }

            // TODO: Replace mock data with actual API call to get enterprise type distribution
            // Example: var enterpriseTypes = await _enterpriseApiService.GetTypeDistributionAsync();
            // For now, using mock data for demonstration
            model.EnterprisesByType = new List<EnterpriseTypeChart>
            {
                new() { Type = "Manufacturing", Count = 45 },
                new() { Type = "Service", Count = 32 },
                new() { Type = "Trading", Count = 28 },
                new() { Type = "Technology", Count = 19 }
            };

            // TODO: Replace mock data with actual API call to get recent activities
            // Example: var activities = await _activityLogService.GetRecentActivitiesAsync(limit: 5);
            // For now, using mock data for demonstration
            model.RecentActivities = new List<RecentActivityItem>
            {
                new()
                {
                    Description = "New enterprise registered",
                    Timestamp = DateTime.Now.AddMinutes(-15),
                    Icon = "fas fa-building",
                    Type = "enterprise"
                },
                new()
                {
                    Description = "Report submitted for review",
                    Timestamp = DateTime.Now.AddHours(-2),
                    Icon = "fas fa-file-alt",
                    Type = "report"
                },
                new()
                {
                    Description = "Document uploaded",
                    Timestamp = DateTime.Now.AddHours(-5),
                    Icon = "fas fa-file-upload",
                    Type = "document"
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View(new DashboardViewModel());
        }
    }

    /// <summary>
    /// Error page
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
