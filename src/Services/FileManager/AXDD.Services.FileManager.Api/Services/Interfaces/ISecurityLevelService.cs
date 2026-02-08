using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for security level operations
/// </summary>
public interface ISecurityLevelService
{
    /// <summary>
    /// Creates a new security level
    /// </summary>
    Task<Result<SecurityLevelDto>> CreateAsync(CreateSecurityLevelRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a security level by ID
    /// </summary>
    Task<Result<SecurityLevelDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a security level by code
    /// </summary>
    Task<Result<SecurityLevelDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all security levels
    /// </summary>
    Task<Result<PagedResult<SecurityLevelDto>>> ListAsync(int pageNumber = 1, int pageSize = 10, bool? isActive = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a security level
    /// </summary>
    Task<Result<SecurityLevelDto>> UpdateAsync(Guid id, CreateSecurityLevelRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a security level
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);
}
