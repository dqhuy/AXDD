using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Enterprise.Api.Application.DTOs;

namespace AXDD.Services.Enterprise.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for managing enterprise licenses
/// </summary>
public interface IEnterpriseLicenseService
{
    /// <summary>
    /// Gets all licenses for an enterprise
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of licenses</returns>
    Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetByEnterpriseIdAsync(Guid enterpriseId, CancellationToken ct);

    /// <summary>
    /// Gets a license by ID
    /// </summary>
    /// <param name="id">License ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>License details</returns>
    Task<Result<EnterpriseLicenseDto>> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Creates a new license
    /// </summary>
    /// <param name="request">License creation request</param>
    /// <param name="userId">ID of the user creating the license</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created license details</returns>
    Task<Result<EnterpriseLicenseDto>> CreateAsync(CreateLicenseRequest request, string userId, CancellationToken ct);

    /// <summary>
    /// Updates an existing license
    /// </summary>
    /// <param name="id">License ID</param>
    /// <param name="request">License update request</param>
    /// <param name="userId">ID of the user updating the license</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated license details</returns>
    Task<Result<EnterpriseLicenseDto>> UpdateAsync(Guid id, UpdateLicenseRequest request, string userId, CancellationToken ct);

    /// <summary>
    /// Deletes a license
    /// </summary>
    /// <param name="id">License ID</param>
    /// <param name="userId">ID of the user deleting the license</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if deleted successfully</returns>
    Task<Result<bool>> DeleteAsync(Guid id, string userId, CancellationToken ct);

    /// <summary>
    /// Gets licenses that are expiring soon
    /// </summary>
    /// <param name="days">Number of days to check ahead</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of licenses expiring within the specified days</returns>
    Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetExpiringSoonAsync(int days, CancellationToken ct);

    /// <summary>
    /// Gets all licenses for an enterprise (alias for GetByEnterpriseIdAsync)
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of licenses</returns>
    Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetByEnterpriseAsync(Guid enterpriseId, CancellationToken ct);

    /// <summary>
    /// Gets licenses that are expiring soon (alias for GetExpiringSoonAsync)
    /// </summary>
    /// <param name="days">Number of days to check ahead</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of licenses expiring within the specified days</returns>
    Task<Result<IReadOnlyList<EnterpriseLicenseDto>>> GetExpiringLicensesAsync(int days, CancellationToken ct);
}
