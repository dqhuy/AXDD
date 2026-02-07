using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for Configuration
/// </summary>
public class ConfigurationConfiguration : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> builder)
    {
        builder.ToTable("Configurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Value)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.DataType)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => c.Key)
            .IsUnique();

        builder.HasIndex(c => c.Category);

        builder.HasIndex(c => c.IsActive);
    }
}
