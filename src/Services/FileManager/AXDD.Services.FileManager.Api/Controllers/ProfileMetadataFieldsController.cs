using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for profile metadata field operations
/// </summary>
[ApiController]
[Route("api/v1/profile-metadata-fields")]
[Produces("application/json")]
public class ProfileMetadataFieldsController : ControllerBase
{
    private readonly IProfileMetadataFieldService _fieldService;
    private readonly ILogger<ProfileMetadataFieldsController> _logger;

    /// <summary>
    /// Initializes a new instance of ProfileMetadataFieldsController
    /// </summary>
    public ProfileMetadataFieldsController(
        IProfileMetadataFieldService fieldService,
        ILogger<ProfileMetadataFieldsController> logger)
    {
        _fieldService = fieldService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new metadata field
    /// </summary>
    /// <param name="request">The create field request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created field</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProfileMetadataFieldDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateField(
        [FromBody] CreateProfileMetadataFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FieldName))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Field name is required"));
        }

        if (string.IsNullOrWhiteSpace(request.DisplayLabel))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Display label is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _fieldService.CreateFieldAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(
            nameof(GetField),
            new { fieldId = result.Value!.Id },
            ApiResponse<ProfileMetadataFieldDto>.SuccessResponse(result.Value!, "Metadata field created successfully"));
    }

    /// <summary>
    /// Gets a metadata field by ID
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The field</returns>
    [HttpGet("{fieldId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProfileMetadataFieldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetField(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        var result = await _fieldService.GetFieldAsync(fieldId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<ProfileMetadataFieldDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Updates a metadata field
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="request">The update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated field</returns>
    [HttpPut("{fieldId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProfileMetadataFieldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateField(
        Guid fieldId,
        [FromBody] UpdateProfileMetadataFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _fieldService.UpdateFieldAsync(fieldId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return Ok(ApiResponse<ProfileMetadataFieldDto>.SuccessResponse(result.Value!, "Metadata field updated successfully"));
    }

    /// <summary>
    /// Deletes a metadata field
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{fieldId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteField(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _fieldService.DeleteFieldAsync(fieldId, userId, cancellationToken);

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
    /// Lists metadata fields for a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="includeDisabled">Whether to include disabled fields</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of metadata fields</returns>
    [HttpGet("by-profile/{profileId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProfileMetadataFieldDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFieldsByProfile(
        Guid profileId,
        [FromQuery] bool includeDisabled = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _fieldService.ListFieldsAsync(profileId, includeDisabled, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<ProfileMetadataFieldDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Reorders metadata fields
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="fieldOrders">Dictionary of field ID to display order</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("by-profile/{profileId:guid}/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderFields(
        Guid profileId,
        [FromBody] Dictionary<Guid, int> fieldOrders,
        CancellationToken cancellationToken = default)
    {
        if (fieldOrders == null || !fieldOrders.Any())
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Field orders are required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _fieldService.ReorderFieldsAsync(profileId, fieldOrders, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Copies metadata fields from one profile to another
    /// </summary>
    /// <param name="sourceProfileId">The source profile ID</param>
    /// <param name="targetProfileId">The target profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of copied fields</returns>
    [HttpPost("copy")]
    [ProducesResponseType(typeof(ApiResponse<List<ProfileMetadataFieldDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CopyFields(
        [FromQuery] Guid sourceProfileId,
        [FromQuery] Guid targetProfileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _fieldService.CopyFieldsAsync(sourceProfileId, targetProfileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<ProfileMetadataFieldDto>>.SuccessResponse(result.Value!, "Fields copied successfully"));
    }
}
