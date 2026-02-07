using AXDD.Services.Logging.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Logging.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for PerformanceLog entity
/// </summary>
public class PerformanceLogConfiguration : IEntityTypeConfiguration<PerformanceLog>
{
    public void Configure(EntityTypeBuilder<PerformanceLog> builder)
    {
        builder.ToTable("PerformanceLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EndpointName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.HttpMethod)
            .HasMaxLength(10);

        // Indexes
        builder.HasIndex(x => x.Timestamp)
            .HasDatabaseName("IX_PerformanceLogs_Timestamp");

        builder.HasIndex(x => x.ServiceName)
            .HasDatabaseName("IX_PerformanceLogs_ServiceName");

        builder.HasIndex(x => x.DurationMs)
            .HasDatabaseName("IX_PerformanceLogs_DurationMs");

        builder.HasIndex(x => new { x.ServiceName, x.EndpointName })
            .HasDatabaseName("IX_PerformanceLogs_Service_Endpoint");
    }
}
