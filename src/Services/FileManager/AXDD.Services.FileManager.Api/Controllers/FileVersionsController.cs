using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for file version operations
/// </summary>
[ApiController]
[Route("api/v1/files/{fileId}/versions")]
[Produces("application/json")]
public class FileVersionsController : ControllerBase
{
    private readonly IFileVersionService _versionService;
    private readonly ILogger<FileVersionsController> _logger;

    /// <summary>
    /// Initializes a new instance of FileVersionsController
    /// </summary>
    public FileVersionsController(
        IFileVersionService versionService,
        ILogger<FileVersionsController> logger)
    {
        _versionService = versionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all versions of a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of file versions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FileVersionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersions(Guid fileId, CancellationToken cancellationToken = default)
    {
        var result = await _versionService.GetVersionsAsync(fileId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<FileVersionDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a new version of a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="file">The new file version</param>
    /// <param name="notes">Optional version notes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created version</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FileVersionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateVersion(
        Guid fileId,
        [FromForm] IFormFile file,
        [FromForm] string? notes = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        await using var stream = file.OpenReadStream();

        var result = await _versionService.CreateVersionAsync(
            fileId,
            stream,
            userId,
            notes,
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
            nameof(GetVersions),
            new { fileId },
            ApiResponse<FileVersionDto>.SuccessResponse(result.Value!, "Version created successfully"));
    }

    /// <summary>
    /// Restores a specific version of a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="version">The version number to restore</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated file metadata</returns>
    [HttpPost("{version}/restore")]
    [ProducesResponseType(typeof(ApiResponse<FileMetadataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreVersion(
        Guid fileId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _versionService.RestoreVersionAsync(fileId, version, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FileMetadataDto>.SuccessResponse(result.Value!, "Version restored successfully"));
    }
}
