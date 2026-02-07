using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for IndustryCode
/// </summary>
public class IndustryCodeConfiguration : IEntityTypeConfiguration<IndustryCode>
{
    public void Configure(EntityTypeBuilder<IndustryCode> builder)
    {
        builder.ToTable("IndustryCodes");

        builder.HasKey(ic => ic.Id);

        builder.Property(ic => ic.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ic => ic.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ic => ic.Description)
            .HasMaxLength(2000);

        builder.Property(ic => ic.ParentCode)
            .HasMaxLength(20);

        builder.HasIndex(ic => ic.Code)
            .IsUnique();

        builder.HasIndex(ic => ic.Name);

        builder.HasIndex(ic => ic.ParentCode);

        builder.HasIndex(ic => ic.Level);

        builder.HasIndex(ic => ic.IsActive);
    }
}
