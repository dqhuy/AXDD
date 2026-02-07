using AXDD.Services.Logging.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Logging.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for UserActivityLog entity
/// </summary>
public class UserActivityLogConfiguration : IEntityTypeConfiguration<UserActivityLog>
{
    public void Configure(EntityTypeBuilder<UserActivityLog> builder)
    {
        builder.ToTable("UserActivityLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ActivityDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);

        builder.Property(x => x.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(x => x.ResourceType)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserActivityLogs_UserId");

        builder.HasIndex(x => x.Timestamp)
            .HasDatabaseName("IX_UserActivityLogs_Timestamp");

        builder.HasIndex(x => x.ActivityType)
            .HasDatabaseName("IX_UserActivityLogs_ActivityType");

        builder.HasIndex(x => new { x.ResourceType, x.ResourceId })
            .HasDatabaseName("IX_UserActivityLogs_Resource");
    }
}
