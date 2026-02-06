using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for folder operations
/// </summary>
[ApiController]
[Route("api/v1/folders")]
[Produces("application/json")]
public class FoldersController : ControllerBase
{
    private readonly IFolderService _folderService;
    private readonly ILogger<FoldersController> _logger;

    /// <summary>
    /// Initializes a new instance of FoldersController
    /// </summary>
    public FoldersController(
        IFolderService folderService,
        ILogger<FoldersController> logger)
    {
        _folderService = folderService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new folder
    /// </summary>
    /// <param name="request">The folder creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created folder</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FolderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFolder(
        [FromBody] CreateFolderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Folder name is required"));
        }

        if (string.IsNullOrWhiteSpace(request.EnterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _folderService.CreateFolderAsync(
            request.Name,
            request.EnterpriseCode,
            request.ParentFolderId,
            userId,
            request.Description,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(
            nameof(GetFolder),
            new { id = result.Value!.Id },
            ApiResponse<FolderDto>.SuccessResponse(result.Value, "Folder created successfully"));
    }

    /// <summary>
    /// Gets a folder by ID
    /// </summary>
    /// <param name="id">The folder ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The folder</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FolderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFolder(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _folderService.GetFolderAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets the root folder for an enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The root folder</returns>
    [HttpGet("root")]
    [ProducesResponseType(typeof(ApiResponse<FolderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRootFolder(
        [FromQuery] string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var result = await _folderService.GetRootFolderAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Lists folders with pagination
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="parentFolderId">Optional parent folder ID</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of folders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FolderDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListFolders(
        [FromQuery] string enterpriseCode,
        [FromQuery] Guid? parentFolderId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var result = await _folderService.ListFoldersAsync(
            enterpriseCode,
            parentFolderId,
            pageNumber,
            pageSize,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<FolderDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Deletes a folder (soft delete)
    /// </summary>
    /// <param name="id">The folder ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteFolder(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _folderService.DeleteFolderAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return NoContent();
    }
}
