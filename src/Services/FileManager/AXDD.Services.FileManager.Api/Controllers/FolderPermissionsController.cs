using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for folder permission operations
/// </summary>
[ApiController]
[Route("api/v1/folder-permissions")]
[Produces("application/json")]
public class FolderPermissionsController : ControllerBase
{
    private readonly IFolderPermissionService _permissionService;
    private readonly ILogger<FolderPermissionsController> _logger;

    public FolderPermissionsController(IFolderPermissionService permissionService, ILogger<FolderPermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets permissions for a folder
    /// </summary>
    [HttpGet("folder/{folderId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FolderPermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListByFolder(Guid folderId, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.ListByFolderAsync(folderId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<FolderPermissionDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets permissions for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FolderPermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListByUser(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.ListByUserAsync(userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<FolderPermissionDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a permission by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FolderPermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderPermissionDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Checks user permission on a folder
    /// </summary>
    [HttpGet("check")]
    [ProducesResponseType(typeof(ApiResponse<PermissionLevel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckPermission([FromQuery] Guid folderId, [FromQuery] string userId, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.CheckPermissionAsync(folderId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PermissionLevel>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Grants permission to a folder
    /// </summary>
    [HttpPost("folder/{folderId}")]
    [ProducesResponseType(typeof(ApiResponse<FolderPermissionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GrantPermission(Guid folderId, [FromBody] GrantFolderPermissionRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) && string.IsNullOrWhiteSpace(request.UserGroupId))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Either UserId or UserGroupId is required"));
        }

        var grantedBy = User.Identity?.Name ?? "anonymous";
        var result = await _permissionService.GrantPermissionAsync(folderId, request, grantedBy, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<FolderPermissionDto>.SuccessResponse(result.Value, "Permission granted successfully"));
    }

    /// <summary>
    /// Updates a folder permission
    /// </summary>
    [HttpPut("folder/{folderId}/permission/{permissionId}")]
    [ProducesResponseType(typeof(ApiResponse<FolderPermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePermission(Guid folderId, Guid permissionId, [FromBody] GrantFolderPermissionRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _permissionService.UpdatePermissionAsync(folderId, permissionId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderPermissionDto>.SuccessResponse(result.Value!, "Permission updated successfully"));
    }

    /// <summary>
    /// Revokes a folder permission
    /// </summary>
    [HttpDelete("folder/{folderId}/permission/{permissionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokePermission(Guid folderId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _permissionService.RevokePermissionAsync(folderId, permissionId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
