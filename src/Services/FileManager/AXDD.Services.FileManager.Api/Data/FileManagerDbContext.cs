using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.Services.FileManager.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Data;

/// <summary>
/// Database context for FileManager service
/// </summary>
public class FileManagerDbContext : BaseDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of FileManagerDbContext
    /// </summary>
    /// <param name="options">DbContext options</param>
    /// <param name="httpContextAccessor">HTTP context accessor for audit information</param>
    public FileManagerDbContext(
        DbContextOptions<FileManagerDbContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options, httpContextAccessor.HttpContext?.User?.Identity?.Name)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets or sets the FileMetadata entities
    /// </summary>
    public DbSet<FileMetadata> FileMetadata { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Folder entities
    /// </summary>
    public DbSet<Folder> Folders { get; set; } = null!;

    /// <summary>
    /// Gets or sets the FileVersion entities
    /// </summary>
    public DbSet<FileVersion> FileVersions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the FileShare entities
    /// </summary>
    public DbSet<Entities.FileShare> FileShares { get; set; } = null!;

    /// <summary>
    /// Gets or sets the StorageQuota entities
    /// </summary>
    public DbSet<StorageQuota> StorageQuotas { get; set; } = null!;

    /// <summary>
    /// Configures the entity models
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // FileMetadata configuration
        modelBuilder.Entity<FileMetadata>(entity =>
        {
            entity.ToTable("FileMetadata");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.MimeType)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Extension)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.BucketName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ObjectKey)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.EnterpriseCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.UploadedBy)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Checksum)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Tags)
                .HasMaxLength(500);

            entity.HasOne(e => e.Folder)
                .WithMany(f => f.Files)
                .HasForeignKey(e => e.FolderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Versions)
                .WithOne(v => v.FileMetadata)
                .HasForeignKey(v => v.FileMetadataId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Shares)
                .WithOne(s => s.FileMetadata)
                .HasForeignKey(s => s.FileMetadataId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.EnterpriseCode);
            entity.HasIndex(e => e.FolderId);
            entity.HasIndex(e => new { e.EnterpriseCode, e.IsLatest });
            entity.HasIndex(e => e.Checksum);
        });

        // Folder configuration
        modelBuilder.Entity<Folder>(entity =>
        {
            entity.ToTable("Folders");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.EnterpriseCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Path)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.HasOne(e => e.ParentFolder)
                .WithMany(f => f.ChildFolders)
                .HasForeignKey(e => e.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.EnterpriseCode);
            entity.HasIndex(e => e.ParentFolderId);
            entity.HasIndex(e => new { e.EnterpriseCode, e.ParentFolderId });
        });

        // FileVersion configuration
        modelBuilder.Entity<FileVersion>(entity =>
        {
            entity.ToTable("FileVersions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ObjectKey)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.UploadedBy)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Checksum)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            entity.HasIndex(e => e.FileMetadataId);
            entity.HasIndex(e => new { e.FileMetadataId, e.Version }).IsUnique();
        });

        // FileShare configuration
        modelBuilder.Entity<Entities.FileShare>(entity =>
        {
            entity.ToTable("FileShares");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SharedWithUserId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Permission)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SharedBy)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.FileMetadataId);
            entity.HasIndex(e => e.SharedWithUserId);
            entity.HasIndex(e => new { e.FileMetadataId, e.SharedWithUserId });
        });

        // StorageQuota configuration
        modelBuilder.Entity<StorageQuota>(entity =>
        {
            entity.ToTable("StorageQuotas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EnterpriseCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.EnterpriseCode).IsUnique();
        });
    }
}
