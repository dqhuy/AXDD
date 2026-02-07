using AXDD.Services.Logging.Api.Application.DTOs;

namespace AXDD.Services.Logging.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for dashboard operations
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets dashboard summary
    /// </summary>
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}
