# Đặc Tả Kỹ Thuật Chi Tiết
# Hệ Thống AXDD - Quản Lý CSDL KCN Đồng Nai

**Phiên bản:** 2.0  
**Ngày cập nhật:** 05/02/2025  
**Tác giả:** Solution Architect Team

---

## Mục Lục

1. [Tổng Quan Kiến Trúc Hệ Thống](#1-tổng-quan-kiến-trúc-hệ-thống)
2. [Chi Tiết Từng Microservice](#2-chi-tiết-từng-microservice)
3. [API Design Patterns và Conventions](#3-api-design-patterns-và-conventions)
4. [Database Schema Design](#4-database-schema-design)
5. [Security & Authentication](#5-security--authentication)
6. [Performance Considerations](#6-performance-considerations)
7. [Deployment Strategy](#7-deployment-strategy)
8. [Monitoring & Logging](#8-monitoring--logging)
9. [Development Guidelines](#9-development-guidelines)

---

## 1. Tổng Quan Kiến Trúc Hệ Thống

### 1.1. Giới Thiệu

Hệ thống AXDD (CSDL KCN Đồng Nai) là một hệ thống quản lý cơ sở dữ liệu toàn diện cho Ban Quản lý các Khu công nghiệp tỉnh Đồng Nai, được thiết kế để:

- Quản lý thông tin **2.100+ doanh nghiệp** trong các KCN
- Số hóa và lưu trữ **~1.4 triệu trang** tài liệu
- Tích hợp liên thông với các hệ thống của Tỉnh (LGSP, Một cửa, QLVB)
- Hỗ trợ các nghiệp vụ: Đầu tư, Xây dựng, Môi trường, Lao động

### 1.2. Kiến Trúc Tổng Thể

Hệ thống được xây dựng theo kiến trúc **Microservices** với các đặc điểm:

```
┌─────────────────────────────────────────────────────────────────────┐
│                    KIẾN TRÚC TỔNG QUAN                               │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                    PRESENTATION LAYER                         │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐        │  │
│  │  │  Staff Web   │  │ Enterprise   │  │  Dashboard   │        │  │
│  │  │  App (Next.js)│  │  Web App     │  │  Web App     │        │  │
│  │  └──────────────┘  └──────────────┘  └──────────────┘        │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                              │                                      │
│                              │ HTTPS/REST                           │
│                              ▼                                      │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                       API GATEWAY                             │  │
│  │         (YARP/Ocelot - .NET 9)                               │  │
│  │  • Routing  • Auth  • Rate Limiting  • Load Balancing       │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                              │                                      │
│              ┌───────────────┼───────────────┐                     │
│              │               │               │                     │
│  ┌───────────▼────┐ ┌────────▼────────┐ ┌───▼──────────────┐     │
│  │  CORE SERVICES │ │ BUSINESS SERVICES│ │INTEGRATION SERVICES│    │
│  │                │ │                  │ │                  │     │
│  │• Auth          │ │• Investment      │ │• LGSP            │     │
│  │• MasterData    │ │• Environment     │ │• OCR             │     │
│  │• File          │ │• Construction    │ │• VNeID           │     │
│  │• Notification  │ │• Labor           │ │                  │     │
│  │• Search        │ │• Enterprise      │ │                  │     │
│  │• GIS           │ │• Inspection      │ │                  │     │
│  │                │ │• Report          │ │                  │     │
│  └────────────────┘ └──────────────────┘ └──────────────────┘     │
│              │               │               │                     │
│              └───────────────┼───────────────┘                     │
│                              │                                      │
│  ┌──────────────────────────▼───────────────────────────────────┐  │
│  │                       DATA LAYER                              │  │
│  │  ┌───────────┐ ┌────────────┐ ┌─────────┐ ┌──────────────┐  │  │
│  │  │SQL Server │ │PostgreSQL  │ │  MinIO  │ │ Elasticsearch│  │  │
│  │  │(Business) │ │  + PostGIS │ │(Object  │ │   (Search)   │  │  │
│  │  │           │ │   (GIS)    │ │Storage) │ │              │  │  │
│  │  └───────────┘ └────────────┘ └─────────┘ └──────────────┘  │  │
│  │  ┌───────────┐ ┌────────────┐                               │  │
│  │  │   Redis   │ │  RabbitMQ  │                               │  │
│  │  │  (Cache)  │ │   (Queue)  │                               │  │
│  │  └───────────┘ └────────────┘                               │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 1.3. Stack Công Nghệ

| Layer | Công Nghệ | Phiên Bản | Mô Tả |
|-------|-----------|-----------|-------|
| **Backend** | .NET Core | 9.0 | Framework chính cho microservices |
| **Frontend** | Next.js | 14.x | React framework với SSR |
| **API Gateway** | YARP/Ocelot | Latest | Reverse proxy và API Gateway |
| **RDBMS** | SQL Server | 2022 | Database chính cho nghiệp vụ |
| **GIS Database** | PostgreSQL + PostGIS | 16.x + 3.4 | Dữ liệu không gian địa lý |
| **Object Storage** | MinIO | Latest | S3-compatible storage |
| **Search Engine** | Elasticsearch | 8.x | Full-text search |
| **Cache** | Redis | 7.x | In-memory cache |
| **Message Queue** | RabbitMQ | 3.12.x | Async messaging |
| **Container** | Docker | Latest | Container runtime |
| **Orchestration** | Docker Compose | Latest | Container orchestration |

### 1.4. Nguyên Tắc Thiết Kế

#### 1.4.1. Domain-Driven Design (DDD)

- **Bounded Context**: Mỗi microservice đại diện cho một bounded context riêng biệt
- **Aggregate Root**: Mỗi entity chính là một aggregate root
- **Domain Events**: Sử dụng domain events để giao tiếp giữa các service

#### 1.4.2. SOLID Principles

- **Single Responsibility**: Mỗi class/method có một trách nhiệm duy nhất
- **Open/Closed**: Mở để mở rộng, đóng để sửa đổi
- **Liskov Substitution**: Có thể thay thế các implementation
- **Interface Segregation**: Interface nhỏ, tập trung
- **Dependency Inversion**: Phụ thuộc vào abstraction

#### 1.4.3. Clean Architecture

```
┌────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                   │
│         (API Controllers, SignalR Hubs)                │
└────────────────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                     │
│    (Use Cases, DTOs, Commands, Queries, Handlers)     │
└────────────────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                        │
│     (Entities, Value Objects, Domain Events,           │
│      Domain Services, Repository Interfaces)           │
└────────────────────────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────┐
│                 INFRASTRUCTURE LAYER                    │
│  (EF Core, External Services, File System, Cache)     │
└────────────────────────────────────────────────────────┘
```

#### 1.4.4. CQRS Pattern

- **Command**: Thực hiện thay đổi dữ liệu (CUD trong CRUD)
- **Query**: Đọc dữ liệu (R trong CRUD)
- **Separation of Concerns**: Tách biệt logic đọc và ghi

---

## 2. Chi Tiết Từng Microservice

### 2.1. Core Services (Platform Services)

#### 2.1.1. Auth Service

**Mục đích:** Xác thực và phân quyền tập trung cho toàn hệ thống

**Công nghệ:**
- .NET 9 Web API
- Identity Server / Duende IdentityServer
- JWT Token
- OAuth 2.0 / OpenID Connect

**Database:** SQL Server (AuthDB)

**Chức năng chính:**

1. **Authentication**
   - Login/Logout
   - SSO với VNeID
   - Refresh Token
   - Password reset
   - Two-Factor Authentication (2FA)

2. **Authorization**
   - Role-Based Access Control (RBAC)
   - Permission management
   - Resource-based authorization

3. **User Management**
   - User CRUD
   - User profile
   - User preferences

**API Endpoints:**

```
POST   /api/auth/login
POST   /api/auth/logout
POST   /api/auth/refresh-token
POST   /api/auth/forgot-password
POST   /api/auth/reset-password
POST   /api/auth/sso/vneid
GET    /api/auth/user-info

POST   /api/users
GET    /api/users/{id}
PUT    /api/users/{id}
DELETE /api/users/{id}
GET    /api/users

POST   /api/roles
GET    /api/roles/{id}
PUT    /api/roles/{id}
DELETE /api/roles/{id}
GET    /api/roles

POST   /api/permissions
GET    /api/permissions
```

**Database Schema:**

```sql
-- Users Table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(100) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    FullName NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    IsActive BIT DEFAULT 1,
    LastLoginAt DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    INDEX IX_Users_Username (Username),
    INDEX IX_Users_Email (Email)
);

-- Roles Table
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    IsSystem BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    INDEX IX_Roles_Name (Name)
);

-- Permissions Table
CREATE TABLE Permissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Resource NVARCHAR(100) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    INDEX IX_Permissions_Resource (Resource)
);

-- UserRoles Table
CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    AssignedAt DATETIME2 DEFAULT GETUTCDATE(),
    AssignedBy UNIQUEIDENTIFIER,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

-- RolePermissions Table
CREATE TABLE RolePermissions (
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
);

-- RefreshTokens Table
CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_RefreshTokens_Token (Token),
    INDEX IX_RefreshTokens_UserId (UserId)
);
```

**Configuration:**

```json
{
  "AuthService": {
    "JwtSettings": {
      "SecretKey": "{{SECRET_KEY}}",
      "Issuer": "AXDD.AuthService",
      "Audience": "AXDD.Services",
      "ExpirationMinutes": 60,
      "RefreshTokenExpirationDays": 7
    },
    "VNeIDIntegration": {
      "ClientId": "{{CLIENT_ID}}",
      "ClientSecret": "{{CLIENT_SECRET}}",
      "AuthorizationEndpoint": "https://vneid.gov.vn/oauth/authorize",
      "TokenEndpoint": "https://vneid.gov.vn/oauth/token",
      "UserInfoEndpoint": "https://vneid.gov.vn/oauth/userinfo"
    }
  }
}
```

---

#### 2.1.2. Master Data Service

**Mục đích:** Quản lý danh mục dùng chung cho toàn hệ thống

**Công nghệ:**
- .NET 9 Web API
- Entity Framework Core
- Redis Cache

**Database:** SQL Server (MasterDataDB)

**Chức năng chính:**

1. **Administrative Data**
   - Tỉnh/Thành phố
   - Quận/Huyện
   - Phường/Xã

2. **Industrial Zone Data**
   - Danh sách KCN
   - Loại hình KCN
   - Trạng thái KCN

3. **Industry Classification**
   - Ngành nghề kinh tế (VSIC 2018)
   - Loại hình doanh nghiệp

4. **Project Status**
   - Trạng thái dự án
   - Loại dự án

5. **Document Types**
   - Loại giấy phép
   - Loại tài liệu

**API Endpoints:**

```
GET    /api/master-data/provinces
GET    /api/master-data/districts/{provinceId}
GET    /api/master-data/wards/{districtId}

GET    /api/master-data/industrial-zones
GET    /api/master-data/industrial-zones/{id}
POST   /api/master-data/industrial-zones
PUT    /api/master-data/industrial-zones/{id}

GET    /api/master-data/industries
GET    /api/master-data/enterprise-types
GET    /api/master-data/project-statuses
GET    /api/master-data/document-types
```

**Caching Strategy:**

```csharp
public class MasterDataCacheSettings
{
    // Cache time-to-live
    public const int DefaultCacheDurationMinutes = 60;
    public const int AdministrativeCacheDurationHours = 24;
    
    // Cache keys
    public const string ProvincesCacheKey = "masterdata:provinces";
    public const string IndustrialZonesCacheKey = "masterdata:industrial-zones";
    public const string IndustriesCacheKey = "masterdata:industries";
}
```

---

#### 2.1.3. File Service

**Mục đích:** Quản lý tài liệu và file tương tự Dropbox/Google Drive

**Công nghệ:**
- .NET 9 Web API
- MinIO (S3-compatible)
- SignalR (upload progress)

**Database:** SQL Server (FileMetaDB)

**Chức năng chính:**

1. **File Operations**
   - Upload file
   - Download file
   - Delete file
   - Move/Copy file
   - Rename file

2. **Folder Management**
   - Create folder
   - Delete folder
   - Move folder
   - Folder hierarchy

3. **Sharing & Permissions**
   - Share file/folder
   - Permission management
   - Public links
   - Access control

4. **Version Control**
   - File versioning
   - Restore previous version

5. **Metadata**
   - File metadata
   - Tags
   - Search

**API Endpoints:**

```
POST   /api/files/upload
GET    /api/files/{id}/download
GET    /api/files/{id}
PUT    /api/files/{id}
DELETE /api/files/{id}
POST   /api/files/{id}/copy
POST   /api/files/{id}/move
GET    /api/files/search

POST   /api/folders
GET    /api/folders/{id}
PUT    /api/folders/{id}
DELETE /api/folders/{id}
GET    /api/folders/{id}/children

POST   /api/shares
GET    /api/shares/{id}
DELETE /api/shares/{id}
```

**Database Schema:**

```sql
-- Files Table
CREATE TABLE Files (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    OriginalName NVARCHAR(255) NOT NULL,
    Extension NVARCHAR(20),
    Size BIGINT NOT NULL,
    MimeType NVARCHAR(100),
    StoragePath NVARCHAR(500) NOT NULL,
    FolderId UNIQUEIDENTIFIER,
    Version INT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    DeletedAt DATETIME2,
    INDEX IX_Files_FolderId (FolderId),
    INDEX IX_Files_OwnerId (OwnerId),
    INDEX IX_Files_Name (Name)
);

-- Folders Table
CREATE TABLE Folders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    ParentId UNIQUEIDENTIFIER,
    Path NVARCHAR(1000),
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    INDEX IX_Folders_ParentId (ParentId),
    INDEX IX_Folders_OwnerId (OwnerId)
);

-- FileShares Table
CREATE TABLE FileShares (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FileId UNIQUEIDENTIFIER,
    FolderId UNIQUEIDENTIFIER,
    SharedWithUserId UNIQUEIDENTIFIER,
    Permission NVARCHAR(20) NOT NULL, -- Read, Write, Delete
    SharedBy UNIQUEIDENTIFIER NOT NULL,
    SharedAt DATETIME2 DEFAULT GETUTCDATE(),
    ExpiresAt DATETIME2,
    INDEX IX_FileShares_FileId (FileId),
    INDEX IX_FileShares_SharedWithUserId (SharedWithUserId)
);

-- FileVersions Table
CREATE TABLE FileVersions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FileId UNIQUEIDENTIFIER NOT NULL,
    VersionNumber INT NOT NULL,
    Size BIGINT NOT NULL,
    StoragePath NVARCHAR(500) NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (FileId) REFERENCES Files(Id) ON DELETE CASCADE
);
```

**MinIO Configuration:**

```json
{
  "MinIO": {
    "Endpoint": "minio:9000",
    "AccessKey": "{{ACCESS_KEY}}",
    "SecretKey": "{{SECRET_KEY}}",
    "UseSSL": false,
    "BucketName": "axdd-files",
    "Buckets": {
      "Documents": "axdd-documents",
      "Attachments": "axdd-attachments",
      "Archives": "axdd-archives",
      "Temp": "axdd-temp"
    }
  }
}
```

---

#### 2.1.4. OCR Service

**Mục đích:** Nhận dạng ký tự quang học và bóc tách thông tin từ giấy phép

**Công nghệ:**
- .NET 9 Web API
- Tesseract OCR / Azure Form Recognizer
- RabbitMQ (async processing)

**Database:** SQL Server (OCRDB)

**Chức năng chính:**

1. **OCR Processing**
   - Nhận dạng văn bản từ ảnh/PDF
   - Hỗ trợ 20+ mẫu giấy phép
   - Xử lý bất đồng bộ

2. **Data Extraction**
   - Bóc tách thông tin cấu trúc
   - Validation dữ liệu
   - Mapping to entities

3. **Queue Management**
   - Job queue
   - Retry mechanism
   - Priority processing

**API Endpoints:**

```
POST   /api/ocr/submit
GET    /api/ocr/jobs/{id}
GET    /api/ocr/jobs/{id}/result
GET    /api/ocr/templates
POST   /api/ocr/templates
```

**Supported Document Templates:**

1. Giấy chứng nhận đăng ký đầu tư
2. Giấy chứng nhận đăng ký doanh nghiệp
3. Giấy phép xây dựng
4. Giấy phép môi trường
5. Đánh giá tác động môi trường
6. Báo cáo sử dụng lao động
... (total 20 templates)

**Processing Flow:**

```
┌──────────┐      ┌──────────┐      ┌──────────┐      ┌──────────┐
│  Client  │─────>│   API    │─────>│ RabbitMQ │─────>│   OCR    │
│          │      │ Gateway  │      │  Queue   │      │  Worker  │
└──────────┘      └──────────┘      └──────────┘      └──────────┘
                                                             │
                                                             ▼
                                     ┌──────────────────────────────┐
                                     │  1. Get file from MinIO      │
                                     │  2. OCR Processing            │
                                     │  3. Data Extraction           │
                                     │  4. Validation                │
                                     │  5. Save result to DB         │
                                     │  6. Send notification         │
                                     └──────────────────────────────┘
```

---

#### 2.1.5. Search Service

**Mục đích:** Tìm kiếm full-text trên toàn hệ thống

**Công nghệ:**
- .NET 9 Web API
- Elasticsearch 8.x
- NEST (Elasticsearch .NET client)

**Database:** Elasticsearch

**Chức năng chính:**

1. **Indexing**
   - Index doanh nghiệp
   - Index dự án
   - Index tài liệu
   - Index giấy phép

2. **Searching**
   - Full-text search
   - Faceted search
   - Autocomplete
   - Fuzzy search

3. **Analytics**
   - Search analytics
   - Popular searches
   - Search suggestions

**API Endpoints:**

```
POST   /api/search/enterprises
POST   /api/search/projects
POST   /api/search/documents
GET    /api/search/suggestions
GET    /api/search/analytics
```

**Elasticsearch Indices:**

```json
{
  "enterprises_idx": {
    "mappings": {
      "properties": {
        "id": { "type": "keyword" },
        "name": { 
          "type": "text",
          "analyzer": "vietnamese",
          "fields": {
            "keyword": { "type": "keyword" }
          }
        },
        "taxCode": { "type": "keyword" },
        "address": { "type": "text", "analyzer": "vietnamese" },
        "industryCode": { "type": "keyword" },
        "industryName": { "type": "text", "analyzer": "vietnamese" },
        "status": { "type": "keyword" },
        "registeredDate": { "type": "date" },
        "createdAt": { "type": "date" },
        "suggest": {
          "type": "completion",
          "analyzer": "simple"
        }
      }
    }
  }
}
```

---

#### 2.1.6. Notification Service

**Mục đích:** Gửi thông báo qua nhiều kênh

**Công nghệ:**
- .NET 9 Web API
- SignalR (real-time)
- SMTP (email)
- RabbitMQ

**Database:** SQL Server (NotifyDB)

**Chức năng chính:**

1. **Email Notifications**
   - Template-based emails
   - Bulk emails
   - Email tracking

2. **In-App Notifications**
   - Real-time notifications
   - Notification center
   - Read/unread status

3. **Push Notifications** (future)
   - Mobile push
   - Browser push

**API Endpoints:**

```
POST   /api/notifications/send
GET    /api/notifications
GET    /api/notifications/{id}
PUT    /api/notifications/{id}/read
DELETE /api/notifications/{id}

POST   /api/notifications/templates
GET    /api/notifications/templates
```

---

#### 2.1.7. GIS Service

**Mục đích:** Quản lý dữ liệu bản đồ và không gian địa lý

**Công nghệ:**
- .NET 9 Web API
- NetTopologySuite
- PostgreSQL + PostGIS

**Database:** PostgreSQL with PostGIS extension

**Chức năng chính:**

1. **Map Management**
   - KCN boundaries
   - Enterprise locations
   - Land plots
   - Infrastructure

2. **Spatial Queries**
   - Point in polygon
   - Distance calculations
   - Buffer zones
   - Intersection

3. **Map Layers**
   - Layer management
   - Layer styles
   - Layer permissions

**API Endpoints:**

```
GET    /api/gis/industrial-zones
GET    /api/gis/industrial-zones/{id}/boundary
GET    /api/gis/enterprises/{id}/location
GET    /api/gis/plots
POST   /api/gis/spatial-query
GET    /api/gis/layers
```

**PostGIS Schema:**

```sql
-- Industrial Zone boundaries
CREATE TABLE IndustrialZones (
    Id UUID PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Code VARCHAR(50) UNIQUE NOT NULL,
    Area DECIMAL(15,2), -- in hectares
    Boundary GEOMETRY(POLYGON, 4326),
    CreatedAt TIMESTAMP DEFAULT NOW()
);

CREATE INDEX idx_industrial_zones_boundary 
    ON IndustrialZones USING GIST(Boundary);

-- Enterprise locations
CREATE TABLE EnterpriseLocations (
    Id UUID PRIMARY KEY,
    EnterpriseId UUID NOT NULL,
    IndustrialZoneId UUID,
    Location GEOMETRY(POINT, 4326),
    Address TEXT,
    CreatedAt TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (IndustrialZoneId) REFERENCES IndustrialZones(Id)
);

CREATE INDEX idx_enterprise_locations_location 
    ON EnterpriseLocations USING GIST(Location);

-- Land plots
CREATE TABLE LandPlots (
    Id UUID PRIMARY KEY,
    PlotNumber VARCHAR(50),
    IndustrialZoneId UUID,
    Area DECIMAL(15,2),
    Geometry GEOMETRY(POLYGON, 4326),
    Status VARCHAR(50),
    FOREIGN KEY (IndustrialZoneId) REFERENCES IndustrialZones(Id)
);

CREATE INDEX idx_land_plots_geometry 
    ON LandPlots USING GIST(Geometry);
```

---

### 2.2. Business Services (Domain Services)

#### 2.2.1. Investment Service

**Mục đích:** Quản lý đầu tư và giấy chứng nhận đăng ký đầu tư

**Database:** SQL Server (InvestmentDB)

**Chức năng chính:**

1. **Investment Certificate Management**
   - Cấp mới GCNĐKĐT
   - Điều chỉnh GCNĐKĐT
   - Gia hạn
   - Thu hồi

2. **Investor Management**
   - Thông tin nhà đầu tư
   - Lịch sử đầu tư

3. **Project Management**
   - Thông tin dự án đầu tư
   - Vốn đầu tư
   - Tiến độ thực hiện

**API Endpoints:**

```
POST   /api/investment/certificates
GET    /api/investment/certificates/{id}
PUT    /api/investment/certificates/{id}
DELETE /api/investment/certificates/{id}
POST   /api/investment/certificates/{id}/adjust
POST   /api/investment/certificates/{id}/extend
POST   /api/investment/certificates/{id}/revoke

GET    /api/investment/investors
POST   /api/investment/investors
GET    /api/investment/investors/{id}

GET    /api/investment/projects
POST   /api/investment/projects
GET    /api/investment/projects/{id}
PUT    /api/investment/projects/{id}
```

---

#### 2.2.2. Environment Service

**Mục đích:** Quản lý môi trường và giấy phép môi trường

**Database:** SQL Server (EnvironmentDB)

**Chức năng chính:**

1. **EIA Management**
   - Đánh giá tác động môi trường (ĐTM)
   - Phê duyệt ĐTM

2. **Environmental License**
   - Giấy phép môi trường
   - Điều chỉnh, gia hạn

3. **Trial Operation**
   - Vận hành thử nghiệm
   - Báo cáo kết quả

**API Endpoints:**

```
POST   /api/environment/eia
GET    /api/environment/eia/{id}
PUT    /api/environment/eia/{id}

POST   /api/environment/licenses
GET    /api/environment/licenses/{id}
PUT    /api/environment/licenses/{id}

POST   /api/environment/trial-operations
GET    /api/environment/trial-operations/{id}
```

---

#### 2.2.3. Construction Service

**Mục đích:** Quản lý xây dựng và giấy phép xây dựng

**Database:** SQL Server (ConstructionDB)

**Chức năng chính:**

1. **Construction Permit**
   - Giấy phép xây dựng
   - Điều chỉnh, gia hạn

2. **Building Management**
   - Thông tin công trình
   - Tiến độ xây dựng

3. **Technical Review**
   - Thẩm định báo cáo NCKT
   - Nghiệm thu công trình

---

#### 2.2.4. Labor Service

**Mục đích:** Quản lý lao động

**Database:** SQL Server (LaborDB)

**Chức năng chính:**

1. **Labor Regulations**
   - Nội quy lao động
   - Thỏa ước lao động

2. **Labor Reports**
   - Báo cáo sử dụng lao động
   - Thống kê lao động

---

#### 2.2.5. Enterprise Service

**Mục đích:** Quản lý doanh nghiệp

**Database:** SQL Server (EnterpriseDB)

**Chức năng chính:**

1. **Enterprise Management**
   - CRUD doanh nghiệp
   - Thông tin doanh nghiệp
   - Trạng thái hoạt động

2. **Rewards & Recognition**
   - Thi đua khen thưởng
   - Danh hiệu

---

#### 2.2.6. Inspection Service

**Mục đích:** Thanh tra kiểm tra

**Database:** SQL Server (InspectionDB)

**Chức năng chính:**

1. **Inspection Management**
   - Kế hoạch thanh tra
   - Thực hiện thanh tra
   - Kết luận thanh tra

2. **Violation Management**
   - Vi phạm
   - Xử phạt
   - Theo dõi khắc phục

---

#### 2.2.7. Report Service

**Mục đích:** Báo cáo và thống kê

**Database:** SQL Server (ReportDB)

**Chức năng chính:**

1. **Dashboard**
   - Dashboard điều hành
   - Thống kê tổng quan

2. **Reports**
   - Báo cáo định kỳ
   - Báo cáo chuyên đề
   - Export to Excel/PDF

3. **Alerts**
   - Cảnh báo hệ thống
   - Cảnh báo nghiệp vụ

---

### 2.3. Integration Services

#### 2.3.1. LGSP Service

**Mục đích:** Tích hợp với LGSP Tỉnh Đồng Nai

**Chức năng:**
- Đồng bộ dữ liệu Một cửa
- Đồng bộ QLVB điện tử
- Data exchange

---

## 3. API Design Patterns và Conventions

_(Xem file api_specification.md để biết chi tiết)_

---

## 4. Database Schema Design

_(Xem file database_design.md để biết chi tiết)_

---

## 5. Security & Authentication

### 5.1. Security Layers

```
┌─────────────────────────────────────────────────────────┐
│                  SECURITY ARCHITECTURE                   │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Layer 1: NETWORK SECURITY                              │
│  • Firewall rules                                       │
│  • DMZ configuration                                    │
│  • VPN for admin access                                 │
│                                                         │
│  Layer 2: API GATEWAY SECURITY                          │
│  • JWT Token validation                                 │
│  • Rate limiting (100 req/min per user)                 │
│  • IP whitelisting (admin APIs)                         │
│  • CORS policy                                          │
│                                                         │
│  Layer 3: APPLICATION SECURITY                          │
│  • OAuth 2.0 / OpenID Connect                           │
│  • Role-Based Access Control (RBAC)                     │
│  • Resource-based authorization                         │
│  • Input validation                                     │
│  • XSS protection                                       │
│  • CSRF protection                                      │
│                                                         │
│  Layer 4: DATA SECURITY                                 │
│  • TLS 1.3 for data in transit                          │
│  • Encryption at rest (SQL Server TDE)                  │
│  • Password hashing (BCrypt)                            │
│  • Sensitive data masking                               │
│  • SQL injection prevention (EF Core)                   │
│                                                         │
│  Layer 5: AUDIT & MONITORING                            │
│  • Audit logging (all CRUD operations)                  │
│  • Security event logging                               │
│  • Anomaly detection                                    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 5.2. Authentication Flow

```
┌──────────┐                           ┌──────────┐
│  Client  │                           │   Auth   │
│          │                           │  Service │
└────┬─────┘                           └────┬─────┘
     │                                      │
     │  1. POST /api/auth/login             │
     │  { username, password }              │
     │─────────────────────────────────────>│
     │                                      │
     │                                      │ 2. Validate credentials
     │                                      │ 3. Generate JWT + Refresh Token
     │                                      │
     │  4. { accessToken, refreshToken }    │
     │<─────────────────────────────────────│
     │                                      │
     │                                      │
     │  5. GET /api/enterprises             │
     │  Authorization: Bearer {accessToken} │
     │─────────────────────────────────────>│ API Gateway
     │                                      │
     │                                      │ 6. Validate JWT
     │                                      │ 7. Forward to Enterprise Service
     │                                      │
     │  8. { data }                         │
     │<─────────────────────────────────────│
     │                                      │
```

### 5.3. Authorization Model

**RBAC (Role-Based Access Control)**

```
User ─── has ───> Roles ─── have ───> Permissions
  │                                         │
  │                                         │
  └─────────── can access ──────────────────┘
                 Resources
```

**Predefined Roles:**

| Role | Description | Permissions |
|------|-------------|-------------|
| **SuperAdmin** | Quản trị hệ thống | Full access |
| **Admin** | Quản trị viên | All except system config |
| **Manager** | Trưởng phòng | Read/Write in department |
| **Staff** | Cán bộ | Read/Write assigned tasks |
| **Enterprise** | Doanh nghiệp | Read/Update own data |
| **Viewer** | Xem thông tin | Read-only |

**Permission Format:**

```
{Resource}:{Action}

Examples:
- Enterprise:Read
- Enterprise:Create
- Enterprise:Update
- Enterprise:Delete
- Investment:Approve
- Report:Export
```

### 5.4. Data Protection

**Encryption:**

```csharp
// Data in transit - TLS 1.3
services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
});

// Data at rest - SQL Server TDE
ALTER DATABASE InvestmentDB SET ENCRYPTION ON;

// Password hashing - BCrypt
var passwordHash = BCrypt.Net.BCrypt.HashPassword(
    password, 
    BCrypt.Net.BCrypt.GenerateSalt(12)
);

// Sensitive data encryption - AES256
public class DataProtectionService
{
    private readonly IDataProtector _protector;
    
    public string Protect(string plainText) => 
        _protector.Protect(plainText);
        
    public string Unprotect(string cipherText) => 
        _protector.Unprotect(cipherText);
}
```

---

## 6. Performance Considerations

### 6.1. Performance Requirements

| Metric | Target | Strategy |
|--------|--------|----------|
| **CCU** | ≥ 500 users | Load balancing, horizontal scaling |
| **Response Time** | < 10s per request | Caching, query optimization |
| **API Throughput** | 100 req/sec | Rate limiting, async processing |
| **Database Query** | < 100ms | Indexing, read replicas |
| **Search Latency** | < 1s | Elasticsearch optimization |
| **File Upload** | 100MB max | Chunked upload, MinIO |

### 6.2. Caching Strategy

**Redis Caching:**

```csharp
// Cache configuration
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = "AXDD:";
});

// Cache aside pattern
public async Task<Enterprise> GetEnterpriseAsync(Guid id)
{
    var cacheKey = $"enterprise:{id}";
    
    // Try get from cache
    var cached = await _cache.GetStringAsync(cacheKey);
    if (cached != null)
        return JsonSerializer.Deserialize<Enterprise>(cached);
    
    // Get from database
    var enterprise = await _dbContext.Enterprises
        .FindAsync(id);
    
    // Set cache
    await _cache.SetStringAsync(
        cacheKey, 
        JsonSerializer.Serialize(enterprise),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        }
    );
    
    return enterprise;
}
```

**Cache Invalidation:**

```csharp
public async Task UpdateEnterpriseAsync(Enterprise enterprise)
{
    // Update database
    _dbContext.Enterprises.Update(enterprise);
    await _dbContext.SaveChangesAsync();
    
    // Invalidate cache
    var cacheKey = $"enterprise:{enterprise.Id}";
    await _cache.RemoveAsync(cacheKey);
    
    // Publish domain event
    await _eventBus.PublishAsync(new EnterpriseUpdatedEvent(enterprise));
}
```

### 6.3. Database Optimization

**Indexing Strategy:**

```sql
-- Composite indexes for common queries
CREATE NONCLUSTERED INDEX IX_Enterprises_IndustryZone_Status
    ON Enterprises (IndustrialZoneId, Status)
    INCLUDE (Name, TaxCode, CreatedAt);

-- Full-text index for search
CREATE FULLTEXT INDEX ON Enterprises(Name, Address)
    KEY INDEX PK_Enterprises;

-- Filtered index for active records
CREATE NONCLUSTERED INDEX IX_Enterprises_Active
    ON Enterprises (Id, Name)
    WHERE Status = 'Active' AND IsDeleted = 0;
```

**Query Optimization:**

```csharp
// Use AsNoTracking for read-only queries
var enterprises = await _dbContext.Enterprises
    .AsNoTracking()
    .Where(e => e.Status == EnterpriseStatus.Active)
    .ToListAsync();

// Use pagination
var pagedResult = await _dbContext.Enterprises
    .OrderBy(e => e.Name)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// Use compiled queries for frequently used queries
private static readonly Func<AppDbContext, Guid, Task<Enterprise>> 
    GetEnterpriseById = 
        EF.CompileAsyncQuery((AppDbContext ctx, Guid id) =>
            ctx.Enterprises
                .Include(e => e.IndustrialZone)
                .FirstOrDefault(e => e.Id == id));
```

### 6.4. Async Processing

**RabbitMQ for Background Jobs:**

```csharp
// Publish message
public async Task ProcessOCRAsync(Guid fileId)
{
    var message = new OCRJobMessage
    {
        FileId = fileId,
        Priority = OCRPriority.Normal,
        CreatedAt = DateTime.UtcNow
    };
    
    await _messageBus.PublishAsync("ocr.jobs", message);
}

// Consume message
public class OCRJobConsumer : IConsumer<OCRJobMessage>
{
    public async Task ConsumeAsync(OCRJobMessage message)
    {
        // Long-running OCR processing
        var result = await _ocrService.ProcessAsync(message.FileId);
        
        // Save result
        await _dbContext.OCRResults.AddAsync(result);
        await _dbContext.SaveChangesAsync();
        
        // Send notification
        await _notificationService.NotifyAsync(
            message.UserId, 
            "OCR processing completed"
        );
    }
}
```

---

## 7. Deployment Strategy

### 7.1. Docker Compose

**docker-compose.yml:**

```yaml
version: '3.8'

services:
  # API Gateway
  api-gateway:
    build: ./src/ApiGateway
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - auth-service
      - enterprise-service

  # Auth Service
  auth-service:
    build: ./src/Services/Auth
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=AuthDB;
    depends_on:
      - sqlserver
      - redis

  # Enterprise Service
  enterprise-service:
    build: ./src/Services/Enterprise
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=EnterpriseDB;
    depends_on:
      - sqlserver
      - redis
      - elasticsearch

  # SQL Server
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password
    volumes:
      - sqlserver-data:/var/opt/mssql
    ports:
      - "1433:1433"

  # PostgreSQL with PostGIS
  postgres:
    image: postgis/postgis:16-3.4
    environment:
      - POSTGRES_DB=GIS_DB
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  # MinIO
  minio:
    image: minio/minio:latest
    command: server /data --console-address ":9001"
    environment:
      - MINIO_ROOT_USER=minioadmin
      - MINIO_ROOT_PASSWORD=minioadmin
    volumes:
      - minio-data:/data
    ports:
      - "9000:9000"
      - "9001:9001"

  # Elasticsearch
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
      - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
    volumes:
      - elasticsearch-data:/usr/share/elasticsearch/data
    ports:
      - "9200:9200"

  # Redis
  redis:
    image: redis:7-alpine
    volumes:
      - redis-data:/data
    ports:
      - "6379:6379"

  # RabbitMQ
  rabbitmq:
    image: rabbitmq:3.12-management-alpine
    environment:
      - RABBITMQ_DEFAULT_USER=guest
      - RABBITMQ_DEFAULT_PASS=guest
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"

volumes:
  sqlserver-data:
  postgres-data:
  minio-data:
  elasticsearch-data:
  redis-data:
  rabbitmq-data:
```

### 7.2. Environment Configuration

**Development:**
- Local Docker Compose
- Hot reload enabled
- Swagger UI enabled
- Debug logging

**Staging:**
- Cloud deployment
- Similar to production
- Test with production-like data

**Production:**
- Cloud deployment (Azure/AWS/On-premise)
- High availability configuration
- Automated backup
- Monitoring and alerting

---

## 8. Monitoring & Logging

### 8.1. Logging

**Serilog Configuration:**

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "axdd-logs-{0:yyyy.MM.dd}"
    })
    .CreateLogger();
```

### 8.2. Health Checks

```csharp
services.AddHealthChecks()
    .AddSqlServer(configuration["ConnectionStrings:DefaultConnection"])
    .AddRedis(configuration["Redis:ConnectionString"])
    .AddElasticsearch(configuration["Elasticsearch:Uri"])
    .AddRabbitMQ(configuration["RabbitMQ:ConnectionString"]);

app.MapHealthChecks("/health");
```

### 8.3. Audit Logging

```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
    public string ResourceId { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}
```

---

## 9. Development Guidelines

### 9.1. Coding Standards

- Follow [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use async/await for I/O operations
- Use nullable reference types
- Use record types for DTOs
- Use LINQ for collections

### 9.2. Testing

```
src/
├── Services/
│   └── Enterprise/
│       ├── Enterprise.API/
│       ├── Enterprise.Application/
│       ├── Enterprise.Domain/
│       ├── Enterprise.Infrastructure/
│       └── Enterprise.Tests/
│           ├── UnitTests/
│           ├── IntegrationTests/
│           └── E2ETests/
```

**Test Coverage Target:** ≥ 80%

### 9.3. Git Workflow

```
main (production)
  └── develop
       ├── feature/INV-001-investment-cert
       ├── feature/ENV-002-eia-management
       └── hotfix/BUG-003-login-issue
```

**Commit Message Format:**

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- feat: New feature
- fix: Bug fix
- docs: Documentation
- refactor: Code refactoring
- test: Adding tests
- chore: Maintenance

---

## Phụ Lục

### A. Thuật Ngữ Viết Tắt

| Viết Tắt | Nghĩa |
|----------|-------|
| KCN | Khu Công Nghiệp |
| GCNĐKĐT | Giấy Chứng Nhận Đăng Ký Đầu Tư |
| ĐTM | Đánh Giá Tác Động Môi Trường |
| NCKT | Nghiên Cứu Khả Thi |
| LGSP | Local Government Service Platform |
| QLVB | Quản Lý Văn Bản |
| BVMT | Bảo Vệ Môi Trường |

### B. Tài Liệu Tham Khảo

- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Microservices Architecture](https://microservices.io/)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

**Kết thúc tài liệu đặc tả kỹ thuật**
