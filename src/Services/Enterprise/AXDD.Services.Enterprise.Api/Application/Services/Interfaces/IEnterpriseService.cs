using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for managing enterprises
/// </summary>
public interface IEnterpriseService
{
    /// <summary>
    /// Gets a paginated list of enterprises with filtering and sorting
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Search term for name, code, or tax code</param>
    /// <param name="status">Filter by enterprise status</param>
    /// <param name="zoneId">Filter by industrial zone ID</param>
    /// <param name="industryCode">Filter by industry code</param>
    /// <param name="sortBy">Field to sort by</param>
    /// <param name="descending">Sort in descending order</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of enterprises</returns>
    Task<Result<PagedResult<EnterpriseListDto>>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        EnterpriseStatus? status,
        Guid? zoneId,
        string? industryCode,
        string? sortBy,
        bool descending,
        CancellationToken ct);

    /// <summary>
    /// Gets an enterprise by its unique identifier
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Enterprise details including contacts and licenses</returns>
    Task<Result<EnterpriseDto>> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Gets an enterprise by its unique code
    /// </summary>
    /// <param name="code">Enterprise code</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Enterprise details</returns>
    Task<Result<EnterpriseDto>> GetByCodeAsync(string code, CancellationToken ct);

    /// <summary>
    /// Gets an enterprise by its tax code
    /// </summary>
    /// <param name="taxCode">Tax code</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Enterprise details</returns>
    Task<Result<EnterpriseDto>> GetByTaxCodeAsync(string taxCode, CancellationToken ct);

    /// <summary>
    /// Creates a new enterprise
    /// </summary>
    /// <param name="request">Enterprise creation request</param>
    /// <param name="userId">ID of the user creating the enterprise</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created enterprise details</returns>
    Task<Result<EnterpriseDto>> CreateAsync(CreateEnterpriseRequest request, string userId, CancellationToken ct);

    /// <summary>
    /// Updates an existing enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="request">Enterprise update request</param>
    /// <param name="userId">ID of the user updating the enterprise</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated enterprise details</returns>
    Task<Result<EnterpriseDto>> UpdateAsync(Guid id, UpdateEnterpriseRequest request, string userId, CancellationToken ct);

    /// <summary>
    /// Deletes an enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="userId">ID of the user deleting the enterprise</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if deleted successfully</returns>
    Task<Result<bool>> DeleteAsync(Guid id, string userId, CancellationToken ct);

    /// <summary>
    /// Changes the status of an enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="newStatus">New status</param>
    /// <param name="reason">Reason for status change</param>
    /// <param name="userId">ID of the user changing the status</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated enterprise details</returns>
    Task<Result<EnterpriseDto>> ChangeStatusAsync(
        Guid id,
        EnterpriseStatus newStatus,
        string? reason,
        string userId,
        CancellationToken ct);

    /// <summary>
    /// Gets statistics about enterprises
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Enterprise statistics</returns>
    Task<Result<EnterpriseStatisticsDto>> GetStatisticsAsync(CancellationToken ct);
}
