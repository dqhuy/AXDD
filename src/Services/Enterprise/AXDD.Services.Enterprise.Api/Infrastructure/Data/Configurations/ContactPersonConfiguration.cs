using AXDD.Services.Enterprise.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Enterprise.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ContactPerson
/// </summary>
public class ContactPersonConfiguration : IEntityTypeConfiguration<ContactPerson>
{
    public void Configure(EntityTypeBuilder<ContactPerson> builder)
    {
        builder.ToTable("ContactPersons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Position)
            .HasMaxLength(200);

        builder.Property(c => c.Department)
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(255);

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(255);

        builder.Property(c => c.DeletedBy)
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(c => c.EnterpriseId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => c.IsMain)
            .HasFilter("[IsDeleted] = 0 AND [IsMain] = 1");
    }
}
