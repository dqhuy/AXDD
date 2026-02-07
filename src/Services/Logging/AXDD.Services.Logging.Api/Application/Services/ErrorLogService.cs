using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Enums;
using AXDD.Services.Logging.Api.Domain.Exceptions;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Application.Services;

/// <summary>
/// Service for error log operations
/// </summary>
public class ErrorLogService : IErrorLogService
{
    private readonly LogDbContext _context;

    public ErrorLogService(LogDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorLogDto> LogErrorAsync(CreateErrorLogRequest request, CancellationToken cancellationToken = default)
    {
        var error = new ErrorLog
        {
            Timestamp = DateTime.UtcNow,
            ServiceName = request.ServiceName,
            ErrorMessage = request.ErrorMessage,
            StackTrace = request.StackTrace,
            Severity = request.Severity,
            UserId = request.UserId,
            RequestPath = request.RequestPath,
            ExceptionType = request.ExceptionType,
            CorrelationId = request.CorrelationId,
            AdditionalData = request.AdditionalData
        };

        _context.ErrorLogs.Add(error);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(error);
    }

    public async Task<PagedResult<ErrorLogDto>> GetErrorsAsync(int pageNumber, int pageSize, ErrorSeverity? severity = null, bool? isResolved = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ErrorLogs.AsQueryable();

        if (severity.HasValue)
            query = query.Where(x => x.Severity == severity.Value);

        if (isResolved.HasValue)
            query = query.Where(x => x.IsResolved == isResolved.Value);

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

        return new PagedResult<ErrorLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ErrorLogDto> GetErrorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var error = await _context.ErrorLogs.FindAsync(new object[] { id }, cancellationToken);
        if (error == null)
            throw new LogNotFoundException(id);

        return MapToDto(error);
    }

    public async Task<PagedResult<ErrorLogDto>> GetUnresolvedErrorsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ErrorLogs
            .Where(x => !x.IsResolved)
            .OrderByDescending(x => x.Severity)
            .ThenByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<ErrorLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ErrorLogDto> ResolveErrorAsync(Guid errorId, ResolveErrorRequest request, CancellationToken cancellationToken = default)
    {
        var error = await _context.ErrorLogs.FindAsync(new object[] { errorId }, cancellationToken);
        if (error == null)
            throw new LogNotFoundException(errorId);

        error.IsResolved = true;
        error.ResolvedBy = request.ResolvedBy;
        error.ResolvedAt = DateTime.UtcNow;
        error.Resolution = request.Resolution;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(error);
    }

    public async Task<PagedResult<ErrorLogDto>> GetErrorsByServiceAsync(string serviceName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ErrorLogs
            .Where(x => x.ServiceName == serviceName)
            .OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<ErrorLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<ErrorLogDto>> GetCriticalErrorsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ErrorLogs
            .Where(x => x.Severity == ErrorSeverity.Critical)
            .OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<ErrorLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static ErrorLogDto MapToDto(ErrorLog error)
    {
        return new ErrorLogDto
        {
            Id = error.Id,
            Timestamp = error.Timestamp,
            ServiceName = error.ServiceName,
            ErrorMessage = error.ErrorMessage,
            StackTrace = error.StackTrace,
            Severity = error.Severity,
            UserId = error.UserId,
            RequestPath = error.RequestPath,
            ExceptionType = error.ExceptionType,
            IsResolved = error.IsResolved,
            ResolvedBy = error.ResolvedBy,
            ResolvedAt = error.ResolvedAt,
            Resolution = error.Resolution,
            CorrelationId = error.CorrelationId,
            AdditionalData = error.AdditionalData
        };
    }
}
