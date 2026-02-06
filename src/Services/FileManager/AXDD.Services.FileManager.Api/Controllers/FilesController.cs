using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for file operations
/// </summary>
[ApiController]
[Route("api/v1/files")]
[Produces("application/json")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FilesController> _logger;

    /// <summary>
    /// Initializes a new instance of FilesController
    /// </summary>
    public FilesController(
        IFileService fileService,
        ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a new file
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="folderId">Optional folder ID</param>
    /// <param name="description">Optional description</param>
    /// <param name="tags">Optional tags</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The uploaded file metadata</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FileMetadataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status507InsufficientStorage)]
    public async Task<IActionResult> UploadFile(
        [FromForm] IFormFile file,
        [FromForm] string enterpriseCode,
        [FromForm] Guid? folderId = null,
        [FromForm] string? description = null,
        [FromForm] string? tags = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded"));
        }

        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        // Get user ID from claims (placeholder - should come from authentication)
        var userId = User.Identity?.Name ?? "anonymous";

        await using var stream = file.OpenReadStream();

        var result = await _fileService.UploadAsync(
            stream,
            file.FileName,
            file.ContentType,
            enterpriseCode,
            folderId,
            userId,
            description,
            tags,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("quota exceeded", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status507InsufficientStorage,
                    ApiResponse<object>.ErrorResponse(result.Error));
            }

            if (result.Error.Contains("exceeds the maximum", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    ApiResponse<object>.ErrorResponse(result.Error));
            }

            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return Ok(ApiResponse<FileMetadataDto>.SuccessResponse(result.Value!, "File uploaded successfully"));
    }

    /// <summary>
    /// Downloads a file
    /// </summary>
    /// <param name="id">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The file stream</returns>
    [HttpGet("{id}/download")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _fileService.DownloadAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        var (stream, fileName, mimeType) = result.Value;

        return File(stream, mimeType, fileName);
    }

    /// <summary>
    /// Gets a presigned URL for viewing/downloading a file
    /// </summary>
    /// <param name="id">The file ID</param>
    /// <param name="expiryMinutes">URL expiry in minutes (default 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The presigned URL</returns>
    [HttpGet("{id}/view")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileUrl(
        Guid id,
        [FromQuery] int expiryMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var result = await _fileService.GetFileUrlAsync(id, expiryMinutes, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<string>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets file metadata by ID
    /// </summary>
    /// <param name="id">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The file metadata</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FileMetadataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileMetadata(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _fileService.GetFileMetadataAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FileMetadataDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Deletes a file (soft delete)
    /// </summary>
    /// <param name="id">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFile(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _fileService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Lists files with pagination
    /// </summary>
    /// <param name="enterpriseCode">Optional enterprise code filter</param>
    /// <param name="folderId">Optional folder ID filter</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of files</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FileMetadataDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFiles(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] Guid? folderId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new FileListQuery
        {
            EnterpriseCode = enterpriseCode,
            FolderId = folderId,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _fileService.ListFilesAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<FileMetadataDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Checks storage quota availability
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="fileSize">The required file size in bytes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Whether quota is available</returns>
    [HttpGet("quota/check")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckQuota(
        [FromQuery] string enterpriseCode,
        [FromQuery] long fileSize,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var result = await _fileService.CheckQuotaAsync(enterpriseCode, fileSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(result.Value));
    }
}
