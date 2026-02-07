using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Logging.Api.Controllers;

/// <summary>
/// Controller for user activity operations
/// </summary>
[ApiController]
[Route("api/v1/logs/activities")]
[Produces("application/json")]
public class UserActivitiesController : ControllerBase
{
    private readonly IUserActivityService _activityService;

    public UserActivitiesController(IUserActivityService activityService)
    {
        _activityService = activityService;
    }

    /// <summary>
    /// Gets all user activities with filtering and pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="activityType">Optional activity type filter</param>
    /// <param name="dateFrom">Optional start date filter</param>
    /// <param name="dateTo">Optional end date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user activities</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserActivityDto>>> GetAllActivitiesAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] ActivityType? activityType = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _activityService.GetAllActivitiesAsync(pageNumber, pageSize, activityType, dateFrom, dateTo, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets user activities by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="activityType">Optional activity type filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user activities</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(PagedResult<UserActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserActivityDto>>> GetUserActivitiesAsync(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] ActivityType? activityType = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _activityService.GetUserActivitiesAsync(userId, pageNumber, pageSize, activityType, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets recent activities across all users
    /// </summary>
    /// <param name="count">Number of recent activities to retrieve (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of recent user activities</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(List<UserActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserActivityDto>>> GetRecentActivitiesAsync(
        [FromQuery] int count = 50,
        CancellationToken cancellationToken = default)
    {
        var activities = await _activityService.GetRecentActivitiesAsync(count, cancellationToken);
        return Ok(activities);
    }

    /// <summary>
    /// Gets activities by resource
    /// </summary>
    /// <param name="resourceType">Resource type</param>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of activities for the resource</returns>
    [HttpGet("resource/{resourceType}/{resourceId:guid}")]
    [ProducesResponseType(typeof(PagedResult<UserActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserActivityDto>>> GetActivitiesByResourceAsync(
        string resourceType,
        Guid resourceId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _activityService.GetActivitiesByResourceAsync(resourceType, resourceId, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Logs a new user activity
    /// </summary>
    /// <param name="request">User activity creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user activity</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserActivityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserActivityDto>> LogActivityAsync(
        [FromBody] CreateUserActivityRequest request,
        CancellationToken cancellationToken)
    {
        var activity = await _activityService.LogActivityAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetUserActivitiesAsync), new { userId = activity.UserId }, activity);
    }
}
