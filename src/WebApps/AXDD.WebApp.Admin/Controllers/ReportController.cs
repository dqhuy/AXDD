using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Controller for report management
/// </summary>
[Authorize]
public class ReportController : Controller
{
    private readonly IReportApiService _reportApiService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportApiService reportApiService, ILogger<ReportController> logger)
    {
        _reportApiService = reportApiService;
        _logger = logger;
    }

    /// <summary>
    /// List all reports
    /// </summary>
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        string? statusFilter = null,
        string? typeFilter = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _reportApiService.GetReportsAsync(
                pageNumber,
                pageSize,
                statusFilter,
                typeFilter,
                cancellationToken);

            var model = new ReportListViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                StatusFilter = statusFilter,
                TypeFilter = typeFilter
            };

            if (response.Success && response.Data != null)
            {
                model.Reports = response.Data.Items.Select(r => new ReportItemViewModel
                {
                    Id = r.Id,
                    EnterpriseId = r.EnterpriseId,
                    EnterpriseName = r.EnterpriseName,
                    ReportType = r.ReportType,
                    Year = r.Year,
                    Quarter = r.Quarter,
                    Month = r.Month,
                    Status = r.Status,
                    SubmittedAt = r.SubmittedAt
                }).ToList();
                model.TotalCount = response.Data.TotalCount;
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to retrieve reports";
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports list");
            TempData["Error"] = "An error occurred while loading reports";
            return View(new ReportListViewModel());
        }
    }

    /// <summary>
    /// Display report details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _reportApiService.GetReportByIdAsync(id, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Report not found";
                return RedirectToAction(nameof(Index));
            }

            var model = new ReportDetailsViewModel
            {
                Id = response.Data.Id,
                EnterpriseId = response.Data.EnterpriseId,
                EnterpriseName = response.Data.EnterpriseName,
                ReportType = response.Data.ReportType,
                Year = response.Data.Year,
                Quarter = response.Data.Quarter,
                Month = response.Data.Month,
                Status = response.Data.Status,
                SubmittedAt = response.Data.SubmittedAt,
                ReviewedAt = response.Data.ReviewedAt,
                ReviewComments = response.Data.ReviewComments,
                Data = response.Data.Data
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report details {ReportId}", id);
            TempData["Error"] = "An error occurred while loading report details";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display approve report form
    /// </summary>
    [HttpGet]
    public IActionResult Approve(Guid id)
    {
        var model = new ReportApprovalViewModel
        {
            ReportId = id,
            Approve = true
        };
        return View(model);
    }

    /// <summary>
    /// Process approve report
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(ReportApprovalViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = model.Approve
                ? await _reportApiService.ApproveReportAsync(model.ReportId, model.Comments, cancellationToken)
                : await _reportApiService.RejectReportAsync(model.ReportId, model.Comments, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = model.Approve ? "Report approved successfully" : "Report rejected successfully";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to process report");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing report {ReportId}", model.ReportId);
            ModelState.AddModelError(string.Empty, "An error occurred while processing the report");
            return View(model);
        }
    }

    /// <summary>
    /// Reject report action
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, string comments, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _reportApiService.RejectReportAsync(id, comments, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Report rejected successfully";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to reject report";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting report {ReportId}", id);
            TempData["Error"] = "An error occurred while rejecting the report";
            return RedirectToAction(nameof(Index));
        }
    }
}
