using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for managing enterprise history
/// </summary>
public interface IEnterpriseHistoryService
{
    /// <summary>
    /// Gets the history of changes for an enterprise
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of history records</returns>
    Task<Result<IReadOnlyList<EnterpriseHistoryDto>>> GetHistoryAsync(Guid enterpriseId, CancellationToken ct);

    /// <summary>
    /// Gets the history of changes for an enterprise with pagination
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paged result of history records</returns>
    Task<Result<PagedResult<EnterpriseHistoryDto>>> GetHistoryAsync(Guid enterpriseId, int pageNumber, int pageSize, CancellationToken ct);

    /// <summary>
    /// Logs a creation event
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="userId">User who created the enterprise</param>
    /// <param name="details">Additional details</param>
    /// <param name="ct">Cancellation token</param>
    Task LogCreationAsync(Guid enterpriseId, string userId, string? details, CancellationToken ct);

    /// <summary>
    /// Logs an update event
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="userId">User who updated the enterprise</param>
    /// <param name="changes">Dictionary of changed fields and their old/new values</param>
    /// <param name="ct">Cancellation token</param>
    Task LogUpdateAsync(
        Guid enterpriseId,
        string userId,
        Dictionary<string, (string? OldValue, string? NewValue)> changes,
        CancellationToken ct);

    /// <summary>
    /// Logs a status change event
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="userId">User who changed the status</param>
    /// <param name="oldStatus">Previous status</param>
    /// <param name="newStatus">New status</param>
    /// <param name="reason">Reason for the change</param>
    /// <param name="ct">Cancellation token</param>
    Task LogStatusChangeAsync(
        Guid enterpriseId,
        string userId,
        EnterpriseStatus oldStatus,
        EnterpriseStatus newStatus,
        string? reason,
        CancellationToken ct);

    /// <summary>
    /// Logs a deletion event
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="userId">User who deleted the enterprise</param>
    /// <param name="reason">Reason for deletion</param>
    /// <param name="ct">Cancellation token</param>
    Task LogDeletionAsync(Guid enterpriseId, string userId, string? reason, CancellationToken ct);

    /// <summary>
    /// Logs a contact person change
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="userId">User who made the change</param>
    /// <param name="changeType">Type of change (ContactAdded, ContactUpdated, ContactRemoved)</param>
    /// <param name="contactName">Name of the contact person</param>
    /// <param name="details">Additional details</param>
    /// <param name="ct">Cancellation token</param>
    Task LogContactChangeAsync(
        Guid enterpriseId,
        string userId,
        ChangeType changeType,
        string contactName,
        string? details,
        CancellationToken ct);

    /// <summary>
    /// Logs a license change
    /// </summary>
    /// <param name="enterpriseId">Enterprise ID</param>
    /// <param name="userId">User who made the change</param>
    /// <param name="changeType">Type of change (LicenseAdded, LicenseUpdated, LicenseRemoved)</param>
    /// <param name="licenseNumber">License number</param>
    /// <param name="details">Additional details</param>
    /// <param name="ct">Cancellation token</param>
    Task LogLicenseChangeAsync(
        Guid enterpriseId,
        string userId,
        ChangeType changeType,
        string licenseNumber,
        string? details,
        CancellationToken ct);
}
