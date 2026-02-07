using AXDD.Services.Notification.Api.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AXDD.Services.Notification.Api.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for NotificationTemplate
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.TemplateKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.BodyTemplate)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.ChannelType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(t => t.TemplateKey)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(t => t.IsActive)
            .HasFilter("[IsDeleted] = 0");
    }
}
