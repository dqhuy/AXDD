using AXDD.BuildingBlocks.Common.Middleware;
using AXDD.BuildingBlocks.Domain.Repositories;
using AXDD.Services.Report.Api.Application.Services;
using AXDD.Services.Report.Api.Application.Services.Interfaces;
using AXDD.Services.Report.Api.Application.Validators;
using AXDD.Services.Report.Api.Domain.Repositories;
using AXDD.Services.Report.Api.Infrastructure.Data;
using AXDD.Services.Report.Api.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "AXDD Report Service", 
        Version = "v1",
        Description = "API for managing enterprise reports and report templates"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database
builder.Services.AddDbContext<ReportDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ReportDatabase")
        ?? "Server=localhost;Database=AXDD_Report;Trusted_Connection=true;TrustServerCertificate=true;";
    options.UseSqlServer(connectionString);
});

// Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, ReportUnitOfWork>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportTemplateRepository, ReportTemplateRepository>();

// Services
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportTemplateService, ReportTemplateService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateReportRequestValidator>();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ReportDbContext>();

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
