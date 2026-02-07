using AXDD.BuildingBlocks.Infrastructure.Persistence;
using AXDD.Services.Report.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Report.Api.Infrastructure.Data;

/// <summary>
/// Database context for Report Service
/// </summary>
public class ReportDbContext : BaseDbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public DbSet<EnterpriseReport> EnterpriseReports => Set<EnterpriseReport>();
    public DbSet<ReportTemplate> ReportTemplates => Set<ReportTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
    }
}
