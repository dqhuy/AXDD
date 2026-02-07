using AXDD.Services.Logging.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Logging.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for ErrorLog entity
/// </summary>
public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.RequestPath)
            .HasMaxLength(500);

        builder.Property(x => x.ExceptionType)
            .HasMaxLength(200);

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.Property(x => x.Resolution)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(x => x.Timestamp)
            .HasDatabaseName("IX_ErrorLogs_Timestamp");

        builder.HasIndex(x => x.ServiceName)
            .HasDatabaseName("IX_ErrorLogs_ServiceName");

        builder.HasIndex(x => x.Severity)
            .HasDatabaseName("IX_ErrorLogs_Severity");

        builder.HasIndex(x => x.IsResolved)
            .HasDatabaseName("IX_ErrorLogs_IsResolved");

        builder.HasIndex(x => new { x.Severity, x.IsResolved })
            .HasDatabaseName("IX_ErrorLogs_Severity_IsResolved");
    }
}
