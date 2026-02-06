using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Auth.Api.DTOs;

namespace AXDD.Services.Auth.Api.Services.Interfaces;

/// <summary>
/// User management service interface
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a paginated list of users
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged result of users</returns>
    Task<Result<PagedResult<UserDto>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User DTO</returns>
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request">Create user request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user DTO</returns>
    Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Update user request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user DTO</returns>
    Task<Result<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns roles to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Assign roles request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    Task<Result> AssignRolesAsync(Guid userId, AssignRolesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes roles from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roles">Roles to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    Task<Result> RemoveRolesAsync(Guid userId, List<string> roles, CancellationToken cancellationToken = default);
}
