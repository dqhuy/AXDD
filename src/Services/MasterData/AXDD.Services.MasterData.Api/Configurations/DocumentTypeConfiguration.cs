using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.MasterData.Api.Configurations;

/// <summary>
/// Entity configuration for DocumentType
/// </summary>
public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.ToTable("DocumentTypes");

        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dt => dt.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(dt => dt.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(dt => dt.Description)
            .HasMaxLength(2000);

        builder.Property(dt => dt.AllowedExtensions)
            .HasMaxLength(200);

        builder.HasIndex(dt => dt.Code)
            .IsUnique();

        builder.HasIndex(dt => dt.Name);

        builder.HasIndex(dt => dt.Category);

        builder.HasIndex(dt => dt.IsActive);
    }
}
