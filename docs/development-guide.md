# Development Guide

## Prerequisites

- .NET 9.0 SDK
- Docker Desktop (optional)
- Visual Studio 2022 / VS Code
- Git

## Getting Started

### 1. Clone Repository

```bash
git clone <repository-url>
cd AXDD
```

### 2. Build Solution

```bash
cd src
dotnet restore
dotnet build
```

### 3. Run Services Locally

#### Option A: Run Individual Services

```bash
# Terminal 1 - Auth Service
cd src/Services/Auth/AXDD.Services.Auth.Api
dotnet run

# Terminal 2 - MasterData Service
cd src/Services/MasterData/AXDD.Services.MasterData.Api
dotnet run

# Terminal 3 - API Gateway
cd src/ApiGateway/AXDD.ApiGateway
dotnet run
```

#### Option B: Docker Compose

```bash
docker-compose up --build
```

### 4. Access Services

- API Gateway: http://localhost:5000
- Auth Service Swagger: http://localhost:5001/swagger
- MasterData Service Swagger: http://localhost:5002/swagger
- Enterprise Service Swagger: http://localhost:5003/swagger
- Investment Service Swagger: http://localhost:5004/swagger
- FileManager Service Swagger: http://localhost:5005/swagger
- Report Service Swagger: http://localhost:5006/swagger

## Project Structure

```
src/
├── AXDD.sln
├── BuildingBlocks/          # Shared libraries
├── ApiGateway/              # YARP Gateway
├── Services/                # Microservices
└── Tests/                   # Test projects
```

## Adding New Service

### 1. Create Service Project

```bash
cd src/Services
mkdir NewService/AXDD.Services.NewService.Api
cd NewService/AXDD.Services.NewService.Api
dotnet new webapi -f net9.0
```

### 2. Add References

```bash
dotnet add reference ../../../BuildingBlocks/AXDD.BuildingBlocks.Common/
dotnet add reference ../../../BuildingBlocks/AXDD.BuildingBlocks.Domain/
```

### 3. Add to Solution

```bash
cd ../../../
dotnet sln add Services/NewService/AXDD.Services.NewService.Api/
```

### 4. Configure Program.cs

```csharp
using AXDD.BuildingBlocks.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NewService API", Version = "v1" });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

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
```

### 5. Update API Gateway

Edit `src/ApiGateway/AXDD.ApiGateway/appsettings.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "newservice-route": {
        "ClusterId": "newservice-cluster",
        "Match": {
          "Path": "/api/newservice/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "newservice-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://newservice-api:8087"
          }
        }
      }
    }
  }
}
```

## Testing

### Run All Tests

```bash
cd src
dotnet test
```

### Run Specific Test Project

```bash
cd src/Tests/AXDD.Tests.Unit
dotnet test
```

### Add New Test

```csharp
using Xunit;

namespace AXDD.Tests.Unit;

public class MyTests
{
    [Fact]
    public void TestMethod()
    {
        // Arrange
        var input = "test";
        
        // Act
        var result = input.ToUpper();
        
        // Assert
        Assert.Equal("TEST", result);
    }
}
```

## Debugging

### Visual Studio

1. Open `AXDD.sln`
2. Set multiple startup projects
3. Select services to debug
4. Press F5

### VS Code

1. Open workspace
2. Use `.vscode/launch.json` configurations
3. Select service to debug
4. Press F5

## Common Commands

```bash
# Build
dotnet build

# Run tests
dotnet test

# Clean
dotnet clean

# Restore packages
dotnet restore

# Run specific service
dotnet run --project src/Services/Auth/AXDD.Services.Auth.Api

# Watch mode (auto-reload)
dotnet watch --project src/Services/Auth/AXDD.Services.Auth.Api
```

## Troubleshooting

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Port Conflicts

Edit `Properties/launchSettings.json` in each service to change ports.

### Docker Issues

```bash
# Stop all containers
docker-compose down

# Remove volumes
docker-compose down -v

# Rebuild images
docker-compose build --no-cache
```

## Best Practices

1. **Always build before committing**
2. **Write tests for new features**
3. **Follow naming conventions**
4. **Use async/await for I/O operations**
5. **Handle exceptions properly**
6. **Log important events**
7. **Update documentation**

## Code Review Checklist

- [ ] Code builds successfully
- [ ] Tests pass
- [ ] No compiler warnings
- [ ] Follows naming conventions
- [ ] Proper exception handling
- [ ] Logging added where needed
- [ ] Documentation updated
- [ ] No sensitive data in code
