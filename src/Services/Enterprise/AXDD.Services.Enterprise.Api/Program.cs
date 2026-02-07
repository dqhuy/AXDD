using AXDD.BuildingBlocks.Common.Middleware;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Application.Services;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using AXDD.Services.Enterprise.Api.Application.Validators;
using AXDD.Services.Enterprise.Api.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Infrastructure.Data;
using AXDD.Services.Enterprise.Api.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "AXDD Enterprise Service", 
        Version = "v1",
        Description = "API for managing 2,100+ enterprises in industrial zones"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database
builder.Services.AddDbContext<EnterpriseDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("EnterpriseDatabase")
        ?? "Server=localhost;Database=AXDD_Enterprise;Trusted_Connection=true;TrustServerCertificate=true;";
    options.UseSqlServer(connectionString);
});

// Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, EnterpriseUnitOfWork>();
builder.Services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();

// Services
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IContactPersonService, ContactPersonService>();
builder.Services.AddScoped<IEnterpriseLicenseService, EnterpriseLicenseService>();
builder.Services.AddScoped<IEnterpriseHistoryService, EnterpriseHistoryService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEnterpriseRequestValidator>();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EnterpriseDbContext>();

// CORS (if needed)
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
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
