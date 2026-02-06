using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NetTopologySuite;

namespace AXDD.Services.GIS.Api.Data;

/// <summary>
/// Design-time factory for GisDbContext
/// Used by EF Core tools for migrations
/// </summary>
public class GisDbContextFactory : IDesignTimeDbContextFactory<GisDbContext>
{
    public GisDbContext CreateDbContext(string[] args)
    {
        // Configure NetTopologySuite
        NtsGeometryServices.Instance = new NtsGeometryServices(
            NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
            new NetTopologySuite.Geometries.PrecisionModel(NetTopologySuite.Geometries.PrecisionModels.Floating),
            4326
        );

        var optionsBuilder = new DbContextOptionsBuilder<GisDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=AXDD_GIS;Username=postgres;Password=postgres123",
            o => o.UseNetTopologySuite()
        );

        return new GisDbContext(optionsBuilder.Options);
    }
}
