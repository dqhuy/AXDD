using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for document profile operations
/// </summary>
[ApiController]
[Route("api/v1/document-profiles")]
[Produces("application/json")]
public class DocumentProfilesController : ControllerBase
{
    private readonly IDocumentProfileService _profileService;
    private readonly ILogger<DocumentProfilesController> _logger;

    /// <summary>
    /// Initializes a new instance of DocumentProfilesController
    /// </summary>
    public DocumentProfilesController(
        IDocumentProfileService profileService,
        ILogger<DocumentProfilesController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new document profile
    /// </summary>
    /// <param name="request">The create profile request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created profile</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile(
        [FromBody] CreateDocumentProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Profile name is required"));
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Profile code is required"));
        }

        if (string.IsNullOrWhiteSpace(request.EnterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.CreateProfileAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(
            nameof(GetProfile),
            new { profileId = result.Value!.Id },
            ApiResponse<DocumentProfileDto>.SuccessResponse(result.Value!, "Profile created successfully"));
    }

    /// <summary>
    /// Gets a profile by ID
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="includeMetadataFields">Whether to include metadata fields</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The profile</returns>
    [HttpGet("{profileId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(
        Guid profileId,
        [FromQuery] bool includeMetadataFields = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _profileService.GetProfileAsync(profileId, includeMetadataFields, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentProfileDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Updates a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="request">The update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated profile</returns>
    [HttpPut("{profileId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        Guid profileId,
        [FromBody] UpdateDocumentProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.UpdateProfileAsync(profileId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return Ok(ApiResponse<DocumentProfileDto>.SuccessResponse(result.Value!, "Profile updated successfully"));
    }

    /// <summary>
    /// Deletes a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfile(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.DeleteProfileAsync(profileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return NoContent();
    }

    /// <summary>
    /// Lists profiles with pagination
    /// </summary>
    /// <param name="enterpriseCode">Optional enterprise code filter</param>
    /// <param name="parentProfileId">Optional parent profile ID filter</param>
    /// <param name="profileType">Optional profile type filter</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="isTemplate">Optional template filter</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of profiles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentProfileDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProfiles(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] Guid? parentProfileId = null,
        [FromQuery] string? profileType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isTemplate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new DocumentProfileListQuery
        {
            EnterpriseCode = enterpriseCode,
            ParentProfileId = parentProfileId,
            ProfileType = profileType,
            Status = status,
            SearchTerm = searchTerm,
            IsTemplate = isTemplate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _profileService.ListProfilesAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<DocumentProfileDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets the profile hierarchy tree
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="rootProfileId">Optional root profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The profile hierarchy</returns>
    [HttpGet("hierarchy")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentProfileDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfileHierarchy(
        [FromQuery] string enterpriseCode,
        [FromQuery] Guid? rootProfileId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var result = await _profileService.GetProfileHierarchyAsync(enterpriseCode, rootProfileId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<DocumentProfileDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a profile from a template
    /// </summary>
    /// <param name="templateId">The template profile ID</param>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="name">The new profile name</param>
    /// <param name="code">The new profile code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created profile</returns>
    [HttpPost("from-template/{templateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFromTemplate(
        Guid templateId,
        [FromQuery] string enterpriseCode,
        [FromQuery] string name,
        [FromQuery] string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Profile name is required"));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Profile code is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.CreateFromTemplateAsync(templateId, enterpriseCode, name, code, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return CreatedAtAction(
            nameof(GetProfile),
            new { profileId = result.Value!.Id },
            ApiResponse<DocumentProfileDto>.SuccessResponse(result.Value!, "Profile created from template successfully"));
    }

    /// <summary>
    /// Opens a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{profileId:guid}/open")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OpenProfile(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.OpenProfileAsync(profileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Closes a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{profileId:guid}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseProfile(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.CloseProfileAsync(profileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Archives a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{profileId:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveProfile(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _profileService.ArchiveProfileAsync(profileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
