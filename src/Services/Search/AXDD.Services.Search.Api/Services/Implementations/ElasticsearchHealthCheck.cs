using AXDD.Services.Search.Api.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AXDD.Services.Search.Api.Services.Implementations;

/// <summary>
/// Health check for Elasticsearch connectivity
/// </summary>
public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly IElasticsearchClientFactory _clientFactory;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(
        IElasticsearchClientFactory clientFactory,
        ILogger<ElasticsearchHealthCheck> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = await _clientFactory.IsHealthyAsync(cancellationToken);

            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Elasticsearch is responding");
            }

            return HealthCheckResult.Unhealthy("Elasticsearch is not responding");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch health check failed");
            return HealthCheckResult.Unhealthy(
                "Elasticsearch health check failed", 
                ex);
        }
    }
}
