using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.MasterData.Api.Data;

/// <summary>
/// Database context for MasterData service
/// </summary>
public class MasterDataDbContext : BaseDbContext
{
    public MasterDataDbContext(DbContextOptions<MasterDataDbContext> options) : base(options)
    {
    }

    public MasterDataDbContext(DbContextOptions<MasterDataDbContext> options, string? currentUser) 
        : base(options, currentUser)
    {
    }

    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<IndustrialZone> IndustrialZones => Set<IndustrialZone>();
    public DbSet<IndustryCode> IndustryCodes => Set<IndustryCode>();
    public DbSet<CertificateType> CertificateTypes => Set<CertificateType>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<StatusCode> StatusCodes => Set<StatusCode>();
    public DbSet<Configuration> Configurations => Set<Configuration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDataDbContext).Assembly);

        // Seed data
        MasterDataSeeder.SeedData(modelBuilder);
    }
}
