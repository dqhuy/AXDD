using AXDD.Services.Logging.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Infrastructure.Data;

/// <summary>
/// Database context for logging service
/// </summary>
public class LogDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogDbContext"/> class
    /// </summary>
    /// <param name="options">Database context options</param>
    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the audit logs
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// Gets or sets the user activity logs
    /// </summary>
    public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();

    /// <summary>
    /// Gets or sets the error logs
    /// </summary>
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();

    /// <summary>
    /// Gets or sets the performance logs
    /// </summary>
    public DbSet<PerformanceLog> PerformanceLogs => Set<PerformanceLog>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogDbContext).Assembly);
    }
}
