using AXDD.Services.Logging.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Logging.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for AuditLog entity
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Username)
            .HasMaxLength(100);

        builder.Property(x => x.UserRole)
            .HasMaxLength(50);

        builder.Property(x => x.ActionName)
            .HasMaxLength(100);

        builder.Property(x => x.EntityType)
            .HasMaxLength(100);

        builder.Property(x => x.HttpMethod)
            .HasMaxLength(10);

        builder.Property(x => x.RequestPath)
            .HasMaxLength(500);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.Property(x => x.ExceptionMessage)
            .HasMaxLength(2000);

        // Indexes for performance
        builder.HasIndex(x => x.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.HasIndex(x => x.ServiceName)
            .HasDatabaseName("IX_AuditLogs_ServiceName");

        builder.HasIndex(x => x.Level)
            .HasDatabaseName("IX_AuditLogs_Level");

        builder.HasIndex(x => x.CorrelationId)
            .HasDatabaseName("IX_AuditLogs_CorrelationId");

        builder.HasIndex(x => new { x.Timestamp, x.ServiceName })
            .HasDatabaseName("IX_AuditLogs_Timestamp_ServiceName");
    }
}
