# Task Planning - AXDD Project Implementation
# Kế Hoạch Thực Hiện Dự Án AXDD

**Ngày tạo:** 06/02/2026  
**Trạng thái:** Đang thực hiện (In Progress)  
**Phiên bản:** 1.0

---

## 📋 Yêu Cầu Gốc (Original Requirements)

### Nguồn: Issue "Viết codebase function cho toàn bộ các service"

#### Mục tiêu chung:
Xây dựng hệ thống quản lý CSDL KCN Đồng Nai với kiến trúc Microservices, bao gồm:

1. **Web App Quản Lý Nội Bộ (Angular)**
   - Quản lý hồ sơ tài liệu
   - Quản lý thông tin doanh nghiệp (2,100+ doanh nghiệp)
   - Quản lý các dự án, giấy phép
   - Upload tài liệu đính kèm → gửi sang File Service

2. **Web App Portal Doanh Nghiệp**
   - Nộp báo cáo
   - Xem kết quả phê duyệt
   - Nhận thông báo từ hệ thống

3. **Hệ Thống Backend Services**
   - **Auth Service**: ASP.NET Identity + SQL Server
   - **File Service**: MinIO (lưu file) + SQL Server (metadata)
     - API: upload, download, view file
     - Tổ chức folder theo mã doanh nghiệp
   - **GIS Service**: OpenStreetMap hoặc Google Maps
     - Nhập tọa độ doanh nghiệp
     - View bản đồ
   - **Search Service**: Elasticsearch
     - Index thông tin doanh nghiệp
     - Index hồ sơ doanh nghiệp
     - Tìm kiếm thông tin liên quan
   - **Logging Service**: Ghi log, xem log

4. **Quy Trình Nghiệp Vụ**
   - Cán bộ phê duyệt/từ chối báo cáo
   - Hệ thống gửi thông báo nội bộ
   - Doanh nghiệp đăng nhập và xem thông báo

---

## 🎯 Phân Tích Yêu Cầu Kỹ Thuật (Technical Analysis)

### Từ Tài Liệu Đặc Tả:

#### Quy mô hệ thống:
- **2,100+ doanh nghiệp** trong các KCN
- **~1.4 triệu trang** tài liệu số hóa (3,447 hồ sơ)
- **500+ concurrent users**
- **99.5% uptime** target

#### Kiến trúc:
- Microservices với .NET 9
- API Gateway (YARP)
- Message Queue (RabbitMQ)
- Caching (Redis)
- Elasticsearch cho full-text search
- PostgreSQL + PostGIS cho GIS
- SQL Server cho business data
- MinIO cho object storage

#### Tích hợp:
- LGSP (Cổng dịch vụ công)
- VNeID (Single Sign-On)
- QLVB (Quản lý văn bản)
- Một cửa (One-stop portal)

---

## 📊 Danh Sách Công Việc (Task List)

### ✅ Phase 1: Core Infrastructure & Building Blocks (HOÀN THÀNH)
**Thời gian:** 06/02/2026  
**Trạng thái:** ✅ COMPLETED  

#### Chi tiết:
- [x] Solution structure (11 projects)
- [x] BuildingBlocks.Common (DTOs, Extensions, Middleware)
- [x] BuildingBlocks.Domain (Base Entities, Repository Interfaces, Value Objects)
- [x] BuildingBlocks.Infrastructure (Repository Implementation, UnitOfWork, DbContext)
- [x] API Gateway với YARP
- [x] Docker compose configuration
- [x] Repository Pattern + Unit of Work
- [x] Domain Events Infrastructure
- [x] Value Objects (PhoneNumber, Email, TaxCode - Vietnamese validation)
- [x] Result<T> pattern
- [x] Exception types (NotFoundException, ValidationException, BusinessRuleException)

#### Kết quả:
- **29 files** created
- **+2,247 lines** of code
- **0 errors, 0 warnings**

---

### ✅ Phase 2: Auth Service (HOÀN THÀNH)
**Thời gian:** 06/02/2026  
**Trạng thái:** ✅ COMPLETED

