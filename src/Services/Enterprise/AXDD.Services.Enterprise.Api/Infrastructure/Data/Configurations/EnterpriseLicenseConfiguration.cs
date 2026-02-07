using AXDD.Services.Enterprise.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Enterprise.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for EnterpriseLicense
/// </summary>
public class EnterpriseLicenseConfiguration : IEntityTypeConfiguration<EnterpriseLicense>
{
    public void Configure(EntityTypeBuilder<EnterpriseLicense> builder)
    {
        builder.ToTable("EnterpriseLicenses");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LicenseType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(l => l.LicenseNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.IssuingAuthority)
            .HasMaxLength(500);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Notes)
            .HasMaxLength(1000);

        builder.Property(l => l.CreatedBy)
            .HasMaxLength(255);

        builder.Property(l => l.UpdatedBy)
            .HasMaxLength(255);

        builder.Property(l => l.DeletedBy)
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(l => l.EnterpriseId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(l => l.LicenseType)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(l => l.ExpiryDate)
            .HasFilter("[IsDeleted] = 0");
    }
}
