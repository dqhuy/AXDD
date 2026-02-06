# AXDD Solution Architecture

## Overview
Hệ thống AXDD (Quản lý CSDL KCN Đồng Nai) được thiết kế theo kiến trúc Microservices với các nguyên tắc:

- **Domain-Driven Design (DDD)**: Tổ chức code theo domain
- **SOLID Principles**: Đảm bảo code dễ maintain và mở rộng
- **Clean Architecture**: Phân tách rõ ràng giữa các layer
- **API Gateway Pattern**: Sử dụng YARP làm entry point

## Services

### 1. Auth Service (Port: 5001)
**Trách nhiệm:**
- Xác thực người dùng (Authentication)
- Phân quyền (Authorization)
- Quản lý token (JWT)
- Quản lý session

**Endpoints:**
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/register` - Đăng ký
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/logout` - Đăng xuất

### 2. MasterData Service (Port: 5002)
**Trách nhiệm:**
- Quản lý dữ liệu danh mục hệ thống
- Tỉnh/Thành phố
- Ngành nghề
- Loại hình doanh nghiệp

**Endpoints:**
- `GET /api/masterdata/provinces` - Danh sách tỉnh/thành
- `GET /api/masterdata/industries` - Danh sách ngành nghề

### 3. Enterprise Service (Port: 5003)
**Trách nhiệm:**
- Quản lý thông tin doanh nghiệp
- CRUD operations cho doanh nghiệp
- Tìm kiếm và filter doanh nghiệp

**Endpoints:**
- `GET /api/enterprise` - Danh sách doanh nghiệp (paging)
- `GET /api/enterprise/{id}` - Chi tiết doanh nghiệp
- `POST /api/enterprise` - Tạo mới
- `PUT /api/enterprise/{id}` - Cập nhật
- `DELETE /api/enterprise/{id}` - Xóa

### 4. Investment Service (Port: 5004)
**Trách nhiệm:**
- Quản lý dự án đầu tư
- Theo dõi tiến độ dự án
- Báo cáo đầu tư

**Endpoints:**
- `GET /api/investment/projects` - Danh sách dự án
- `GET /api/investment/projects/{id}` - Chi tiết dự án

### 5. FileManager Service (Port: 5005)
**Trách nhiệm:**
- Upload/Download files
- Quản lý tài liệu
- Storage management

**Endpoints:**
- `GET /api/filemanager/files` - Danh sách files
- `POST /api/filemanager/upload` - Upload file
- `GET /api/filemanager/download/{id}` - Download file

### 6. Report Service (Port: 5006)
**Trách nhiệm:**
- Background jobs tổng hợp dữ liệu định kỳ
- Cung cấp dữ liệu cho Dashboard
- Export báo cáo (PDF, Excel)
- Kết xuất báo cáo định kỳ

**Lưu ý:** Service này KHÔNG đảm nhiệm việc nộp/phê duyệt báo cáo nghiệp vụ của doanh nghiệp.

**Endpoints:**
- `GET /api/report/summary` - Báo cáo tổng hợp
- `GET /api/report/export` - Export báo cáo
- `GET /api/report/dashboard` - Dữ liệu dashboard

### 7. EnterpriseReportManagement Service (Port: 5007)
**Trách nhiệm:**
- Quản lý nộp báo cáo định kỳ của doanh nghiệp
- Phê duyệt/Từ chối báo cáo của ban quản lý KCN
- Gửi thông báo qua RabbitMQ khi có thay đổi trạng thái
- Theo dõi quy trình duyệt báo cáo

**Message Queue Integration:**
- Publish notification messages qua RabbitMQ
- Publish log events qua RabbitMQ

**Endpoints:**
- `GET /api/enterprise-reports` - Danh sách báo cáo
- `POST /api/enterprise-reports` - Nộp báo cáo mới
- `PUT /api/enterprise-reports/{id}/approve` - Phê duyệt báo cáo
- `PUT /api/enterprise-reports/{id}/reject` - Từ chối báo cáo

