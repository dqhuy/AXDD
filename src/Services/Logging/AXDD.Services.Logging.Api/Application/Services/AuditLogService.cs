using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Exceptions;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Application.Services;

/// <summary>
/// Service for audit log operations
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly LogDbContext _context;

    public AuditLogService(LogDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLogDto> CreateLogAsync(CreateAuditLogRequest request, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            Level = request.Level,
            UserId = request.UserId,
            Username = request.Username,
            UserRole = request.UserRole,
            ServiceName = request.ServiceName,
            ActionName = request.ActionName,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            HttpMethod = request.HttpMethod,
            RequestPath = request.RequestPath,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            RequestBody = request.RequestBody,
            ResponseBody = request.ResponseBody,
            StatusCode = request.StatusCode,
            DurationMs = request.DurationMs,
            Message = request.Message,
            ExceptionMessage = request.ExceptionMessage,
            StackTrace = request.StackTrace,
            CorrelationId = request.CorrelationId,
            AdditionalData = request.AdditionalData
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(log);
    }

    public async Task<PagedResult<AuditLogDto>> GetLogsAsync(LogFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        // Apply filters
        if (filter.DateFrom.HasValue)
            query = query.Where(x => x.Timestamp >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(x => x.Timestamp <= filter.DateTo.Value);

        if (filter.Level.HasValue)
            query = query.Where(x => x.Level == filter.Level.Value);

        if (!string.IsNullOrWhiteSpace(filter.ServiceName))
            query = query.Where(x => x.ServiceName == filter.ServiceName);

        if (filter.UserId.HasValue)
            query = query.Where(x => x.UserId == filter.UserId.Value);

        if (!string.IsNullOrWhiteSpace(filter.CorrelationId))
            query = query.Where(x => x.CorrelationId == filter.CorrelationId);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(x => x.Message.ToLower().Contains(searchTerm) ||
                                    (x.ActionName != null && x.ActionName.ToLower().Contains(searchTerm)) ||
                                    (x.EntityType != null && x.EntityType.ToLower().Contains(searchTerm)));
        }

        // Apply sorting
        query = filter.SortDirection.ToLower() == "asc"
            ? query.OrderBy(x => EF.Property<object>(x, filter.SortBy))
            : query.OrderByDescending(x => EF.Property<object>(x, filter.SortBy));

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<AuditLogDto> GetLogByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var log = await _context.AuditLogs.FindAsync(new object[] { id }, cancellationToken);
        if (log == null)
            throw new LogNotFoundException(id);

        return MapToDto(log);
    }

    public async Task<PagedResult<AuditLogDto>> GetLogsByUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<AuditLogDto>> GetLogsByServiceAsync(string serviceName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Where(x => x.ServiceName == serviceName)
            .OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<AuditLogDto>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate)
            .OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<List<AuditLogDto>> GetLogsByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(x => x.CorrelationId == correlationId)
            .OrderBy(x => x.Timestamp)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteOldLogsAsync(int olderThanDays, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var logsToDelete = await _context.AuditLogs
            .Where(x => x.Timestamp < cutoffDate)
            .ToListAsync(cancellationToken);

        _context.AuditLogs.RemoveRange(logsToDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return logsToDelete.Count;
    }

    private static AuditLogDto MapToDto(AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Level = log.Level,
            UserId = log.UserId,
            Username = log.Username,
            UserRole = log.UserRole,
            ServiceName = log.ServiceName,
            ActionName = log.ActionName,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            HttpMethod = log.HttpMethod,
            RequestPath = log.RequestPath,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            RequestBody = log.RequestBody,
            ResponseBody = log.ResponseBody,
            StatusCode = log.StatusCode,
            DurationMs = log.DurationMs,
            Message = log.Message,
            ExceptionMessage = log.ExceptionMessage,
            StackTrace = log.StackTrace,
            CorrelationId = log.CorrelationId,
            AdditionalData = log.AdditionalData
        };
    }
}
