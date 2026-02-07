using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.Services.Notification.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Notification.Api.Infrastructure.Data;

/// <summary>
/// Database context for the Notification service
/// </summary>
public class NotificationDbContext : BaseDbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
    }
}
