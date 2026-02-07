using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Enterprise.Api.Infrastructure.Data;

/// <summary>
/// Database context for the Enterprise service
/// </summary>
public class EnterpriseDbContext : BaseDbContext
{
    public EnterpriseDbContext(DbContextOptions<EnterpriseDbContext> options) : base(options)
    {
    }

    public DbSet<EnterpriseEntity> Enterprises { get; set; }
    public DbSet<ContactPerson> ContactPersons { get; set; }
    public DbSet<EnterpriseLicense> EnterpriseLicenses { get; set; }
    public DbSet<EnterpriseHistory> EnterpriseHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EnterpriseDbContext).Assembly);
    }
}
