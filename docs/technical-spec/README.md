# Tài Liệu Đặc Tả Kỹ Thuật
# Hệ Thống AXDD - Quản Lý CSDL KCN Đồng Nai

**Phiên bản:** 2.0  
**Ngày cập nhật:** 05/02/2025  
**Tác giả:** Solution Architect Team

---

## Giới Thiệu

Thư mục này chứa tài liệu đặc tả kỹ thuật chi tiết cho hệ thống AXDD - Quản lý Cơ sở dữ liệu Ban Quản lý các Khu Công nghiệp tỉnh Đồng Nai.

## Cấu Trúc Tài Liệu

### 1. [Technical Specification](./technical_specification.md)
**Mô tả:** Đặc tả kỹ thuật tổng quan về hệ thống

**Nội dung:**
- Tổng quan kiến trúc hệ thống (Microservices Architecture)
- Chi tiết từng microservice (Auth, File, OCR, GIS, Investment, Enterprise, etc.)
- API Design Patterns và Conventions
- Security & Authentication
- Performance Considerations
- Deployment Strategy
- Monitoring & Logging
- Development Guidelines

**Công nghệ chính:**
- .NET 9
- SQL Server 2022
- PostgreSQL 16 + PostGIS 3.4
- MinIO
- Elasticsearch 8.x
- Redis 7.x
- RabbitMQ 3.12.x
- Docker

**Đọc file này nếu bạn muốn:**
- Hiểu tổng quan kiến trúc hệ thống
- Nắm rõ các microservices và chức năng của chúng
- Hiểu về security layers và authentication flow
- Biết về performance requirements và caching strategy

---

### 2. [API Specification](./api_specification.md)
**Mô tả:** Đặc tả chi tiết về REST API

**Nội dung:**
- REST API Conventions (HTTP methods, URL structure, naming)
- Authentication & Authorization (JWT, OAuth 2.0, VNeID SSO)
- Request/Response Format (headers, body, envelope)
- Error Handling (error codes, validation errors)
- Pagination (page-based, cursor-based)
- Filtering & Sorting
- Rate Limiting
- API Endpoints (Auth, Enterprise, Investment, File, Search, GIS, Report)
- Webhooks
- API Versioning

**Đọc file này nếu bạn:**
- Là frontend developer cần tích hợp với API
- Là backend developer cần implement API endpoints
- Muốn hiểu về authentication flow
- Cần biết về error handling và response format

**Ví dụ API Endpoints:**

```http
# Authentication
POST   /api/v1/auth/login
POST   /api/v1/auth/refresh-token
GET    /api/v1/auth/user-info

# Enterprise Management
GET    /api/v1/enterprises
POST   /api/v1/enterprises
GET    /api/v1/enterprises/{id}
PUT    /api/v1/enterprises/{id}
DELETE /api/v1/enterprises/{id}

# Investment Certificates
GET    /api/v1/investment/certificates
POST   /api/v1/investment/certificates
POST   /api/v1/investment/certificates/{id}/adjust

# File Management
POST   /api/v1/files/upload
GET    /api/v1/files/{id}/download

# Search
POST   /api/v1/search/enterprises
GET    /api/v1/search/suggestions
```

---

### 3. [Database Design](./database_design.md)
**Mô tả:** Thiết kế chi tiết database schema

**Nội dung:**
- Tổng quan Database Architecture
- Entity Relationship Diagrams (text-based ERD)
- Table Schemas (AuthDB, MasterDataDB, EnterpriseDB, InvestmentDB, FileMetaDB, GIS_DB, etc.)
- Index Strategy (single-column, composite, filtered, full-text)
- Migration Strategy (EF Core Migrations, zero-downtime migrations)
- Data Seeding (master data, default roles/permissions)
- Backup & Recovery Strategy

**Databases:**

| Database | Technology | Purpose |
|----------|-----------|---------|
| `AuthDB` | SQL Server | Authentication & Authorization |
| `MasterDataDB` | SQL Server | Master Data (KCN, Industries, etc.) |
| `EnterpriseDB` | SQL Server | Enterprise Management |
| `InvestmentDB` | SQL Server | Investment Management |
| `EnvironmentDB` | SQL Server | Environment Management |
| `ConstructionDB` | SQL Server | Construction Management |
| `LaborDB` | SQL Server | Labor Management |
| `FileMetaDB` | SQL Server | File Metadata |
| `GIS_DB` | PostgreSQL + PostGIS | Geographic Information |