## BuildingBlocks

### Common
**Chức năng:**
- DTOs (ApiResponse, PagedResult, BaseDto)
- Extensions (String, DateTime)
- Exceptions (NotFoundException, BadRequestException)
- Middleware (ExceptionHandlingMiddleware)

### Domain
**Chức năng:**
- Base entities (BaseEntity, AuditableEntity)
- Value Objects (Address)
- Domain events
- Aggregates

### Infrastructure
**Chức năng:**
- Repository interfaces
- EF Core configurations
- Database context
- Unit of Work pattern

## Communication Pattern

### API Gateway (YARP)
- Entry point cho tất cả requests
- Load balancing
- Rate limiting (future)
- Authentication/Authorization (future)

### Service-to-Service
- HTTP/REST APIs cho synchronous communication
- Message Queue (RabbitMQ) cho asynchronous communication

### Message Queue Architecture (RabbitMQ)

**Services nhận message qua RabbitMQ:**

1. **Log Service**
   - Nhận log events từ tất cả business services
   - Ghi nhật ký CRUD, Login, Logout
   - Queue: `log.events`

2. **Notification Service**
   - Nhận notification requests
   - Gửi Email, In-App, Push notifications
   - Queue: `notification.requests`

3. **Search Service**
   - Nhận index/update/delete requests
   - Cập nhật Elasticsearch index
   - Queue: `search.index`

4. **OCR Service**
   - Nhận OCR processing requests
   - Xử lý nhận dạng ký tự từ file
   - Queue: `ocr.requests`

**Services gửi message:**
- Tất cả business services (Investment, Enterprise, EnterpriseReportManagement, etc.)
- Gửi log events khi có CRUD operations
- Gửi notification requests khi cần thông báo users
- Gửi search index updates khi data thay đổi

**Lợi ích:**
- Tách biệt business logic khỏi logging/notification
- Xử lý bất đồng bộ, không blocking main flow
- Retry mechanism cho failed messages
- Scalability: có thể scale consumer services độc lập

## Data Management

### Database Strategy
- Database per service (Microservices pattern)
- Mỗi service có DB riêng
- Eventual consistency giữa services

### Suggested Databases
- **SQL Server / PostgreSQL**: Auth, Enterprise, Investment
- **MongoDB**: FileManager (document storage)
- **Redis**: Caching layer

## Security

### Authentication
- JWT tokens
- Token expiration
- Refresh token mechanism

### Authorization
- Role-based access control (RBAC)
- Policy-based authorization
- API Key for service-to-service (future)

## Monitoring & Logging

### Logging
- Structured logging with Microsoft.Extensions.Logging
- Log levels: Information, Warning, Error
- Centralized logging với Log Service (nhận log qua RabbitMQ)
- Audit trail cho CRUD operations, Login/Logout
- Future: ELK/Seq cho application logs

### Health Checks
- Mỗi service có `/health` endpoint
- Ready for Kubernetes probes

## Deployment

### Docker
- Multi-stage builds
- Optimized image sizes
- Docker Compose for local development

### Kubernetes (Future)
- Deployment manifests
- Services & Ingress
- ConfigMaps & Secrets
- Horizontal Pod Autoscaling

## Development Guidelines

### Code Style
- C# 12 features
- File-scoped namespaces
- Nullable reference types
- Async/await throughout

### Testing
- Unit tests với xUnit
- Integration tests
- Mocking với Moq

### CI/CD
- GitHub Actions
- Automated build & test
- Docker image build
- Deploy to environments

## Future Enhancements

1. **Message Queue**: RabbitMQ/Azure Service Bus cho async communication
2. **API Versioning**: Support multiple API versions
3. **Rate Limiting**: Throttle requests
4. **Distributed Tracing**: OpenTelemetry
5. **GraphQL Gateway**: Alternative to REST
6. **gRPC**: High-performance inter-service communication