#### Chi tiết:
- [x] ASP.NET Identity integration
- [x] Database: AuthDbContext + SQL Server
- [x] Entities: ApplicationUser, ApplicationRole, RefreshToken, UserSession
- [x] Services: AuthService, JwtService, UserService, RoleService
- [x] Controllers:
  - [x] AuthController (8 endpoints): login, register, refresh, logout, change-password, forgot-password, reset-password, user-info
  - [x] UsersController (6 endpoints): CRUD + role assignment
  - [x] RolesController (5 endpoints): CRUD roles
- [x] JWT ******  generation
- [x] Refresh token rotation
- [x] Password policy enforcement
- [x] Account lockout (5 attempts)
- [x] Database migrations + seed data (Admin, Staff, Enterprise roles)
- [x] Complete documentation (5 guides)

#### Kết quả:
- **31 files** created
- **19 API endpoints**
- **+3,500 lines** of code
- Default admin: admin@axdd.com / Admin@123

---

### 🔄 Phase 3: File Service (MinIO + SQL Server) - ĐANG THỰC HIỆN
**Thời gian dự kiến:** 06/02/2026  
**Trạng thái:** ⏳ IN PROGRESS

#### Mục tiêu:
Xây dựng service quản lý file với MinIO object storage và SQL Server metadata.

#### Công việc:
- [ ] **Database & Entities**
  - [ ] FileMetadata entity (Id, FileName, FileSize, MimeType, BucketName, ObjectKey, EnterpriseCode, FolderId, Version, etc.)
  - [ ] Folder entity (organize by enterprise code)
  - [ ] FileVersion entity (version history)
  - [ ] FileShare entity (sharing permissions)
  - [ ] FileDbContext

- [ ] **MinIO Integration**
  - [ ] MinIO client setup (Minio SDK)
  - [ ] Bucket configuration (axdd-documents, axdd-attachments, axdd-temp, axdd-archives)
  - [ ] Connection settings

- [ ] **Services**
  - [ ] IFileService:
    - [ ] UploadAsync (multipart/form-data)
    - [ ] DownloadAsync (streaming)
    - [ ] ViewAsync (get file URL or stream for preview)
    - [ ] DeleteAsync
    - [ ] GetFileMetadataAsync
    - [ ] ListFilesAsync (by folder, enterprise code)
  - [ ] IFolderService:
    - [ ] CreateFolderAsync (organize by enterprise code)
    - [ ] GetFolderAsync
    - [ ] DeleteFolderAsync (soft delete)
  - [ ] IFileVersionService (versioning support)
  - [ ] IFileShareService (sharing and permissions)
  - [ ] IStorageQuotaService (quota management)

- [ ] **Controllers**
  - [ ] FilesController:
    - [ ] POST /api/v1/files/upload
    - [ ] GET /api/v1/files/{id}/download
    - [ ] GET /api/v1/files/{id}/view
    - [ ] GET /api/v1/files/{id}
    - [ ] DELETE /api/v1/files/{id}
    - [ ] GET /api/v1/files (list with filters)
  - [ ] FoldersController:
    - [ ] POST /api/v1/folders (create folder by enterprise code)
    - [ ] GET /api/v1/folders/{id}
    - [ ] GET /api/v1/folders (list)
    - [ ] DELETE /api/v1/folders/{id}

- [ ] **Features**
  - [ ] Upload progress tracking (SignalR)
  - [ ] File versioning
  - [ ] Access control (who can view/download)
  - [ ] Storage quota per enterprise
  - [ ] Virus scanning placeholder
  - [ ] Thumbnail generation (images, PDFs)

- [ ] **Configuration**
  - [ ] appsettings.json (MinIO endpoint, credentials, bucket names)
  - [ ] Database migration
  - [ ] Swagger documentation

#### Yêu cầu kỹ thuật:
- Minio SDK
- Streaming for large files
- Metadata in SQL Server
- Folder hierarchy by enterprise code
- Version control
- Access permissions

---

