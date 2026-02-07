using AXDD.WebApp.Admin.Models.ApiModels;
using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Controller for enterprise management
/// </summary>
[Authorize]
public class EnterpriseController : Controller
{
    private readonly IEnterpriseApiService _enterpriseApiService;
    private readonly ILogger<EnterpriseController> _logger;

    public EnterpriseController(IEnterpriseApiService enterpriseApiService, ILogger<EnterpriseController> logger)
    {
        _enterpriseApiService = enterpriseApiService;
        _logger = logger;
    }

    /// <summary>
    /// List all enterprises
    /// </summary>
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? statusFilter = null,
        string? typeFilter = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _enterpriseApiService.GetEnterprisesAsync(
                pageNumber,
                pageSize,
                searchTerm,
                statusFilter,
                typeFilter,
                cancellationToken);

            var model = new EnterpriseListViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                StatusFilter = statusFilter,
                TypeFilter = typeFilter
            };

            if (response.Success && response.Data != null)
            {
                model.Enterprises = response.Data.Items.Select(e => new EnterpriseItemViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    TaxCode = e.TaxCode,
                    IndustrialZone = e.IndustrialZone,
                    EnterpriseType = e.EnterpriseType,
                    Status = e.Status,
                    CreatedAt = e.CreatedAt
                }).ToList();
                model.TotalCount = response.Data.TotalCount;
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to retrieve enterprises";
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading enterprises list");
            TempData["Error"] = "An error occurred while loading enterprises";
            return View(new EnterpriseListViewModel());
        }
    }

    /// <summary>
    /// Display create enterprise form
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        var model = new EnterpriseFormViewModel
        {
            Status = "Active"
        };
        return View(model);
    }

    /// <summary>
    /// Process create enterprise request
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnterpriseFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var dto = new EnterpriseDto
            {
                Name = model.Name,
                TaxCode = model.TaxCode,
                LegalRepresentative = model.LegalRepresentative,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Website = model.Website,
                IndustrialZone = model.IndustrialZone,
                EnterpriseType = model.EnterpriseType,
                Status = model.Status
            };

            var response = await _enterpriseApiService.CreateEnterpriseAsync(dto, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Enterprise created successfully";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to create enterprise");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating enterprise");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the enterprise");
            return View(model);
        }
    }

    /// <summary>
    /// Display edit enterprise form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _enterpriseApiService.GetEnterpriseByIdAsync(id, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Enterprise not found";
                return RedirectToAction(nameof(Index));
            }

            var model = new EnterpriseFormViewModel
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                TaxCode = response.Data.TaxCode,
                LegalRepresentative = response.Data.LegalRepresentative,
                Address = response.Data.Address,
                Phone = response.Data.Phone,
                Email = response.Data.Email,
                Website = response.Data.Website,
                IndustrialZone = response.Data.IndustrialZone,
                EnterpriseType = response.Data.EnterpriseType,
                Status = response.Data.Status
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading enterprise for edit {EnterpriseId}", id);
            TempData["Error"] = "An error occurred while loading the enterprise";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process edit enterprise request
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EnterpriseFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var dto = new EnterpriseDto
            {
                Id = id,
                Name = model.Name,
                TaxCode = model.TaxCode,
                LegalRepresentative = model.LegalRepresentative,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Website = model.Website,
                IndustrialZone = model.IndustrialZone,
                EnterpriseType = model.EnterpriseType,
                Status = model.Status
            };

            var response = await _enterpriseApiService.UpdateEnterpriseAsync(id, dto, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Enterprise updated successfully";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update enterprise");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enterprise {EnterpriseId}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the enterprise");
            return View(model);
        }
    }

    /// <summary>
    /// Display enterprise details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _enterpriseApiService.GetEnterpriseByIdAsync(id, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Enterprise not found";
                return RedirectToAction(nameof(Index));
            }

            var model = new EnterpriseDetailsViewModel
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                TaxCode = response.Data.TaxCode,
                LegalRepresentative = response.Data.LegalRepresentative,
                Address = response.Data.Address,
                Phone = response.Data.Phone,
                Email = response.Data.Email,
                Website = response.Data.Website,
                IndustrialZone = response.Data.IndustrialZone,
                EnterpriseType = response.Data.EnterpriseType,
                Status = response.Data.Status,
                CreatedAt = response.Data.CreatedAt,
                UpdatedAt = response.Data.UpdatedAt
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading enterprise details {EnterpriseId}", id);
            TempData["Error"] = "An error occurred while loading enterprise details";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Delete enterprise
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _enterpriseApiService.DeleteEnterpriseAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Enterprise deleted successfully";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to delete enterprise";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting enterprise {EnterpriseId}", id);
            TempData["Error"] = "An error occurred while deleting the enterprise";
            return RedirectToAction(nameof(Index));
        }
    }
}
