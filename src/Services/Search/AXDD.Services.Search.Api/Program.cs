using System.Reflection;
using System.Text;
using AXDD.BuildingBlocks.Common.Middleware;
using AXDD.Services.Search.Api.Services.Implementations;
using AXDD.Services.Search.Api.Services.Interfaces;
using AXDD.Services.Search.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Services.Configure<ElasticsearchSettings>(
    builder.Configuration.GetSection("Elasticsearch"));
builder.Services.Configure<SearchSettings>(
    builder.Configuration.GetSection("Search"));

// Add services
builder.Services.AddSingleton<IElasticsearchClientFactory, ElasticsearchClientFactory>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IIndexingService, IndexingService>();
builder.Services.AddScoped<IIndexManagementService, IndexManagementService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<ElasticsearchHealthCheck>("elasticsearch");

// Add controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add JWT authentication (optional, can be enabled later)
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (!string.IsNullOrWhiteSpace(jwtSecret))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });
}

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AXDD Search Service API",
        Version = "v1",
        Description = "Full-text search service for enterprises, documents, and projects using Elasticsearch",
        Contact = new OpenApiContact
        {
            Name = "AXDD Development Team",
            Email = "dev@axdd.com"
        }
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AXDD Search Service API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Add global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

// Add authentication if configured
if (!string.IsNullOrWhiteSpace(jwtSecret))
{
    app.UseAuthentication();
}

app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Initialize indexes on startup (optional, can be commented out)
try
{
    using var scope = app.Services.CreateScope();
    var indexManagementService = scope.ServiceProvider.GetRequiredService<IIndexManagementService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Checking Elasticsearch connection and initializing indexes...");
    
    var clientFactory = scope.ServiceProvider.GetRequiredService<IElasticsearchClientFactory>();
    var isHealthy = await clientFactory.IsHealthyAsync();
    
    if (isHealthy)
    {
        await indexManagementService.InitializeIndexesAsync();
        logger.LogInformation("Indexes initialized successfully");
    }
    else
    {
        logger.LogWarning("Elasticsearch is not available. Indexes will not be initialized. The service will continue to run.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to initialize indexes on startup. This is not critical if Elasticsearch is not yet available.");
}

app.Run();
