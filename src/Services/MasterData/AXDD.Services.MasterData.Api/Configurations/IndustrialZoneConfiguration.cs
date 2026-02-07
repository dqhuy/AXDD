using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for IndustrialZone
/// </summary>
public class IndustrialZoneConfiguration : IEntityTypeConfiguration<IndustrialZone>
{
    public void Configure(EntityTypeBuilder<IndustrialZone> builder)
    {
        builder.ToTable("IndustrialZones");

        builder.HasKey(iz => iz.Id);

        builder.Property(iz => iz.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(iz => iz.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(iz => iz.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(iz => iz.Area)
            .HasPrecision(18, 2);

        builder.Property(iz => iz.ManagementUnit)
            .HasMaxLength(300);

        builder.Property(iz => iz.Description)
            .HasMaxLength(2000);

        builder.HasIndex(iz => iz.Code)
            .IsUnique();

        builder.HasIndex(iz => iz.Name);

        builder.HasIndex(iz => iz.ProvinceId);

        builder.HasIndex(iz => iz.DistrictId);

        builder.HasIndex(iz => iz.Status);

        // Configure relationships
        builder.HasOne(iz => iz.Province)
            .WithMany(p => p.IndustrialZones)
            .HasForeignKey(iz => iz.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(iz => iz.District)
            .WithMany(d => d.IndustrialZones)
            .HasForeignKey(iz => iz.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