### 📍 Phase 4: GIS Service (PostGIS + OpenStreetMap) - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 07/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Service quản lý thông tin địa lý và bản đồ.

#### Công việc:
- [ ] **Database**
  - [ ] PostgreSQL 16 + PostGIS 3.4 setup
  - [ ] GIS_DB database
  - [ ] Entities: IndustrialZone (with geometry), Enterprise Location, Land Plot

- [ ] **NetTopologySuite Integration**
  - [ ] Spatial types (Point, Polygon, LineString)
  - [ ] Spatial indexes (GIST)

- [ ] **Services**
  - [ ] IGisService:
    - [ ] SaveEnterpriseLocationAsync (latitude, longitude)
    - [ ] GetEnterpriseLocationAsync
    - [ ] GetIndustrialZoneBoundaryAsync
    - [ ] SpatialQueryAsync (point-in-polygon, buffering, distance)
  - [ ] IMapService:
    - [ ] GetMapTilesAsync (OpenStreetMap or Google Maps integration)
    - [ ] RenderMapAsync

- [ ] **Controllers**
  - [ ] GisController:
    - [ ] POST /api/v1/gis/enterprises/{id}/location
    - [ ] GET /api/v1/gis/enterprises/{id}/location
    - [ ] GET /api/v1/gis/industrial-zones/{id}/boundary
    - [ ] POST /api/v1/gis/spatial-query
  - [ ] MapsController:
    - [ ] GET /api/v1/maps/view

- [ ] **Features**
  - [ ] Coordinate input validation
  - [ ] Map rendering with OpenStreetMap
  - [ ] Industrial zone boundary visualization
  - [ ] Distance calculations
  - [ ] Spatial queries

#### Yêu cầu kỹ thuật:
- Npgsql.EntityFrameworkCore.PostgreSQL
- NetTopologySuite
- NetTopologySuite.IO.GeoJSON
- OpenStreetMap integration or Google Maps API

---

### 🔍 Phase 5: Search Service (Elasticsearch) - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 08/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Full-text search service cho enterprises, documents, projects.

#### Công việc:
- [ ] **Elasticsearch Setup**
  - [ ] Elasticsearch 8.x client (NEST/Elastic.Clients.Elasticsearch)
  - [ ] Connection configuration

- [ ] **Indexes**
  - [ ] enterprises_idx (name, taxCode, address, industry, status)
  - [ ] documents_idx (fileName, content, metadata)
  - [ ] projects_idx (projectName, description, status)

- [ ] **Services**
  - [ ] ISearchService:
    - [ ] IndexEnterpriseAsync
    - [ ] SearchEnterprisesAsync (full-text, filters, pagination)
    - [ ] GetSuggestionsAsync (autocomplete)
    - [ ] DeleteFromIndexAsync
  - [ ] ISearchIndexer:
    - [ ] BulkIndexAsync
    - [ ] ReindexAllAsync

- [ ] **Controllers**
  - [ ] SearchController:
    - [ ] POST /api/v1/search/enterprises
    - [ ] POST /api/v1/search/documents
    - [ ] GET /api/v1/search/suggestions

- [ ] **Features**
  - [ ] Full-text search with Vietnamese analyzer
  - [ ] Faceted search
  - [ ] Autocomplete/suggestions
  - [ ] Fuzzy search
  - [ ] Search result ranking
  - [ ] Highlighting

#### Yêu cầu kỹ thuật:
- Elasticsearch 8.x
- NEST or Elastic.Clients.Elasticsearch
- Vietnamese language analyzer

---

### 🔔 Phase 6: Notification Service - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 09/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Service thông báo realtime và email.

#### Công việc:
- [ ] **Database & Entities**
  - [ ] Notification entity
  - [ ] NotificationTemplate entity
  - [ ] UserNotificationPreference entity

- [ ] **SignalR Integration**
  - [ ] NotificationHub (realtime notifications)
  - [ ] Connection management

