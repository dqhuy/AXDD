using AXDD.WebApp.Admin.Models.ApiModels;
using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Controller for Document Profile management (Hồ sơ tài liệu)
/// </summary>
[Authorize]
public class DocumentProfileController : Controller
{
    private readonly IDocumentProfileApiService _profileApiService;
    private readonly IEnterpriseApiService _enterpriseApiService;
    private readonly ILogger<DocumentProfileController> _logger;

    public DocumentProfileController(
        IDocumentProfileApiService profileApiService,
        IEnterpriseApiService enterpriseApiService,
        ILogger<DocumentProfileController> logger)
    {
        _profileApiService = profileApiService;
        _enterpriseApiService = enterpriseApiService;
        _logger = logger;
    }

    /// <summary>
    /// List all document profiles (Google Drive-like browser)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 20,
        string? enterpriseCode = null,
        Guid? parentProfileId = null,
        string? profileType = null,
        string? status = null,
        string? searchTerm = null,
        bool? isTemplate = null,
        string viewMode = "grid",
        CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Quản lý Hồ sơ Tài liệu";

        try
        {
            var response = await _profileApiService.GetProfilesAsync(
                pageNumber,
                pageSize,
                enterpriseCode,
                parentProfileId,
                profileType,
                status,
                searchTerm,
                isTemplate,
                cancellationToken);

            var model = new DocumentProfileListViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                EnterpriseCode = enterpriseCode,
                ParentProfileId = parentProfileId,
                ProfileType = profileType,
                Status = status,
                SearchTerm = searchTerm,
                IsTemplate = isTemplate,
                ViewMode = viewMode
            };

            if (response.Success && response.Data != null)
            {
                model.Profiles = response.Data.Items.Select(p => new DocumentProfileItemViewModel
                {
                    Id = p.Id,
                    ProfileCode = p.ProfileCode,
                    ProfileName = p.ProfileName,
                    Description = p.Description,
                    EnterpriseCode = p.EnterpriseCode,
                    ParentProfileId = p.ParentProfileId,
                    ProfileType = p.ProfileType,
                    Status = p.Status,
                    IsTemplate = p.IsTemplate,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    DocumentCount = p.DocumentCount,
                    ChildProfileCount = p.ChildProfileCount
                }).ToList();
                model.TotalCount = response.Data.TotalCount;
            }
            else
            {
                TempData["Error"] = response.Message ?? "Không thể tải danh sách hồ sơ";
            }

