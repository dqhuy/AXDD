using AXDD.BuildingBlocks.Common.Middleware;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.Services.Implementations;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using AXDD.Services.FileManager.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("MinIO"));
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUpload"));
builder.Services.Configure<StorageQuotaSettings>(builder.Configuration.GetSection("StorageQuota"));

// Add DbContext
builder.Services.AddDbContext<FileManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add IHttpContextAccessor for audit support
builder.Services.AddHttpContextAccessor();

// Add MinIO client
var minioSettings = builder.Configuration.GetSection("MinIO").Get<MinioSettings>();
if (minioSettings == null)
{
    throw new InvalidOperationException("MinIO settings are not configured");
}

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MinioSettings>>().Value;
    
    var minioClient = new MinioClient()
        .WithEndpoint(settings.Endpoint)
        .WithCredentials(settings.AccessKey, settings.SecretKey);

    if (settings.UseSSL)
    {
        minioClient = minioClient.WithSSL();
    }

    return minioClient.Build();
});

// Register services
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IFileVersionService, FileVersionService>();
builder.Services.AddScoped<IFileShareService, FileShareService>();
builder.Services.AddScoped<IStorageQuotaService, StorageQuotaService>();

// Register Document Profile Management services
builder.Services.AddScoped<IDocumentProfileService, DocumentProfileService>();
builder.Services.AddScoped<IProfileMetadataFieldService, ProfileMetadataFieldService>();
builder.Services.AddScoped<IDocumentProfileDocumentService, DocumentProfileDocumentService>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with file upload support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AXDD FileManager Service",
        Version = "v1",
        Description = "File management service with MinIO storage and SQL Server metadata"
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
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sqlserver",
        tags: new[] { "db", "sql", "sqlserver" });

var app = builder.Build();

// Initialize MinIO buckets
using (var scope = app.Services.CreateScope())
{
    var minioService = scope.ServiceProvider.GetRequiredService<IMinioService>();
    var settings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MinioSettings>>().Value;
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Initializing MinIO buckets...");
        
        await minioService.EnsureBucketExistsAsync(settings.BucketNames.Documents);
        await minioService.EnsureBucketExistsAsync(settings.BucketNames.Attachments);
        await minioService.EnsureBucketExistsAsync(settings.BucketNames.Temp);
        await minioService.EnsureBucketExistsAsync(settings.BucketNames.Archives);

        logger.LogInformation("MinIO buckets initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize MinIO buckets");
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