**Đọc file này nếu bạn:**
- Là database developer/DBA
- Cần hiểu về database schema
- Muốn biết về indexing strategy
- Cần implement migrations
- Quan tâm về backup/recovery

---

## Kiến Trúc Hệ Thống Tổng Quan

```
┌─────────────────────────────────────────────────────────┐
│                  PRESENTATION LAYER                      │
│   ┌──────────────┐  ┌──────────────┐  ┌─────────────┐  │
│   │  Staff Web   │  │ Enterprise   │  │  Dashboard  │  │
│   │ (Next.js 14) │  │  Web App     │  │  Web App    │  │
│   └──────────────┘  └──────────────┘  └─────────────┘  │
└──────────────────────┬──────────────────────────────────┘
                       │ HTTPS/REST
                       ▼
┌─────────────────────────────────────────────────────────┐
│                  API GATEWAY LAYER                       │
│              (YARP/Ocelot - .NET 9)                     │
│   • Routing  • Auth  • Rate Limiting  • Load Balance   │
└──────────────────────┬──────────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
        ▼              ▼              ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│   CORE      │ │  BUSINESS   │ │INTEGRATION  │
│  SERVICES   │ │  SERVICES   │ │  SERVICES   │
├─────────────┤ ├─────────────┤ ├─────────────┤
│• Auth       │ │• Investment │ │• LGSP       │
│• MasterData │ │• Environment│ │• OCR        │
│• File       │ │• Construction│ │• VNeID     │
│• Notify     │ │• Labor      │ │             │
│• Search     │ │• Enterprise │ │             │
│• GIS        │ │• Inspection │ │             │
│             │ │• Report     │ │             │
└─────────────┘ └─────────────┘ └─────────────┘
        │              │              │
        └──────────────┼──────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────┐
│                    DATA LAYER                            │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────────┐ │
│  │   SQL    │ │PostgreSQL│ │  MinIO   │ │Elasticsearch│
│  │  Server  │ │+ PostGIS │ │ (S3)     │ │  (Search) │ │
│  └──────────┘ └──────────┘ └──────────┘ └───────────┘ │
│  ┌──────────┐ ┌──────────┐                             │
│  │  Redis   │ │ RabbitMQ │                             │
│  │ (Cache)  │ │ (Queue)  │                             │
│  └──────────┘ └──────────┘                             │
└─────────────────────────────────────────────────────────┘
```

## Yêu Cầu Hệ Thống

### Yêu Cầu Chức Năng
- Quản lý 2.100+ doanh nghiệp trong các KCN
- Số hóa ~1.4 triệu trang tài liệu
- Quản lý đầu tư, môi trường, xây dựng, lao động
- Tích hợp với LGSP, Một cửa, QLVB, VNeID

### Yêu Cầu Phi Chức Năng

| Yêu Cầu | Chỉ Tiêu |
|---------|----------|
| **Hiệu năng** | ≥ 500 CCU (Concurrent Users) |
| **Response Time** | < 10 giây/request |
| **Database Query** | < 100ms |
| **Search Latency** | < 1 second |
| **Uptime** | 99.5% availability |
| **Bảo mật** | Cấp độ 3 theo NĐ 85/2016 |

## Nguyên Tắc Thiết Kế

### SOLID Principles
- **S**ingle Responsibility
- **O**pen/Closed
- **L**iskov Substitution
- **I**nterface Segregation
- **D**ependency Inversion

### Architecture Patterns
- **Microservices Architecture**
- **Domain-Driven Design (DDD)**
- **Clean Architecture**
- **CQRS Pattern**
- **Event-Driven Architecture**

### Design Patterns
- Repository Pattern
- Unit of Work Pattern
- Factory Pattern
- Strategy Pattern
- Observer Pattern (Domain Events)

## Stack Công Nghệ

### Backend
- **.NET 9** - Primary framework
- **Entity Framework Core** - ORM (Code First)
- **MediatR** - CQRS implementation
- **FluentValidation** - Validation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging

