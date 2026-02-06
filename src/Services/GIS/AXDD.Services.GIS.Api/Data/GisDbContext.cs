using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.Services.GIS.Api.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace AXDD.Services.GIS.Api.Data;

/// <summary>
/// Database context for GIS service with PostGIS support
/// </summary>
public class GisDbContext : BaseDbContext
{
    public GisDbContext(DbContextOptions<GisDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options, httpContextAccessor?.HttpContext?.User?.Identity?.Name)
    {
    }

    /// <summary>
    /// Enterprise locations
    /// </summary>
    public DbSet<EnterpriseLocation> EnterpriseLocations => Set<EnterpriseLocation>();

    /// <summary>
    /// Industrial zones
    /// </summary>
    public DbSet<IndustrialZone> IndustrialZones => Set<IndustrialZone>();

    /// <summary>
    /// Land plots
    /// </summary>
    public DbSet<LandPlot> LandPlots => Set<LandPlot>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure EnterpriseLocation
        modelBuilder.Entity<EnterpriseLocation>(entity =>
        {
            entity.ToTable("EnterpriseLocations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EnterpriseCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.EnterpriseName)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Location)
                .IsRequired()
                .HasColumnType("geometry(Point, 4326)");

            entity.Property(e => e.Latitude)
                .IsRequired();

            entity.Property(e => e.Longitude)
                .IsRequired();

            entity.Property(e => e.Address)
                .HasMaxLength(1000);

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            // Spatial index on Location
            entity.HasIndex(e => e.Location)
                .HasMethod("gist");

            // Index on enterprise code for quick lookup
            entity.HasIndex(e => e.EnterpriseCode);

            // Index on enterprise ID
            entity.HasIndex(e => e.EnterpriseId);

            // Index on industrial zone
            entity.HasIndex(e => e.IndustrialZoneId);

            // Relationship with IndustrialZone
            entity.HasOne(e => e.IndustrialZone)
                .WithMany(z => z.EnterpriseLocations)
                .HasForeignKey(e => e.IndustrialZoneId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure IndustrialZone
        modelBuilder.Entity<IndustrialZone>(entity =>
        {
            entity.ToTable("IndustrialZones");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Boundary)
                .IsRequired()
                .HasColumnType("geometry(Polygon, 4326)");

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Province)
                .HasMaxLength(100);

            entity.Property(e => e.District)
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            // Spatial index on Boundary
            entity.HasIndex(e => e.Boundary)
                .HasMethod("gist");

            // Unique index on code
            entity.HasIndex(e => e.Code)
                .IsUnique();

            // Index on name
            entity.HasIndex(e => e.Name);

            // Index on province
            entity.HasIndex(e => e.Province);
        });

        // Configure LandPlot
        modelBuilder.Entity<LandPlot>(entity =>
        {
            entity.ToTable("LandPlots");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PlotNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Geometry)
                .IsRequired()
                .HasColumnType("geometry(Polygon, 4326)");

            entity.Property(e => e.OwnerEnterpriseCode)
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            // Spatial index on Geometry
            entity.HasIndex(e => e.Geometry)
                .HasMethod("gist");

            // Index on plot number within zone
            entity.HasIndex(e => new { e.IndustrialZoneId, e.PlotNumber })
                .IsUnique();

            // Index on owner
            entity.HasIndex(e => e.OwnerEnterpriseId);

            // Relationship with IndustrialZone
            entity.HasOne(e => e.IndustrialZone)
                .WithMany(z => z.LandPlots)
                .HasForeignKey(e => e.IndustrialZoneId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
