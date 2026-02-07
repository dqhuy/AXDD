using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for CertificateType
/// </summary>
public class CertificateTypeConfiguration : IEntityTypeConfiguration<CertificateType>
{
    public void Configure(EntityTypeBuilder<CertificateType> builder)
    {
        builder.ToTable("CertificateTypes");

        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ct => ct.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(ct => ct.Description)
            .HasMaxLength(2000);

        builder.Property(ct => ct.RequiringAuthority)
            .HasMaxLength(300);

        builder.HasIndex(ct => ct.Code)
            .IsUnique();

        builder.HasIndex(ct => ct.Name);

        builder.HasIndex(ct => ct.IsActive);
    }
}
