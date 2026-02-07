using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Controller for document management
/// </summary>
[Authorize]
public class DocumentController : Controller
{
    private readonly IDocumentApiService _documentApiService;
    private readonly IEnterpriseApiService _enterpriseApiService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(
        IDocumentApiService documentApiService,
        IEnterpriseApiService enterpriseApiService,
        ILogger<DocumentController> logger)
    {
        _documentApiService = documentApiService;
        _enterpriseApiService = enterpriseApiService;
        _logger = logger;
    }

    /// <summary>
    /// List all documents
    /// </summary>
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        Guid? enterpriseId = null,
        string? documentType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _documentApiService.GetDocumentsAsync(
                pageNumber,
                pageSize,
                enterpriseId,
                documentType,
                cancellationToken);

            var model = new DocumentListViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                EnterpriseId = enterpriseId,
                DocumentType = documentType
            };

            if (response.Success && response.Data != null)
            {
                model.Documents = response.Data.Items.Select(d => new DocumentItemViewModel
                {
                    Id = d.Id,
                    EnterpriseId = d.EnterpriseId,
                    FileName = d.FileName,
                    FileType = d.FileType,
                    FileSize = d.FileSize,
                    DocumentType = d.DocumentType,
                    Description = d.Description,
                    UploadedAt = d.UploadedAt
                }).ToList();
                model.TotalCount = response.Data.TotalCount;
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to retrieve documents";
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading documents list");
            TempData["Error"] = "An error occurred while loading documents";
            return View(new DocumentListViewModel());
        }
    }

    /// <summary>
    /// Display upload document form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Upload(CancellationToken cancellationToken = default)
    {
        try
        {
            var model = new DocumentUploadViewModel();

            // Load enterprises for dropdown
            var enterprisesResponse = await _enterpriseApiService.GetEnterprisesAsync(
                pageNumber: 1,
                pageSize: 100,
                status: "Active",
                cancellationToken: cancellationToken);

            if (enterprisesResponse.Success && enterprisesResponse.Data != null)
            {
                model.Enterprises = enterprisesResponse.Data.Items
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Name} ({e.TaxCode})"
                    })
                    .ToList();
            }

            // Document types
            model.DocumentTypes = new List<SelectListItem>
            {
                new() { Value = "BusinessLicense", Text = "Business License" },
                new() { Value = "TaxRegistration", Text = "Tax Registration" },
                new() { Value = "Certificate", Text = "Certificate" },
                new() { Value = "Report", Text = "Report" },
                new() { Value = "Contract", Text = "Contract" },
                new() { Value = "Other", Text = "Other" }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading upload form");
            TempData["Error"] = "An error occurred while loading the upload form";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process document upload
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(DocumentUploadViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            // Reload dropdowns
            var enterprisesResponse = await _enterpriseApiService.GetEnterprisesAsync(
                pageNumber: 1,
                pageSize: 100,
                status: "Active",
                cancellationToken: cancellationToken);

            if (enterprisesResponse.Success && enterprisesResponse.Data != null)
            {
                model.Enterprises = enterprisesResponse.Data.Items
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = $"{e.Name} ({e.TaxCode})"
                    })
                    .ToList();
            }

            model.DocumentTypes = new List<SelectListItem>
            {
                new() { Value = "BusinessLicense", Text = "Business License" },
                new() { Value = "TaxRegistration", Text = "Tax Registration" },
                new() { Value = "Certificate", Text = "Certificate" },
                new() { Value = "Report", Text = "Report" },
                new() { Value = "Contract", Text = "Contract" },
                new() { Value = "Other", Text = "Other" }
            };

            return View(model);
        }

        try
        {
            // Validate file
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload");
                return View(model);
            }

            // Check file size (max 10MB)
            if (model.File.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("File", "File size cannot exceed 10MB");
                return View(model);
            }

            var response = await _documentApiService.UploadDocumentAsync(
                model.EnterpriseId,
                model.File,
                model.DocumentType,
                model.Description,
                cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Document uploaded successfully";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to upload document");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            ModelState.AddModelError(string.Empty, "An error occurred while uploading the document");
            return View(model);
        }
    }

    /// <summary>
    /// Download document
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get document info first
            var docResponse = await _documentApiService.GetDocumentByIdAsync(id, cancellationToken);
            if (!docResponse.Success || docResponse.Data == null)
            {
                TempData["Error"] = "Document not found";
                return RedirectToAction(nameof(Index));
            }

            // Download file
            var fileBytes = await _documentApiService.DownloadDocumentAsync(id, cancellationToken);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                TempData["Error"] = "Failed to download document";
                return RedirectToAction(nameof(Index));
            }

            return File(fileBytes, "application/octet-stream", docResponse.Data.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", id);
            TempData["Error"] = "An error occurred while downloading the document";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Delete document
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _documentApiService.DeleteDocumentAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Document deleted successfully";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Failed to delete document";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            TempData["Error"] = "An error occurred while deleting the document";
            return RedirectToAction(nameof(Index));
        }
    }
}