### Frontend
- **Next.js 14** - React framework
- **TypeScript** - Type safety
- **TailwindCSS** - Styling
- **React Query** - Data fetching
- **Zustand** - State management

### Database
- **SQL Server 2022** - Primary RDBMS
- **PostgreSQL 16 + PostGIS 3.4** - Spatial database
- **Elasticsearch 8.x** - Search engine
- **Redis 7.x** - Caching
- **MinIO** - Object storage

### Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Orchestration (dev/staging)
- **YARP/Ocelot** - API Gateway
- **RabbitMQ** - Message queue

## Cách Sử Dụng Tài Liệu

### Cho Product Owner / Business Analyst
1. Đọc phần "Tổng quan" trong **technical_specification.md**
2. Xem phần "Chức năng chính" của các microservices
3. Tham khảo API endpoints trong **api_specification.md**

### Cho Solution Architect / Tech Lead
1. Đọc toàn bộ **technical_specification.md**
2. Xem chi tiết database design trong **database_design.md**
3. Review security architecture và performance considerations

### Cho Backend Developer
1. Đọc phần microservice của mình trong **technical_specification.md**
2. Implement API theo **api_specification.md**
3. Thiết kế database theo **database_design.md**
4. Follow development guidelines

### Cho Frontend Developer
1. Đọc **api_specification.md** để hiểu API
2. Xem authentication flow
3. Implement API integration với đúng request/response format

### Cho Database Developer / DBA
1. Đọc **database_design.md**
2. Implement table schemas và indexes
3. Setup migrations và seeding
4. Configure backup/recovery

### Cho DevOps Engineer
1. Xem deployment strategy trong **technical_specification.md**
2. Setup Docker containers
3. Configure monitoring và logging
4. Implement backup automation

## Quy Trình Phát Triển

### 1. Setup Development Environment
```bash
# Clone repository
git clone https://github.com/dongnai/axdd.git

# Setup infrastructure
docker-compose up -d

# Run migrations
dotnet ef database update

# Run application
dotnet run --project src/ApiGateway
```

### 2. Development Workflow
```
1. Create feature branch: feature/INV-001-investment-cert
2. Implement feature following guidelines
3. Write unit tests (≥80% coverage)
4. Submit pull request
5. Code review
6. Merge to develop
7. Deploy to staging
8. QA testing
9. Merge to main
10. Deploy to production
```

### 3. Testing Strategy
- **Unit Tests**: Test business logic
- **Integration Tests**: Test API endpoints with database
- **E2E Tests**: Test complete user flows
- **Performance Tests**: Load testing with k6/JMeter

## Liên Hệ & Hỗ Trợ

**Solution Architect Team**
- Email: architecture@dongnai.gov.vn
- Slack: #axdd-architecture

**Technical Support**
- Email: support@dongnai.gov.vn
- Hotline: 0251.xxx.xxxx

---

## Lịch Sử Phiên Bản

### Version 2.0 (05/02/2025)
- Cập nhật kiến trúc microservices chi tiết
- Bổ sung API specification đầy đủ
- Thiết kế database schema hoàn chỉnh
- Thêm deployment strategy

### Version 1.0 (15/01/2025)
- Phiên bản đầu tiên
- Kiến trúc tổng quan
- Yêu cầu chức năng cơ bản

---

## Phụ Lục

### Tài Liệu Liên Quan
- [Yêu cầu kỹ thuật](../requirement/yeu-cau-ky-thuat.md)
- [Kiến trúc hệ thống](../architecture/system-architecture.md)
- [Architecture Design](../architecture/Architecture_Design.md)

### Công Cụ Hỗ Trợ
- [Draw.io](https://draw.io) - Vẽ diagram
- [dbdiagram.io](https://dbdiagram.io) - Thiết kế database
- [Swagger](https://swagger.io) - API documentation
- [Postman](https://postman.com) - API testing

### Tài Nguyên Học Tập
- [.NET Documentation](https://learn.microsoft.com/dotnet/)
- [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [Microservices Patterns](https://microservices.io/patterns/)

---

**Lưu ý:** Tài liệu này là tài liệu sống (living document) và sẽ được cập nhật thường xuyên theo tiến độ dự án.

**Cập nhật lần cuối:** 05/02/2025  
**Người cập nhật:** Solution Architect Team
