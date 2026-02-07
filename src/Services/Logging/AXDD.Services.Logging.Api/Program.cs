using AXDD.BuildingBlocks.Common.Middleware;
using AXDD.Services.Logging.Api.Application.Services;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using AXDD.Services.Logging.Api.Infrastructure.HostedServices;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/logging-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "AXDD Logging Service",
        Version = "v1",
        Description = "Comprehensive logging service with CRUD operations for audit trails, user activities, error tracking, and performance monitoring"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database
builder.Services.AddDbContext<LogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("LogDatabase")
        ?? "Server=localhost;Database=AXDD_Logging;Trusted_Connection=true;TrustServerCertificate=true;";
    options.UseSqlServer(connectionString);
});

// Services
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IUserActivityService, UserActivityService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IPerformanceLogService, PerformanceLogService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Hosted Services
builder.Services.AddHostedService<LogCleanupHostedService>();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LogDbContext>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AXDD Logging Service v1");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LogDbContext>();
        await context.Database.MigrateAsync();
        await LogDbSeeder.SeedAsync(context);
        Log.Information("Database migration and seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating or seeding the database");
    }
}

Log.Information("Starting AXDD Logging Service");

app.Run();
