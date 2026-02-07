using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.BuildingBlocks.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AXDD.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
/// Dependency injection extensions for infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds database infrastructure services
    /// </summary>
    /// <typeparam name="TContext">DbContext type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="configureOptions">Optional DbContext configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddDatabaseInfrastructure<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? configureOptions = null) 
        where TContext : DbContext
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });

            configureOptions?.Invoke(options);
        });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));

        return services;
    }

    /// <summary>
    /// Adds SQL Server connection factory
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddSqlConnectionFactory(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

        return services;
    }
}
