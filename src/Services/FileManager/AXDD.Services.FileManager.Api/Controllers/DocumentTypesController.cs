using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for document type operations
/// </summary>
[ApiController]
[Route("api/v1/document-types")]
[Produces("application/json")]
public class DocumentTypesController : ControllerBase
{
    private readonly IDocumentTypeService _documentTypeService;
    private readonly ILogger<DocumentTypesController> _logger;

    public DocumentTypesController(IDocumentTypeService documentTypeService, ILogger<DocumentTypesController> logger)
    {
        _documentTypeService = documentTypeService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all document types with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _documentTypeService.ListAsync(pageNumber, pageSize, searchTerm, isActive, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<DocumentTypeDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a document type by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _documentTypeService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentTypeDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a document type by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        var result = await _documentTypeService.GetByCodeAsync(code, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentTypeDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a new document type
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentTypeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentTypeRequest request, CancellationToken cancellationToken = default)
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
        var result = await _documentTypeService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<DocumentTypeDto>.SuccessResponse(result.Value, "Document type created successfully"));
    }

    /// <summary>
    /// Updates a document type
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateDocumentTypeRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _documentTypeService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentTypeDto>.SuccessResponse(result.Value!, "Document type updated successfully"));
    }

    /// <summary>
    /// Deletes a document type
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _documentTypeService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Adds a metadata field to a document type
    /// </summary>
    [HttpPost("{documentTypeId}/metadata-fields")]
    [ProducesResponseType(typeof(ApiResponse<DocumentTypeMetadataFieldDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMetadataField(Guid documentTypeId, [FromBody] CreateDocumentTypeMetadataFieldRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _documentTypeService.AddMetadataFieldAsync(documentTypeId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Created("", ApiResponse<DocumentTypeMetadataFieldDto>.SuccessResponse(result.Value!, "Metadata field added successfully"));
    }

    /// <summary>
    /// Removes a metadata field from a document type
    /// </summary>
    [HttpDelete("{documentTypeId}/metadata-fields/{fieldId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMetadataField(Guid documentTypeId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _documentTypeService.RemoveMetadataFieldAsync(documentTypeId, fieldId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
