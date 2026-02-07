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
    /// Gets or sets the DocumentProfile entities
    /// </summary>
    public DbSet<DocumentProfile> DocumentProfiles { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ProfileMetadataField entities
    /// </summary>
    public DbSet<ProfileMetadataField> ProfileMetadataFields { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DocumentProfileDocument entities
    /// </summary>
    public DbSet<DocumentProfileDocument> DocumentProfileDocuments { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DocumentMetadataValue entities
    /// </summary>
    public DbSet<DocumentMetadataValue> DocumentMetadataValues { get; set; } = null!;

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

        // DocumentProfile configuration
        modelBuilder.Entity<DocumentProfile>(entity =>
        {
            entity.ToTable("DocumentProfiles");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.EnterpriseCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.ProfileType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Path)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(e => e.ParentProfile)
                .WithMany(p => p.ChildProfiles)
                .HasForeignKey(e => e.ParentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.EnterpriseCode);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => new { e.EnterpriseCode, e.Code }).IsUnique()
                .HasFilter("[IsDeleted] = 0");
            entity.HasIndex(e => e.ParentProfileId);
            entity.HasIndex(e => e.ProfileType);
            entity.HasIndex(e => e.Status);
        });

        // ProfileMetadataField configuration
        modelBuilder.Entity<ProfileMetadataField>(entity =>
        {
            entity.ToTable("ProfileMetadataFields");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FieldName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.DisplayLabel)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.DataType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DefaultValue)
                .HasMaxLength(1000);

            entity.Property(e => e.Placeholder)
                .HasMaxLength(255);

            entity.Property(e => e.SelectOptions)
                .HasMaxLength(4000);

            entity.Property(e => e.ValidationPattern)
                .HasMaxLength(500);

            entity.Property(e => e.ValidationMessage)
                .HasMaxLength(500);

            entity.Property(e => e.HelpText)
                .HasMaxLength(1000);

            entity.HasOne(e => e.Profile)
                .WithMany(p => p.MetadataFields)
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProfileId);
            entity.HasIndex(e => new { e.ProfileId, e.FieldName }).IsUnique()
                .HasFilter("[IsDeleted] = 0");
        });

        // DocumentProfileDocument configuration
        modelBuilder.Entity<DocumentProfileDocument>(entity =>
        {
            entity.ToTable("DocumentProfileDocuments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.DocumentType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(100);

            entity.Property(e => e.IssuingAuthority)
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            entity.HasOne(e => e.Profile)
                .WithMany(p => p.Documents)
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.FileMetadata)
                .WithMany()
                .HasForeignKey(e => e.FileMetadataId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ProfileId);
            entity.HasIndex(e => e.FileMetadataId);
            entity.HasIndex(e => e.DocumentType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ExpiryDate);
        });

        // DocumentMetadataValue configuration
        modelBuilder.Entity<DocumentMetadataValue>(entity =>
        {
            entity.ToTable("DocumentMetadataValues");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StringValue)
                .HasMaxLength(4000);

            entity.Property(e => e.NumberValue)
                .HasPrecision(18, 6);

            entity.Property(e => e.JsonValue)
                .HasMaxLength(8000);

            entity.HasOne(e => e.Document)
                .WithMany(d => d.MetadataValues)
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.MetadataField)
                .WithMany(f => f.MetadataValues)
                .HasForeignKey(e => e.MetadataFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.MetadataFieldId);
            entity.HasIndex(e => new { e.DocumentId, e.MetadataFieldId }).IsUnique()
                .HasFilter("[IsDeleted] = 0");
        });
    }
}
