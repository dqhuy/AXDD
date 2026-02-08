using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for folder type operations
/// </summary>
[ApiController]
[Route("api/v1/folder-types")]
[Produces("application/json")]
public class FolderTypesController : ControllerBase
{
    private readonly IFolderTypeService _folderTypeService;
    private readonly ILogger<FolderTypesController> _logger;

    public FolderTypesController(IFolderTypeService folderTypeService, ILogger<FolderTypesController> logger)
    {
        _folderTypeService = folderTypeService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all folder types with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FolderTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _folderTypeService.ListAsync(pageNumber, pageSize, searchTerm, isActive, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<FolderTypeDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a folder type by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FolderTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _folderTypeService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderTypeDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a folder type by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<FolderTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        var result = await _folderTypeService.GetByCodeAsync(code, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderTypeDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a new folder type
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FolderTypeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFolderTypeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Code is required"));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Name is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _folderTypeService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<FolderTypeDto>.SuccessResponse(result.Value, "Folder type created successfully"));
    }

    /// <summary>
    /// Updates a folder type
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FolderTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateFolderTypeRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _folderTypeService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<FolderTypeDto>.SuccessResponse(result.Value!, "Folder type updated successfully"));
    }

    /// <summary>
    /// Deletes a folder type
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _folderTypeService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Adds a metadata field to a folder type
    /// </summary>
    [HttpPost("{folderTypeId}/metadata-fields")]
    [ProducesResponseType(typeof(ApiResponse<FolderTypeMetadataFieldDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMetadataField(Guid folderTypeId, [FromBody] CreateFolderTypeMetadataFieldRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _folderTypeService.AddMetadataFieldAsync(folderTypeId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Created("", ApiResponse<FolderTypeMetadataFieldDto>.SuccessResponse(result.Value!, "Metadata field added successfully"));
    }

    /// <summary>
    /// Removes a metadata field from a folder type
    /// </summary>
    [HttpDelete("{folderTypeId}/metadata-fields/{fieldId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMetadataField(Guid folderTypeId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _folderTypeService.RemoveMetadataFieldAsync(folderTypeId, fieldId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
