using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Enums;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Application.Services;

/// <summary>
/// Service for user activity operations
/// </summary>
public class UserActivityService : IUserActivityService
{
    private readonly LogDbContext _context;

    public UserActivityService(LogDbContext context)
    {
        _context = context;
    }

    public async Task<UserActivityDto> LogActivityAsync(CreateUserActivityRequest request, CancellationToken cancellationToken = default)
    {
        var activity = new UserActivityLog
        {
            UserId = request.UserId,
            Username = request.Username,
            ActivityType = request.ActivityType,
            ActivityDescription = request.ActivityDescription,
            Timestamp = DateTime.UtcNow,
            IpAddress = request.IpAddress,
            DeviceInfo = request.DeviceInfo,
            ResourceType = request.ResourceType,
            ResourceId = request.ResourceId,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            AdditionalData = request.AdditionalData
        };

        _context.UserActivityLogs.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(activity);
    }

    public async Task<PagedResult<UserActivityDto>> GetUserActivitiesAsync(Guid userId, int pageNumber, int pageSize, ActivityType? activityType = null, CancellationToken cancellationToken = default)
    {
        var query = _context.UserActivityLogs
            .Where(x => x.UserId == userId);

        if (activityType.HasValue)
            query = query.Where(x => x.ActivityType == activityType.Value);

        query = query.OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserActivityDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<List<UserActivityDto>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.UserActivityLogs
            .OrderByDescending(x => x.Timestamp)
            .Take(count)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<UserActivityDto>> GetActivitiesByResourceAsync(string resourceType, Guid resourceId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.UserActivityLogs
            .Where(x => x.ResourceType == resourceType && x.ResourceId == resourceId)
            .OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserActivityDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<UserActivityDto>> GetAllActivitiesAsync(int pageNumber, int pageSize, ActivityType? activityType = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default)
    {
        var query = _context.UserActivityLogs.AsQueryable();

        if (activityType.HasValue)
            query = query.Where(x => x.ActivityType == activityType.Value);

        if (dateFrom.HasValue)
            query = query.Where(x => x.Timestamp >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(x => x.Timestamp <= dateTo.Value);

        query = query.OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserActivityDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static UserActivityDto MapToDto(UserActivityLog activity)
    {
        return new UserActivityDto
        {
            Id = activity.Id,
            UserId = activity.UserId,
            Username = activity.Username,
            ActivityType = activity.ActivityType,
            ActivityDescription = activity.ActivityDescription,
            Timestamp = activity.Timestamp,
            IpAddress = activity.IpAddress,
            DeviceInfo = activity.DeviceInfo,
            ResourceType = activity.ResourceType,
            ResourceId = activity.ResourceId,
            OldValue = activity.OldValue,
            NewValue = activity.NewValue,
            AdditionalData = activity.AdditionalData
        };
    }
}
