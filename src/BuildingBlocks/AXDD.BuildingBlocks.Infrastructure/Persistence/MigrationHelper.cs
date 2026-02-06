using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AXDD.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Helper methods for database migrations
/// </summary>
public static class MigrationHelper
{
    /// <summary>
    /// Applies pending migrations for the specified DbContext
    /// </summary>
    /// <typeparam name="TContext">DbContext type</typeparam>
    /// <param name="host">Application host</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task MigrateDatabaseAsync<TContext>(
        this IHost host,
        CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database for {DbContext}...", typeof(TContext).Name);
            
            await context.Database.MigrateAsync(cancellationToken);
            
            logger.LogInformation("Database migration completed for {DbContext}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database for {DbContext}", typeof(TContext).Name);
            throw;
        }
    }

    /// <summary>
    /// Ensures the database is created for the specified DbContext
    /// </summary>
    /// <typeparam name="TContext">DbContext type</typeparam>
    /// <param name="host">Application host</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task EnsureDatabaseCreatedAsync<TContext>(
        this IHost host,
        CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Ensuring database is created for {DbContext}...", typeof(TContext).Name);
            
            var created = await context.Database.EnsureCreatedAsync(cancellationToken);
            
            if (created)
            {
                logger.LogInformation("Database created for {DbContext}", typeof(TContext).Name);
            }
            else
            {
                logger.LogInformation("Database already exists for {DbContext}", typeof(TContext).Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the database for {DbContext}", typeof(TContext).Name);
            throw;
        }
    }
}
