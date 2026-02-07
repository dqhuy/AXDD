using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for StatusCode
/// </summary>
public class StatusCodeConfiguration : IEntityTypeConfiguration<StatusCode>
{
    public void Configure(EntityTypeBuilder<StatusCode> builder)
    {
        builder.ToTable("StatusCodes");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sc => sc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sc => sc.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sc => sc.Description)
            .HasMaxLength(1000);

        builder.Property(sc => sc.Color)
            .HasMaxLength(20);

        builder.HasIndex(sc => sc.Code);

        builder.HasIndex(sc => sc.EntityType);

        builder.HasIndex(sc => sc.IsActive);

        // Composite unique index on Code and EntityType
        builder.HasIndex(sc => new { sc.Code, sc.EntityType })
            .IsUnique();
    }
}
