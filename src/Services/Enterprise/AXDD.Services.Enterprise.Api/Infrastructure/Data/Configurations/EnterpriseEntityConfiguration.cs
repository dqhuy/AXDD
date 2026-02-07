using AXDD.Services.Enterprise.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Enterprise.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for EnterpriseEntity
/// </summary>
public class EnterpriseEntityConfiguration : IEntityTypeConfiguration<EnterpriseEntity>
{
    public void Configure(EntityTypeBuilder<EnterpriseEntity> builder)
    {
        builder.ToTable("Enterprises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.TaxCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.EnglishName)
            .HasMaxLength(500);

        builder.Property(e => e.ShortName)
            .HasMaxLength(200);

        builder.Property(e => e.IndustryCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.IndustryName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.IndustrialZoneName)
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.LegalRepresentative)
            .HasMaxLength(200);

        builder.Property(e => e.Position)
            .HasMaxLength(200);

        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Ward)
            .HasMaxLength(200);

        builder.Property(e => e.District)
            .HasMaxLength(200);

        builder.Property(e => e.Province)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.Fax)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(255);

        builder.Property(e => e.Website)
            .HasMaxLength(500);

        builder.Property(e => e.RegisteredCapital)
            .HasPrecision(18, 2);

        builder.Property(e => e.CharterCapital)
            .HasPrecision(18, 2);

        builder.Property(e => e.AnnualRevenue)
            .HasPrecision(18, 2);

        builder.Property(e => e.ProductionCapacity)
            .HasMaxLength(1000);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(255);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(255);

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.TaxCode)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.IndustrialZoneId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.IndustryCode)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.Status)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.Name)
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasMany(e => e.Contacts)
            .WithOne(c => c.Enterprise)
            .HasForeignKey(c => c.EnterpriseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Licenses)
            .WithOne(l => l.Enterprise)
            .HasForeignKey(l => l.EnterpriseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.History)
            .WithOne(h => h.Enterprise)
            .HasForeignKey(h => h.EnterpriseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
