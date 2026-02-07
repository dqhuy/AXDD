using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for user activity operations
/// </summary>
public interface IUserActivityService
{
    /// <summary>
    /// Logs a user activity
    /// </summary>
    Task<UserActivityDto> LogActivityAsync(CreateUserActivityRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user activities with filtering and pagination
    /// </summary>
    Task<PagedResult<UserActivityDto>> GetUserActivitiesAsync(Guid userId, int pageNumber, int pageSize, ActivityType? activityType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent activities across all users
    /// </summary>
    Task<List<UserActivityDto>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activities by resource
    /// </summary>
    Task<PagedResult<UserActivityDto>> GetActivitiesByResourceAsync(string resourceType, Guid resourceId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all activities with filtering
    /// </summary>
    Task<PagedResult<UserActivityDto>> GetAllActivitiesAsync(int pageNumber, int pageSize, ActivityType? activityType = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default);
}
