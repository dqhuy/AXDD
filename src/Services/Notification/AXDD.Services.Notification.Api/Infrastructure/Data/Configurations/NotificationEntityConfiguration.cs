using AXDD.Services.Notification.Api.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Notification.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for NotificationEntity
/// </summary>
public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        builder.ToTable("Notifications");

        // Primary key
        builder.HasKey(n => n.Id);

        // Properties
        builder.Property(n => n.UserId)
            .IsRequired();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.ReadAt)
            .IsRequired(false);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(n => n.RelatedEntityId)
            .IsRequired(false);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        builder.Property(n => n.Data)
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(n => n.UserId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(n => new { n.UserId, n.IsRead })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(n => n.CreatedAt)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(n => new { n.RelatedEntityType, n.RelatedEntityId })
            .HasFilter("[IsDeleted] = 0 AND [RelatedEntityType] IS NOT NULL");
    }
}
