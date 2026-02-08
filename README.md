# AXDD - Quản lý CSDL KCN Đồng Nai

Hệ thống quản lý Cơ sở dữ liệu Khu Công Nghiệp Đồng Nai được xây dựng bằng .NET 9 với kiến trúc Microservices.

## 🚀 Quick Start với Docker

### Yêu cầu
- Docker Engine 20.10+
- Docker Compose 2.0+
- RAM tối thiểu 8GB (khuyến nghị 16GB)
- Dung lượng ổ đĩa tối thiểu 50GB

### Khởi động hệ thống

**Linux/Mac:**
```bash
./docker-start.sh up
```

**Windows:**
```cmd
docker-start.bat up
```

**Hoặc sử dụng Docker Compose trực tiếp:**
```bash
docker compose up -d
```

### Kiểm tra cấu hình

Sau khi khởi động, chạy script validation để kiểm tra:

**Linux/Mac:**
```bash
./docker-validate.sh
```

**Windows:**
```cmd
docker-validate.bat
```

### Truy cập ứng dụng
- 🌐 **Cổng thông tin Admin**: http://localhost:8080
- 🌐 **Cổng thông tin Doanh nghiệp**: http://localhost:4200
- 🔌 **API Gateway**: http://localhost:5000
- 📊 **RabbitMQ Console**: http://localhost:15672 (admin/admin)
- 💾 **MinIO Console**: http://localhost:9001 (minioadmin/minioadmin)

### Tài liệu Docker
- 📖 [Hướng dẫn triển khai Docker](DOCKER_DEPLOYMENT.md)
- 🔧 [Hướng dẫn xử lý sự cố](DOCKER_TROUBLESHOOTING.md)
- ✅ [Báo cáo sửa lỗi cấu hình](DOCKER_CONFIGURATION_FIXES.md)
- 📋 [Báo cáo hoàn thành Docker](DOCKER_IMPLEMENTATION_COMPLETE.md)

## Cấu trúc Solution

```
src/
├── AXDD.slnx
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
│   ├── Report/AXDD.Services.Report.Api/
│   ├── Notification/AXDD.Services.Notification.Api/
│   ├── Logging/AXDD.Services.Logging.Api/
│   ├── Search/AXDD.Services.Search.Api/
│   └── GIS/AXDD.Services.GIS.Api/
├── WebApps/
│   ├── AXDD.WebApp.Admin/           # MVC Admin Portal
│   └── EnterprisePortal/             # Angular Enterprise Portal
└── Tests/
    └── AXDD.Tests.Unit/
```

## Công nghệ

### Backend
- .NET 9.0
- ASP.NET Core Web API
- YARP API Gateway
- Entity Framework Core 9.0
- Swagger/OpenAPI
- SignalR

### Frontend
- ASP.NET Core MVC (Admin Portal)
- Angular 17 (Enterprise Portal)
- Bootstrap 5

### Infrastructure
- Docker & Docker Compose
- SQL Server 2022
- PostgreSQL 16 + PostGIS 3.4
- Redis 7
- RabbitMQ 3.12
- MinIO (S3-compatible)
- Elasticsearch 8.11

### Testing
- xUnit

## Build và Run (Development)

### Build toàn bộ solution
```bash
cd src
dotnet restore
dotnet build
```

### Chạy từng service
```bash
# Auth Service
cd src/Services/Auth/AXDD.Services.Auth.Api
dotnet run

# API Gateway
cd src/ApiGateway/AXDD.ApiGateway
dotnet run

# Admin Web App
cd src/WebApps/AXDD.WebApp.Admin
dotnet run
```

### Run tests
```bash
dotnet test
```

## API Ports

### API Gateway & Services
- **API Gateway**: 5000
- **Auth**: 5001
- **MasterData**: 5002
- **Enterprise**: 5003
- **Investment**: 5004
- **FileManager**: 5005
- **Report**: 5006
- **Notification**: 5007
- **Logging**: 5008
- **Search**: 5009
- **GIS**: 5010

### Web Applications
- **Admin Portal (MVC)**: 8080
- **Enterprise Portal (Angular)**: 4200

### Infrastructure Services
- **SQL Server**: 1433
- **PostgreSQL + PostGIS**: 5432
- **Redis**: 6379
- **RabbitMQ**: 5672 (AMQP), 15672 (Management)
- **MinIO**: 9000 (API), 9001 (Console)
- **Elasticsearch**: 9200, 9300

## Kiến trúc

```
┌─────────────────────────────────────────────────────────┐
│                     Client Layer                         │
│   Admin Portal (MVC) | Enterprise Portal (Angular)      │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                  API Gateway (YARP)                      │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                Microservices Layer                       │
│  10 Microservices (Auth, MasterData, Enterprise, ...)   │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│             Infrastructure Layer                         │
│  SQL Server | PostgreSQL | Redis | RabbitMQ | MinIO | ES│
└─────────────────────────────────────────────────────────┘
```

## Tài liệu

- 📁 [Kiến trúc hệ thống](docs/architecture/)
- 📁 [Đặc tả kỹ thuật](docs/technical-spec/)
- 📁 [Yêu cầu nghiệp vụ](docs/requirement/)
- 📖 [Hướng dẫn phát triển](docs/development-guide.md)

## License

[Your License Here]
