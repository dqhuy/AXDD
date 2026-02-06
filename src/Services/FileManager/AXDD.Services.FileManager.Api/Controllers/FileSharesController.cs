using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for file sharing operations
/// </summary>
[ApiController]
[Route("api/v1/shares")]
[Produces("application/json")]
public class FileSharesController : ControllerBase
{
    private readonly IFileShareService _shareService;
    private readonly ILogger<FileSharesController> _logger;

    /// <summary>
    /// Initializes a new instance of FileSharesController
    /// </summary>
    public FileSharesController(
        IFileShareService shareService,
        ILogger<FileSharesController> logger)
    {
        _shareService = shareService;
        _logger = logger;
    }

    /// <summary>
    /// Shares a file with a user
    /// </summary>
    /// <param name="request">The share request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created file share</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FileShareDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShareFile(
        [FromBody] ShareFileRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.SharedWithUserId))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Shared with user ID is required"));
        }

        if (string.IsNullOrWhiteSpace(request.Permission))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Permission is required"));
        }

        var sharedBy = User.Identity?.Name ?? "anonymous";

        var result = await _shareService.ShareFileAsync(
            request.FileId,
            request.SharedWithUserId,
            request.Permission,
            sharedBy,
            request.ExpiresAt,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return CreatedAtAction(
            nameof(GetSharedFiles),
            new { userId = request.SharedWithUserId },
            ApiResponse<FileShareDto>.SuccessResponse(result.Value!, "File shared successfully"));
    }

    /// <summary>
    /// Gets files shared with a user
    /// </summary>
    /// <param name="userId">The user ID (optional, defaults to current user)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of shared files</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FileShareDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSharedFiles(
        [FromQuery] string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var targetUserId = userId ?? User.Identity?.Name ?? "anonymous";

        var result = await _shareService.GetSharedFilesAsync(targetUserId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<FileShareDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Revokes a file share
    /// </summary>
    /// <param name="id">The share ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeShare(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _shareService.RevokeShareAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
