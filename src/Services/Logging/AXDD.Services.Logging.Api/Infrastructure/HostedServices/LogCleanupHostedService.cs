using AXDD.Services.Logging.Api.Application.Services.Interfaces;

namespace AXDD.Services.Logging.Api.Infrastructure.HostedServices;

/// <summary>
/// Background service for cleaning up old logs
/// </summary>
public class LogCleanupHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LogCleanupHostedService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run once per day

    public LogCleanupHostedService(IServiceProvider serviceProvider, ILogger<LogCleanupHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Log Cleanup Hosted Service is starting");

        // Wait for a short delay before starting the first run
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldLogsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up old logs");
            }

            // Wait for the next check interval
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Log Cleanup Hosted Service is stopping");
    }

    private async Task CleanupOldLogsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting log cleanup process");

        using var scope = _serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();

        // Get retention days from configuration
        var retentionDays = configuration.GetValue<int>("LoggingSettings:RetentionDays", 90);

        if (retentionDays <= 0)
        {
            _logger.LogWarning("Invalid retention days configuration: {Days}. Skipping cleanup.", retentionDays);
            return;
        }

        try
        {
            var deletedCount = await auditLogService.DeleteOldLogsAsync(retentionDays, cancellationToken);
            _logger.LogInformation("Log cleanup completed. Deleted {Count} audit logs older than {Days} days", deletedCount, retentionDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting old logs");
            throw;
        }
    }
}
