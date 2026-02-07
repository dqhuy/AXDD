using AXDD.BuildingBlocks.Common.Middleware;
using AXDD.Services.GIS.Api.Data;
using AXDD.Services.GIS.Api.Services.Implementations;
using AXDD.Services.GIS.Api.Services.Interfaces;
using AXDD.Services.GIS.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetTopologySuite;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Services.Configure<GisSettings>(builder.Configuration.GetSection("GisSettings"));
builder.Services.Configure<MapSettings>(builder.Configuration.GetSection("MapSettings"));

// Configure NetTopologySuite for spatial data
NtsGeometryServices.Instance = new NtsGeometryServices(
    NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
    new NetTopologySuite.Geometries.PrecisionModel(NetTopologySuite.Geometries.PrecisionModels.Floating),
    4326 // WGS84 SRID
);

// Add DbContext with PostGIS support
builder.Services.AddDbContext<GisDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("GisDatabase");
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
});

// Add IHttpContextAccessor for audit support
builder.Services.AddHttpContextAccessor();

// Register services
builder.Services.AddScoped<IGisService, GisService>();
builder.Services.AddScoped<IIndustrialZoneService, IndustrialZoneService>();
builder.Services.AddScoped<ISpatialQueryService, SpatialQueryService>();
builder.Services.AddScoped<IMapService, MapService>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AXDD GIS Service",
        Version = "v1",
        Description = "Geographic Information System service with PostGIS support for enterprise location management and industrial zone mapping"
    });

    // Enable XML documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("GisDatabase")!,
        name: "postgresql",
        tags: new[] { "db", "postgresql", "postgis" });

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GisDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");

        // Check if PostGIS extension is enabled
        logger.LogInformation("Checking PostGIS extension...");
        var hasPostGIS = await context.Database
            .SqlQueryRaw<bool>("SELECT EXISTS(SELECT 1 FROM pg_extension WHERE extname = 'postgis')")
            .FirstOrDefaultAsync();

        if (!hasPostGIS)
        {
            logger.LogWarning("PostGIS extension not found. Attempting to create...");
            await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS postgis;");
            logger.LogInformation("PostGIS extension created successfully");
        }
        else
        {
            logger.LogInformation("PostGIS extension is available");
        }

        // Seed initial data
        logger.LogInformation("Seeding initial data...");
        await GisDbSeeder.SeedDataAsync(context);
        logger.LogInformation("Data seeding completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