            // Build breadcrumbs if viewing a child profile
            if (parentProfileId.HasValue)
            {
                await BuildBreadcrumbsAsync(model, parentProfileId.Value, cancellationToken);
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading document profiles list");
            TempData["Error"] = "Đã xảy ra lỗi khi tải danh sách hồ sơ";
            return View(new DocumentProfileListViewModel { ViewMode = viewMode });
        }
    }

    /// <summary>
    /// View profile details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Chi tiết Hồ sơ";

        try
        {
            var response = await _profileApiService.GetProfileByIdAsync(id, cancellationToken);
            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Không tìm thấy hồ sơ";
                return RedirectToAction(nameof(Index));
            }

            var profile = response.Data;
            var model = new DocumentProfileDetailsViewModel
            {
                Id = profile.Id,
                ProfileCode = profile.ProfileCode,
                ProfileName = profile.ProfileName,
                Description = profile.Description,
                EnterpriseCode = profile.EnterpriseCode,
                ParentProfileId = profile.ParentProfileId,
                ParentProfileName = profile.ParentProfileName,
                ProfileType = profile.ProfileType,
                Status = profile.Status,
                IsTemplate = profile.IsTemplate,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                OpenedAt = profile.OpenedAt,
                ClosedAt = profile.ClosedAt,
                ArchivedAt = profile.ArchivedAt
            };

            // Get child profiles
            var childrenResponse = await _profileApiService.GetProfilesAsync(
                pageNumber: 1,
                pageSize: 100,
                parentProfileId: id,
                cancellationToken: cancellationToken);

            if (childrenResponse.Success && childrenResponse.Data != null)
            {
                model.ChildProfiles = childrenResponse.Data.Items.Select(p => new DocumentProfileItemViewModel
                {
                    Id = p.Id,
                    ProfileCode = p.ProfileCode,
                    ProfileName = p.ProfileName,
                    ProfileType = p.ProfileType,
                    Status = p.Status,
                    DocumentCount = p.DocumentCount,
                    ChildProfileCount = p.ChildProfileCount
                }).ToList();
            }

            // Get documents
            var documentsResponse = await _profileApiService.GetProfileDocumentsAsync(
                profileId: id,
                cancellationToken: cancellationToken);

            if (documentsResponse.Success && documentsResponse.Data != null)
            {
                model.Documents = documentsResponse.Data.Items.Select(d => new ProfileDocumentItemViewModel
                {
                    Id = d.Id,
                    ProfileId = d.ProfileId,
                    DocumentId = d.DocumentId,
                    DocumentName = d.DocumentName,
                    DocumentCode = d.DocumentCode,
                    Description = d.Description,
                    FileType = d.FileType,
                    FileSize = d.FileSize,
                    DisplayOrder = d.DisplayOrder,
                    ExpiryDate = d.ExpiryDate,
                    Status = d.Status,
                    AddedAt = d.AddedAt,
                    FilePath = d.FilePath
                }).ToList();
            }

            // Get metadata fields
            var fieldsResponse = await _profileApiService.GetMetadataFieldsAsync(id, cancellationToken);
            if (fieldsResponse.Success && fieldsResponse.Data != null)
            {
                model.MetadataFields = fieldsResponse.Data.Select(f => new ProfileMetadataFieldViewModel
                {
                    Id = f.Id,
                    ProfileId = f.ProfileId,
                    FieldName = f.FieldName,
                    FieldLabel = f.FieldLabel,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt
                }).ToList();
            }

            // Build breadcrumbs
            if (profile.ParentProfileId.HasValue)
            {
                await BuildBreadcrumbsAsync(model, profile.ParentProfileId.Value, cancellationToken);
            }
            model.Breadcrumbs.Add(new BreadcrumbItem { ProfileId = profile.Id, Name = profile.ProfileName });

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile details {ProfileId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi tải chi tiết hồ sơ";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display create profile form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create(Guid? parentProfileId = null, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Tạo Hồ sơ mới";

        try
        {
            var model = new DocumentProfileFormViewModel
            {
                ParentProfileId = parentProfileId
            };

            await PopulateFormDropdownsAsync(model, cancellationToken);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create form");
            TempData["Error"] = "Đã xảy ra lỗi khi tải form tạo hồ sơ";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process create profile form
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DocumentProfileFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFormDropdownsAsync(model, cancellationToken);
            return View(model);
        }

        try
        {
            var request = new CreateDocumentProfileRequest
            {
                ProfileCode = model.ProfileCode,
                ProfileName = model.ProfileName,
                Description = model.Description,
                EnterpriseCode = model.EnterpriseCode,
                ParentProfileId = model.ParentProfileId,
                ProfileType = model.ProfileType,
                IsTemplate = model.IsTemplate
            };

            var response = await _profileApiService.CreateProfileAsync(request, cancellationToken);

            if (response.Success && response.Data != null)
            {
                TempData["Success"] = "Tạo hồ sơ thành công";
                return RedirectToAction(nameof(Details), new { id = response.Data.Id });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Không thể tạo hồ sơ");
            await PopulateFormDropdownsAsync(model, cancellationToken);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile");
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo hồ sơ");
            await PopulateFormDropdownsAsync(model, cancellationToken);
            return View(model);
        }
    }

    /// <summary>
    /// Display edit profile form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Chỉnh sửa Hồ sơ";

        try
        {
            var response = await _profileApiService.GetProfileByIdAsync(id, cancellationToken);
            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Không tìm thấy hồ sơ";
                return RedirectToAction(nameof(Index));
            }

            var profile = response.Data;
            var model = new DocumentProfileFormViewModel
            {
                Id = profile.Id,
                ProfileCode = profile.ProfileCode,
                ProfileName = profile.ProfileName,
                Description = profile.Description,
                EnterpriseCode = profile.EnterpriseCode,
                ParentProfileId = profile.ParentProfileId,
                ProfileType = profile.ProfileType,
                IsTemplate = profile.IsTemplate
            };

            await PopulateFormDropdownsAsync(model, cancellationToken);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit form for profile {ProfileId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi tải form chỉnh sửa";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process edit profile form
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DocumentProfileFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateFormDropdownsAsync(model, cancellationToken);
            return View(model);
        }

        try
        {
            var request = new UpdateDocumentProfileRequest
            {
                ProfileName = model.ProfileName,
                Description = model.Description,
                ProfileType = model.ProfileType
            };

            var response = await _profileApiService.UpdateProfileAsync(id, request, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Cập nhật hồ sơ thành công";
                return RedirectToAction(nameof(Details), new { id });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Không thể cập nhật hồ sơ");
            await PopulateFormDropdownsAsync(model, cancellationToken);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile {ProfileId}", id);
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật hồ sơ");
            await PopulateFormDropdownsAsync(model, cancellationToken);
            return View(model);
        }
    }

    /// <summary>
    /// Delete profile
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _profileApiService.DeleteProfileAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Xóa hồ sơ thành công";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Không thể xóa hồ sơ";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile {ProfileId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi xóa hồ sơ";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Open profile
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _profileApiService.OpenProfileAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Mở hồ sơ thành công";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Không thể mở hồ sơ";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening profile {ProfileId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi mở hồ sơ";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    /// <summary>
    /// Close profile
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _profileApiService.CloseProfileAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Đóng hồ sơ thành công";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Không thể đóng hồ sơ";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing profile {ProfileId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi đóng hồ sơ";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    /// <summary>
    /// Archive profile
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _profileApiService.ArchiveProfileAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Lưu trữ hồ sơ thành công";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Không thể lưu trữ hồ sơ";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving profile {ProfileId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi lưu trữ hồ sơ";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // ============================================
    // Metadata Fields Management
    // ============================================

    /// <summary>
    /// Display metadata fields management page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MetadataFields(Guid profileId, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Quản lý Trường Metadata";

        try
        {
            // Get profile info
            var profileResponse = await _profileApiService.GetProfileByIdAsync(profileId, cancellationToken);
            if (!profileResponse.Success || profileResponse.Data == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Profile = profileResponse.Data;

            // Get metadata fields
            var fieldsResponse = await _profileApiService.GetMetadataFieldsAsync(profileId, cancellationToken);
            var fields = new List<ProfileMetadataFieldViewModel>();

            if (fieldsResponse.Success && fieldsResponse.Data != null)
            {
                fields = fieldsResponse.Data.Select(f => new ProfileMetadataFieldViewModel
                {
                    Id = f.Id,
                    ProfileId = f.ProfileId,
                    FieldName = f.FieldName,
                    FieldLabel = f.FieldLabel,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    DisplayOrder = f.DisplayOrder,
                    ValidationRules = f.ValidationRules,
                    DefaultValue = f.DefaultValue,
                    Options = f.Options,
                    CreatedAt = f.CreatedAt
                }).OrderBy(f => f.DisplayOrder).ToList();
            }

            return View(fields);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading metadata fields for profile {ProfileId}", profileId);
            TempData["Error"] = "Đã xảy ra lỗi khi tải danh sách trường metadata";
            return RedirectToAction(nameof(Details), new { id = profileId });
        }
    }

    /// <summary>
    /// Display create metadata field form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CreateField(Guid profileId, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Thêm Trường Metadata";

        try
        {
            // Get profile info for display
            var profileResponse = await _profileApiService.GetProfileByIdAsync(profileId, cancellationToken);
            if (!profileResponse.Success || profileResponse.Data == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Profile = profileResponse.Data;

            // Get current field count for display order
            var fieldsResponse = await _profileApiService.GetMetadataFieldsAsync(profileId, cancellationToken);
            var nextOrder = 1;
            if (fieldsResponse.Success && fieldsResponse.Data != null)
            {
                nextOrder = fieldsResponse.Data.Count + 1;
            }

            var model = new ProfileMetadataFieldFormViewModel
            {
                ProfileId = profileId,
                DisplayOrder = nextOrder,
                FieldTypes = GetFieldTypeSelectList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create field form");
            TempData["Error"] = "Đã xảy ra lỗi khi tải form";
            return RedirectToAction(nameof(MetadataFields), new { profileId });
        }
    }

    /// <summary>
    /// Process create metadata field form
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateField(ProfileMetadataFieldFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            model.FieldTypes = GetFieldTypeSelectList();
            return View(model);
        }

        try
        {
            var request = new CreateProfileMetadataFieldRequest
            {
                ProfileId = model.ProfileId,
                FieldName = model.FieldName,
                FieldLabel = model.FieldLabel,
                FieldType = model.FieldType,
                IsRequired = model.IsRequired,
                DisplayOrder = model.DisplayOrder,
                ValidationRules = model.ValidationRules,
                DefaultValue = model.DefaultValue,
                Options = model.Options
            };

            var response = await _profileApiService.CreateMetadataFieldAsync(request, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Thêm trường metadata thành công";
                return RedirectToAction(nameof(MetadataFields), new { profileId = model.ProfileId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Không thể thêm trường metadata");
            model.FieldTypes = GetFieldTypeSelectList();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating metadata field");
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi thêm trường metadata");
            model.FieldTypes = GetFieldTypeSelectList();
            return View(model);
        }
    }

    /// <summary>
    /// Display edit metadata field form
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditField(Guid id, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "Chỉnh sửa Trường Metadata";

        try
        {
            var response = await _profileApiService.GetMetadataFieldByIdAsync(id, cancellationToken);
            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = "Không tìm thấy trường metadata";
                return RedirectToAction(nameof(Index));
            }

            var field = response.Data;
            var model = new ProfileMetadataFieldFormViewModel
            {
                Id = field.Id,
                ProfileId = field.ProfileId,
                FieldName = field.FieldName,
                FieldLabel = field.FieldLabel,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                DisplayOrder = field.DisplayOrder,
                ValidationRules = field.ValidationRules,
                DefaultValue = field.DefaultValue,
                Options = field.Options,
                FieldTypes = GetFieldTypeSelectList()
            };

            // Get profile info
            var profileResponse = await _profileApiService.GetProfileByIdAsync(field.ProfileId, cancellationToken);
            if (profileResponse.Success && profileResponse.Data != null)
            {
                ViewBag.Profile = profileResponse.Data;
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit field form {FieldId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi tải form chỉnh sửa";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Process edit metadata field form
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditField(Guid id, ProfileMetadataFieldFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.FieldTypes = GetFieldTypeSelectList();
            return View(model);
        }

        try
        {
            var request = new UpdateProfileMetadataFieldRequest
            {
                FieldLabel = model.FieldLabel,
                FieldType = model.FieldType,
                IsRequired = model.IsRequired,
                DisplayOrder = model.DisplayOrder,
                ValidationRules = model.ValidationRules,
                DefaultValue = model.DefaultValue,
                Options = model.Options
            };

            var response = await _profileApiService.UpdateMetadataFieldAsync(id, request, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Cập nhật trường metadata thành công";
                return RedirectToAction(nameof(MetadataFields), new { profileId = model.ProfileId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Không thể cập nhật trường metadata");
            model.FieldTypes = GetFieldTypeSelectList();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating metadata field {FieldId}", id);
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật trường metadata");
            model.FieldTypes = GetFieldTypeSelectList();
            return View(model);
        }
    }

    /// <summary>
    /// Delete metadata field
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteField(Guid id, Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _profileApiService.DeleteMetadataFieldAsync(id, cancellationToken);

            if (response.Success)
            {
                TempData["Success"] = "Xóa trường metadata thành công";
            }
            else
            {
                TempData["Error"] = response.Message ?? "Không thể xóa trường metadata";
            }

            return RedirectToAction(nameof(MetadataFields), new { profileId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata field {FieldId}", id);
            TempData["Error"] = "Đã xảy ra lỗi khi xóa trường metadata";
            return RedirectToAction(nameof(MetadataFields), new { profileId });
        }
    }

    // ============================================
    // Helper Methods
    // ============================================

    private async Task PopulateFormDropdownsAsync(DocumentProfileFormViewModel model, CancellationToken cancellationToken)
    {
        // Load enterprises
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
                    Value = e.TaxCode,
                    Text = $"{e.Name} ({e.TaxCode})"
                })
                .ToList();
        }

        // Load parent profiles (only root level profiles for simplicity)
        var profilesResponse = await _profileApiService.GetProfilesAsync(
            pageNumber: 1,
            pageSize: 100,
            enterpriseCode: model.EnterpriseCode,
            cancellationToken: cancellationToken);

        if (profilesResponse.Success && profilesResponse.Data != null)
        {
            model.ParentProfiles = profilesResponse.Data.Items
                .Where(p => !p.ParentProfileId.HasValue) // Only root profiles
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProfileName
                })
                .ToList();
            
            model.ParentProfiles.Insert(0, new SelectListItem { Value = "", Text = "-- Không có hồ sơ cha --" });
        }

        // Profile types
        model.ProfileTypes = new List<SelectListItem>
        {
            new() { Value = "Folder", Text = "Thư mục" },
            new() { Value = "Project", Text = "Dự án" },
            new() { Value = "Contract", Text = "Hợp đồng" },
            new() { Value = "Report", Text = "Báo cáo" },
            new() { Value = "Other", Text = "Khác" }
        };
    }

    private async Task BuildBreadcrumbsAsync(DocumentProfileListViewModel model, Guid parentId, CancellationToken cancellationToken)
    {
        var breadcrumbs = new List<BreadcrumbItem>();
        var currentId = parentId;

        // Build breadcrumb trail by walking up the hierarchy
        while (currentId != Guid.Empty)
        {
            var response = await _profileApiService.GetProfileByIdAsync(currentId, cancellationToken);
            if (!response.Success || response.Data == null)
                break;

            breadcrumbs.Insert(0, new BreadcrumbItem 
            { 
                ProfileId = response.Data.Id, 
                Name = response.Data.ProfileName 
            });

            currentId = response.Data.ParentProfileId ?? Guid.Empty;
        }

        model.Breadcrumbs = breadcrumbs;
    }

    private async Task BuildBreadcrumbsAsync(DocumentProfileDetailsViewModel model, Guid parentId, CancellationToken cancellationToken)
    {
        var breadcrumbs = new List<BreadcrumbItem>();
        var currentId = parentId;

        // Build breadcrumb trail by walking up the hierarchy
        while (currentId != Guid.Empty)
        {
            var response = await _profileApiService.GetProfileByIdAsync(currentId, cancellationToken);
            if (!response.Success || response.Data == null)
                break;

            breadcrumbs.Insert(0, new BreadcrumbItem 
            { 
                ProfileId = response.Data.Id, 
                Name = response.Data.ProfileName 
            });

            currentId = response.Data.ParentProfileId ?? Guid.Empty;
        }

        model.Breadcrumbs = breadcrumbs;
    }

    private List<SelectListItem> GetFieldTypeSelectList()
    {
        return new List<SelectListItem>
        {
            new() { Value = "Text", Text = "Văn bản" },
            new() { Value = "Number", Text = "Số" },
            new() { Value = "Date", Text = "Ngày tháng" },
            new() { Value = "Select", Text = "Danh sách lựa chọn" },
            new() { Value = "Checkbox", Text = "Hộp kiểm" },
            new() { Value = "TextArea", Text = "Văn bản dài" }
        };
    }
}
