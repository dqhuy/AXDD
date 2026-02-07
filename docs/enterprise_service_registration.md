# Service Registration Guide

## Registering Enterprise Services in Dependency Injection

Add these service registrations to `Program.cs` in the Enterprise API project:

```csharp
// Enterprise Services
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IEnterpriseHistoryService, EnterpriseHistoryService>();
builder.Services.AddScoped<IContactPersonService, ContactPersonService>();
builder.Services.AddScoped<IEnterpriseLicenseService, EnterpriseLicenseService>();
```

## Full Program.cs Example

```csharp
using AXDD.Services.Enterprise.Api.Application.Services;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<EnterpriseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();

// Enterprise Services
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IEnterpriseHistoryService, EnterpriseHistoryService>();
builder.Services.AddScoped<IContactPersonService, ContactPersonService>();
builder.Services.AddScoped<IEnterpriseLicenseService, EnterpriseLicenseService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

## Service Lifetimes

All services are registered with **Scoped** lifetime:
- ✅ Safe for web applications
- ✅ One instance per HTTP request
- ✅ Properly disposed at end of request
- ✅ Works with DbContext (also scoped)
- ✅ Works with IUnitOfWork pattern

## Service Dependencies

Each service requires:

### IEnterpriseService
- `IEnterpriseRepository`
- `IUnitOfWork`
- `IEnterpriseHistoryService`

### IEnterpriseHistoryService
- `IUnitOfWork`

### IContactPersonService
- `IUnitOfWork`
- `IEnterpriseHistoryService`

### IEnterpriseLicenseService
- `IUnitOfWork`
- `IEnterpriseHistoryService`

## Usage in Controllers

```csharp
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Enterprise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnterprisesController : ControllerBase
{
    private readonly IEnterpriseService _enterpriseService;

    public EnterprisesController(IEnterpriseService enterpriseService)
    {
        _enterpriseService = enterpriseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await _enterpriseService.GetAllAsync(
            pageNumber,
            pageSize,
            searchTerm,
            null, // status
            null, // zoneId
            null, // industryCode
            null, // sortBy
            false, // descending
            ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _enterpriseService.GetByIdAsync(id, ct);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEnterpriseRequest request,
        CancellationToken ct)
    {
        var userId = User.Identity?.Name ?? "system"; // Get from claims

        var result = await _enterpriseService.CreateAsync(request, userId, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }
}
```

## Testing Services

```csharp
using AXDD.Services.Enterprise.Api.Application.Services;
using AXDD.Services.Enterprise.Api.Domain.Repositories;
using Moq;
using Xunit;

public class EnterpriseServiceTests
{
    private readonly Mock<IEnterpriseRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEnterpriseHistoryService> _mockHistory;
    private readonly EnterpriseService _service;

    public EnterpriseServiceTests()
    {
        _mockRepo = new Mock<IEnterpriseRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockHistory = new Mock<IEnterpriseHistoryService>();
        
        _service = new EnterpriseService(
            _mockRepo.Object,
            _mockUnitOfWork.Object,
            _mockHistory.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEnterpriseExists_ReturnsSuccess()
    {
        // Arrange
        var enterpriseId = Guid.NewGuid();
        var enterprise = new EnterpriseEntity { Id = enterpriseId, Name = "Test" };
        _mockRepo.Setup(x => x.GetByIdAsync(enterpriseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enterprise);

        // Act
        var result = await _service.GetByIdAsync(enterpriseId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(enterpriseId, result.Value.Id);
    }
}
```
