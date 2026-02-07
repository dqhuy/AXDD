using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for Province
/// </summary>
public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Provinces");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Region)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.Region);

        // Configure relationships
        builder.HasMany(p => p.Districts)
            .WithOne(d => d.Province)
            .HasForeignKey(d => d.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.IndustrialZones)
            .WithOne(iz => iz.Province)
            .HasForeignKey(iz => iz.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
