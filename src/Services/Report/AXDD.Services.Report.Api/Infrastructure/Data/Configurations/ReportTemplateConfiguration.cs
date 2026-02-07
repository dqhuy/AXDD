using AXDD.Services.Report.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Report.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ReportTemplate
/// </summary>
public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
{
    public void Configure(EntityTypeBuilder<ReportTemplate> builder)
    {
        builder.ToTable("ReportTemplates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ReportType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.TemplateName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.FieldsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Indexes
        builder.HasIndex(e => e.ReportType);
        builder.HasIndex(e => e.TemplateName).IsUnique();
        builder.HasIndex(e => e.IsActive);
    }
}
