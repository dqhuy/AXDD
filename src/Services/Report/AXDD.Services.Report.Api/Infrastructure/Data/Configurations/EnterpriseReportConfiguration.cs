using AXDD.Services.Report.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Report.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for EnterpriseReport
/// </summary>
public class EnterpriseReportConfiguration : IEntityTypeConfiguration<EnterpriseReport>
{
    public void Configure(EntityTypeBuilder<EnterpriseReport> builder)
    {
        builder.ToTable("EnterpriseReports");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnterpriseId)
            .IsRequired();

        builder.Property(e => e.EnterpriseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.EnterpriseCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ReportType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.ReportPeriod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Year)
            .IsRequired();

        builder.Property(e => e.Month);

        builder.Property(e => e.SubmittedDate)
            .IsRequired();

        builder.Property(e => e.SubmittedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.DataJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Attachments)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.ReviewedBy);

        builder.Property(e => e.ReviewedDate);

        builder.Property(e => e.ReviewerNotes)
            .HasMaxLength(2000);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(1000);

        // Indexes for better query performance
        builder.HasIndex(e => e.EnterpriseId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.ReportType);
        builder.HasIndex(e => e.SubmittedDate);
        builder.HasIndex(e => e.Year);
        builder.HasIndex(e => e.SubmittedBy);

        // Composite index for uniqueness check
        builder.HasIndex(e => new { e.EnterpriseId, e.ReportType, e.ReportPeriod, e.Year, e.Month })
            .HasDatabaseName("IX_EnterpriseReports_Unique");
    }
}
