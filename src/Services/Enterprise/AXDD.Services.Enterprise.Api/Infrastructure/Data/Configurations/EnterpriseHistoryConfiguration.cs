using AXDD.Services.Enterprise.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Enterprise.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for EnterpriseHistory
/// </summary>
public class EnterpriseHistoryConfiguration : IEntityTypeConfiguration<EnterpriseHistory>
{
    public void Configure(EntityTypeBuilder<EnterpriseHistory> builder)
    {
        builder.ToTable("EnterpriseHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.ChangedBy)
            .HasMaxLength(255);

        builder.Property(h => h.ChangeType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(h => h.FieldName)
            .HasMaxLength(200);

        builder.Property(h => h.OldValue)
            .HasMaxLength(2000);

        builder.Property(h => h.NewValue)
            .HasMaxLength(2000);

        builder.Property(h => h.Reason)
            .HasMaxLength(1000);

        builder.Property(h => h.Details)
            .HasMaxLength(2000);

        builder.Property(h => h.CreatedBy)
            .HasMaxLength(255);

        builder.Property(h => h.UpdatedBy)
            .HasMaxLength(255);

        builder.Property(h => h.DeletedBy)
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(h => h.EnterpriseId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(h => h.ChangedAt)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(h => h.ChangeType)
            .HasFilter("[IsDeleted] = 0");
    }
}
