using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Enterprise.Api.Controllers;

/// <summary>
/// Controller for contact person management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ContactPersonsController : ControllerBase
{
    private readonly IContactPersonService _contactService;
    private readonly ILogger<ContactPersonsController> _logger;

    public ContactPersonsController(
        IContactPersonService contactService,
        ILogger<ContactPersonsController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a contact person by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ContactPersonDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _contactService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<ContactPersonDto>.NotFound(result.Error ?? "Contact not found"));
        }

        return Ok(ApiResponse<ContactPersonDto>.Success(result.Value!));
    }

    /// <summary>
    /// Creates a new contact person
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ContactPersonDto>>> Create(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _contactService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<ContactPersonDto>.Failure(result.Error ?? "Failed to create contact"));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            ApiResponse<ContactPersonDto>.Success(result.Value!, "Contact created successfully", 201));
    }

    /// <summary>
    /// Updates an existing contact person
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ContactPersonDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ContactPersonDto>>> Update(
        Guid id,
        [FromBody] UpdateContactRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _contactService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<ContactPersonDto>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<ContactPersonDto>.Failure(result.Error ?? "Failed to update contact"));
        }

        return Ok(ApiResponse<ContactPersonDto>.Success(result.Value!, "Contact updated successfully"));
    }

    /// <summary>
    /// Deletes a contact person
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _contactService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<bool>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<bool>.Failure(result.Error ?? "Failed to delete contact"));
        }

        return Ok(ApiResponse<bool>.Success(true, "Contact deleted successfully"));
    }

    /// <summary>
    /// Sets a contact as the main contact for an enterprise
    /// </summary>
    [HttpPost("{id:guid}/set-main")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> SetMainContact(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _contactService.SetMainContactAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<bool>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<bool>.Failure(result.Error ?? "Failed to set main contact"));
        }

        return Ok(ApiResponse<bool>.Success(true, "Main contact set successfully"));
    }
}
