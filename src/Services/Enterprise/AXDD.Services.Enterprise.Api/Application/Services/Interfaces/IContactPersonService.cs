using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Enterprise.Api.Application.DTOs;

namespace AXDD.Services.Enterprise.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for managing enterprise contact persons
/// </summary>
public interface IContactPersonService
{
    /// <summary>
    /// Gets all contact persons for an enterprise
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of contact persons</returns>
    Task<Result<IReadOnlyList<ContactPersonDto>>> GetByEnterpriseIdAsync(Guid enterpriseId, CancellationToken ct);

    /// <summary>
    /// Gets a contact person by ID
    /// </summary>
    /// <param name="id">Contact person ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Contact person details</returns>
    Task<Result<ContactPersonDto>> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new contact person
    /// </summary>
    /// <param name="request">Contact creation request</param>
    /// <param name="userId">ID of the user creating the contact</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created contact person details</returns>
    Task<Result<ContactPersonDto>> CreateAsync(CreateContactRequest request, string userId, CancellationToken ct);

    /// <summary>
    /// Updates an existing contact person
    /// </summary>
    /// <param name="id">Contact person ID</param>
    /// <param name="request">Contact update request</param>
    /// <param name="userId">ID of the user updating the contact</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated contact person details</returns>
    Task<Result<ContactPersonDto>> UpdateAsync(Guid id, UpdateContactRequest request, string userId, CancellationToken ct);

    /// <summary>
    /// Deletes a contact person
    /// </summary>
    /// <param name="id">Contact person ID</param>
    /// <param name="userId">ID of the user deleting the contact</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if deleted successfully</returns>
    Task<Result<bool>> DeleteAsync(Guid id, string userId, CancellationToken ct);

    /// <summary>
    /// Sets a contact person as the main contact
    /// </summary>
    /// <param name="id">Contact person ID</param>
    /// <param name="userId">ID of the user making the change</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated contact person details</returns>
    Task<Result<ContactPersonDto>> SetAsMainContactAsync(Guid id, string userId, CancellationToken ct);

    /// <summary>
    /// Gets all contact persons for an enterprise (alias for GetByEnterpriseIdAsync)
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of contact persons</returns>
    Task<Result<IReadOnlyList<ContactPersonDto>>> GetByEnterpriseAsync(Guid enterpriseId, CancellationToken ct);

    /// <summary>
    /// Sets a contact person as the main contact (alias for SetAsMainContactAsync)
    /// </summary>
    /// <param name="contactId">Contact person ID</param>
    /// <param name="userId">ID of the user making the change</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated contact person details</returns>
    Task<Result<ContactPersonDto>> SetMainContactAsync(Guid contactId, string userId, CancellationToken ct);
}
