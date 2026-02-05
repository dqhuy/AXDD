# AXDD - Quản lý CSDL KCN Đồng Nai

Hệ thống quản lý Cơ sở dữ liệu Khu Công Nghiệp Đồng Nai được xây dựng bằng .NET 9 với kiến trúc Microservices.

## Cấu trúc Solution

```
src/
├── AXDD.sln
├── BuildingBlocks/
│   ├── AXDD.BuildingBlocks.Common/
│   ├── AXDD.BuildingBlocks.Domain/
│   └── AXDD.BuildingBlocks.Infrastructure/
├── ApiGateway/
│   └── AXDD.ApiGateway/
├── Services/
│   ├── Auth/AXDD.Services.Auth.Api/
│   ├── MasterData/AXDD.Services.MasterData.Api/
│   ├── Enterprise/AXDD.Services.Enterprise.Api/
│   ├── Investment/AXDD.Services.Investment.Api/
│   ├── FileManager/AXDD.Services.FileManager.Api/
│   └── Report/AXDD.Services.Report.Api/
└── Tests/
    └── AXDD.Tests.Unit/
```

## Công nghệ

- .NET 9.0
- ASP.NET Core Web API
- YARP API Gateway
- Entity Framework Core 9.0
- Swagger/OpenAPI
- Docker & Docker Compose
- xUnit

## Build và Run

```bash
# Build
cd src
dotnet build

# Run with Docker
docker-compose up --build
```

## API Ports

- API Gateway: 5000
- Auth: 5001
- MasterData: 5002
- Enterprise: 5003
- Investment: 5004
- FileManager: 5005
- Report: 5006
