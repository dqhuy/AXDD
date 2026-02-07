using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for error log operations
/// </summary>
public interface IErrorLogService
{
    /// <summary>
    /// Logs an error
    /// </summary>
    Task<ErrorLogDto> LogErrorAsync(CreateErrorLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets errors with filtering and pagination
    /// </summary>
    Task<PagedResult<ErrorLogDto>> GetErrorsAsync(int pageNumber, int pageSize, ErrorSeverity? severity = null, bool? isResolved = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an error by ID
    /// </summary>
    Task<ErrorLogDto> GetErrorByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unresolved errors
    /// </summary>
    Task<PagedResult<ErrorLogDto>> GetUnresolvedErrorsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves an error
    /// </summary>
    Task<ErrorLogDto> ResolveErrorAsync(Guid errorId, ResolveErrorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets errors by service name
    /// </summary>
    Task<PagedResult<ErrorLogDto>> GetErrorsByServiceAsync(string serviceName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets critical errors
    /// </summary>
    Task<PagedResult<ErrorLogDto>> GetCriticalErrorsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