- [ ] **Services**
  - [ ] INotificationService:
    - [ ] SendNotificationAsync (in-app)
    - [ ] SendEmailAsync
    - [ ] GetUserNotificationsAsync
    - [ ] MarkAsReadAsync
  - [ ] IEmailService (SMTP)
  - [ ] ITemplateService (notification templates)

- [ ] **Controllers**
  - [ ] NotificationsController:
    - [ ] GET /api/v1/notifications
    - [ ] PUT /api/v1/notifications/{id}/read
    - [ ] DELETE /api/v1/notifications/{id}

- [ ] **Features**
  - [ ] Real-time via SignalR
  - [ ] Email notifications
  - [ ] Notification templates
  - [ ] User preferences
  - [ ] Notification history

#### Yêu cầu kỹ thuật:
- SignalR
- SMTP client (MailKit)
- RabbitMQ for async notifications

---

### 🏢 Phase 7: Enterprise Service - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 10/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Core business service quản lý doanh nghiệp (2,100+ enterprises).

#### Công việc:
- [ ] **Database & Entities**
  - [ ] Enterprise entity (comprehensive)
  - [ ] ContactPerson entity
  - [ ] EnterpriseLicense entity
  - [ ] EnterpriseStatus workflow

- [ ] **Services**
  - [ ] IEnterpriseService (CRUD + advanced queries)
  - [ ] IContactPersonService
  - [ ] ILicenseService

- [ ] **Controllers**
  - [ ] EnterprisesController (full CRUD + search)

- [ ] **Features**
  - [ ] Enterprise profile management
  - [ ] Industry classification
  - [ ] Industrial zone assignment
  - [ ] Status workflow
  - [ ] Contact management

---

### 💼 Phase 8: Investment Service - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 11/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Quản lý giấy phép đầu tư (GCNĐKĐT).

#### Công việc:
- [ ] Investment Certificate CRUD
- [ ] Adjustment workflow
- [ ] Extension/Revocation
- [ ] Project tracking

---

### 📊 Phase 9: Report Service - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 12/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Service báo cáo và phê duyệt.

#### Công việc:
- [ ] Enterprise report submission
- [ ] Approval/Rejection workflow
- [ ] Notification integration
- [ ] Report templates
- [ ] Dashboard

---

### 📚 Phase 10: MasterData Service - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 13/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Service dữ liệu danh mục.

#### Công việc:
- [ ] Administrative divisions
- [ ] Industrial zones catalog
- [ ] Industry classification (VSIC)
- [ ] Certificate types
- [ ] Document types
- [ ] Caching strategy

---

### 📝 Phase 11: Logging & Monitoring - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 14/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Logging và audit trail.

#### Công việc:
- [ ] Structured logging (Serilog)
- [ ] Audit log database
- [ ] Log viewing endpoints
- [ ] User activity tracking

---

### 🧪 Phase 12: Testing & Documentation - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 15/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Unit tests và integration tests.

#### Công việc:
- [ ] Unit tests cho các services
- [ ] Integration tests
- [ ] API documentation
- [ ] Deployment guides

---

### 🌐 Phase 13: Frontend Integration - CHỜ THỰC HIỆN
**Thời gian dự kiến:** 16/02/2026  
**Trạng thái:** 🔜 PENDING

#### Mục tiêu:
Frontend integration points.

#### Công việc:
- [ ] CORS configuration
- [ ] API Gateway routing
- [ ] Authentication flow docs
- [ ] File upload/download examples
- [ ] WebSocket/SignalR guide

---

## 📈 Tiến Độ Tổng Thể (Overall Progress)

### Thống kê:
- **Tổng số phases:** 13
- **Phases hoàn thành:** 2 ✅
- **Phases đang thực hiện:** 1 ⏳
- **Phases chờ thực hiện:** 10 🔜
- **Tiến độ:** 15% (2/13 phases)

### Biểu đồ tiến độ:
```
Phase 1  [████████████████████] 100% ✅ COMPLETED
Phase 2  [████████████████████] 100% ✅ COMPLETED
Phase 3  [████░░░░░░░░░░░░░░░░]  20% ⏳ IN PROGRESS
Phase 4  [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 5  [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 6  [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 7  [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 8  [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 9  [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 10 [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 11 [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 12 [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
Phase 13 [░░░░░░░░░░░░░░░░░░░░]   0% 🔜 PENDING
```

### Thành tựu đã đạt được:
✅ **60 files** created  
✅ **+5,747 lines** of production code  
✅ **19 API endpoints** implemented  
✅ **0 errors, 0 warnings** in builds  
✅ Complete authentication system  
✅ Repository pattern infrastructure  
✅ Domain-driven design foundation  

---

## 🎯 Ưu Tiên Tiếp Theo (Next Priorities)

### Tuần này (This Week):
1. ✅ **Phase 1-2**: Hoàn thành (DONE)
2. ⏳ **Phase 3**: File Service (IN PROGRESS)
3. 🔜 **Phase 4**: GIS Service (NEXT)
4. 🔜 **Phase 5**: Search Service

### Lý do ưu tiên:
- **File Service**: Quan trọng cho upload/download tài liệu (1.4M pages)
- **GIS Service**: Cần thiết cho quản lý vị trí doanh nghiệp
- **Search Service**: Core cho tìm kiếm 2,100+ enterprises
- **Enterprise Service**: Business domain chính

---

## 📝 Ghi Chú (Notes)

### Quyết định kỹ thuật:
1. **Kiến trúc**: Microservices với Database per Service pattern
2. **Framework**: .NET 9 (latest)
3. **ORM**: Entity Framework Core 9
4. **API Gateway**: YARP (Yet Another Reverse Proxy)
5. **Authentication**: JWT ******  với ASP.NET Identity
6. **Caching**: Redis 7.x
7. **Message Queue**: RabbitMQ 3.12.x
8. **Search**: Elasticsearch 8.x
9. **Object Storage**: MinIO (S3-compatible)
10. **GIS Database**: PostgreSQL 16 + PostGIS 3.4

### Challenges & Risks:
- ⚠️ Large dataset: 1.4M pages documents
- ⚠️ 2,100+ enterprises data migration
- ⚠️ Integration with government systems (LGSP, VNeID)
- ⚠️ Performance optimization for 500+ concurrent users
- ⚠️ Security compliance (Level 3 per NĐ 85/2016)

### Dependencies:
- MinIO server setup
- Elasticsearch cluster setup
- PostgreSQL + PostGIS server
- SQL Server 2022
- Redis server
- RabbitMQ server

---

## 📞 Liên Hệ & Tài Liệu (Contact & Documentation)

### Tài liệu kỹ thuật:
- `docs/technical-spec/technical_specification.md` - Đặc tả kỹ thuật chi tiết
- `docs/technical-spec/database_design.md` - Thiết kế database
- `docs/technical-spec/api_specification.md` - API specification
- `docs/architecture/` - Kiến trúc hệ thống

### Documentation đã tạo:
- `docs/building-blocks/implementation-summary.md`
- `src/Services/Auth/AXDD.Services.Auth.Api/README.md`
- `src/Services/Auth/AXDD.Services.Auth.Api/QUICKSTART.md`
- `src/Services/Auth/AXDD.Services.Auth.Api/SECURITY_SUMMARY.md`

### Source code:
- Repository: `/home/runner/work/AXDD/AXDD`
- Branch: `copilot/write-codebase-function-for-services`

---

## 📅 Lịch Sử Cập Nhật (Update History)

| Ngày | Phiên bản | Nội dung |
|------|-----------|----------|
| 06/02/2026 | 1.0 | Tạo file task planning ban đầu |
| 06/02/2026 | 1.0 | Phase 1 & 2 completed |

---

**Ghi chú:** File này sẽ được cập nhật thường xuyên theo tiến độ thực hiện dự án.

**Người thực hiện:** GitHub Copilot Agent  
**Ngôn ngữ:** Vietnamese (Tiếng Việt) & English  
**Framework:** .NET 9  
**Architecture:** Microservices
