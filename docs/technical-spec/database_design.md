# Database Design
# Hệ Thống AXDD - Quản Lý CSDL KCN Đồng Nai

**Phiên bản:** 2.0  
**Ngày cập nhật:** 05/02/2025

---

## Mục Lục

1. [Tổng Quan Database](#1-tổng-quan-database)
2. [Entity Relationship Diagrams](#2-entity-relationship-diagrams)
3. [Table Schemas](#3-table-schemas)
4. [Index Strategy](#4-index-strategy)
5. [Migration Strategy](#5-migration-strategy)
6. [Data Seeding](#6-data-seeding)
7. [Backup & Recovery](#7-backup--recovery)

---

## 1. Tổng Quan Database

### 1.1. Database Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                   DATABASE ARCHITECTURE                      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  SQL SERVER CLUSTER (Primary RDBMS)                         │
│  ├── AuthDB              (Authentication & Authorization)   │
│  ├── MasterDataDB        (Master Data & Catalogs)          │
│  ├── FileMetaDB          (File Metadata)                   │
│  ├── InvestmentDB        (Investment Management)           │
│  ├── EnvironmentDB       (Environment Management)          │
│  ├── ConstructionDB      (Construction Management)         │
│  ├── LaborDB             (Labor Management)                │
│  ├── EnterpriseDB        (Enterprise Management)           │
│  ├── InspectionDB        (Inspection & Violation)          │
│  ├── ReportDB            (Reports & Analytics)             │
│  ├── NotificationDB      (Notifications)                   │
│  └── OCRDB               (OCR Processing)                  │
│                                                             │
│  POSTGRESQL + POSTGIS (Spatial Database)                    │
│  └── GIS_DB              (Geographic Information)          │
│                                                             │
│  ELASTICSEARCH (Search Engine)                             │
│  ├── enterprises_idx     (Enterprise index)                │
│  ├── projects_idx        (Project index)                   │
│  ├── documents_idx       (Document index)                  │
│  └── audit_logs_idx      (Audit log index)                 │
│                                                             │
│  MINIO (Object Storage)                                    │
│  ├── axdd-documents      (Document files)                  │
│  ├── axdd-attachments    (Attachment files)                │
│  ├── axdd-archives       (Archive files)                   │
│  └── axdd-temp           (Temporary files)                 │
│                                                             │
│  REDIS (Cache & Session)                                   │
│  ├── session:*           (User sessions)                   │
│  ├── cache:*             (API cache)                       │
│  └── rate:*              (Rate limiting)                   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 1.2. Database Distribution Strategy

**Database per Service Pattern:**
- Mỗi microservice có database riêng
- Đảm bảo loose coupling
- Tránh shared database antipattern

**Shared Database for Master Data:**
- Master data được chia sẻ qua API
- Không truy cập trực tiếp database của service khác

---

## 2. Entity Relationship Diagrams

### 2.1. Core Domain Model (Text-based ERD)

```
┌──────────────────────────────────────────────────────────────┐
│                   CORE ENTITY RELATIONSHIPS                   │
└──────────────────────────────────────────────────────────────┘

                    ┌─────────────────┐
                    │ IndustrialZone  │
                    ├─────────────────┤
                    │ PK Id           │
                    │    Name         │
                    │    Code         │
                    │    Area         │
                    │    Status       │
                    └────────┬────────┘
                             │1
                             │
                             │*
         ┌───────────────────┴───────────────────┐
         │                                       │
         ▼                                       ▼
┌─────────────────┐                    ┌─────────────────┐
│   Enterprise    │                    │    LandPlot     │
├─────────────────┤                    ├─────────────────┤
│ PK Id           │                    │ PK Id           │
│    Name         │                    │ FK IndustrialZoneId
│    TaxCode      │                    │    PlotNumber   │
│ FK IndustrialZoneId                  │    Area         │
│    Status       │                    │    Status       │
│    ...          │                    └─────────────────┘
└────────┬────────┘
         │1
         │
         │*
    ┌────┴─────┐
    │          │
    ▼          ▼
┌─────────┐ ┌─────────┐
│ Project │ │Employee │
├─────────┤ ├─────────┤
│ PK Id   │ │ PK Id   │
│ FK EnterpriseId    │ FK EnterpriseId
│   Name  │ │   Name  │
│   ...   │ │   ...   │
└────┬────┘ └─────────┘
     │1
     │
     │*
┌────┴────────────────────────────────────────┐
│                                             │
▼                                             ▼
┌──────────────────┐              ┌──────────────────┐
│InvestmentCertificate             │EnvironmentLicense│
├──────────────────┤              ├──────────────────┤
│ PK Id            │              │ PK Id            │
│ FK ProjectId     │              │ FK ProjectId     │
│    CertNumber    │              │    LicenseNumber │
│    IssuedDate    │              │    IssuedDate    │
│    ExpiryDate    │              │    ExpiryDate    │
│    Status        │              │    Status        │
└──────────────────┘              └──────────────────┘

┌─────────────────┐         ┌─────────────────┐
│ConstructionPermit│         │ LaborReport     │
├─────────────────┤         ├─────────────────┤
│ PK Id           │         │ PK Id           │
│ FK ProjectId    │         │ FK EnterpriseId │
│    PermitNumber │         │    ReportType   │
│    IssuedDate   │         │    ReportPeriod │
│    Status       │         │    SubmittedDate│
└─────────────────┘         └─────────────────┘

┌─────────────────┐
│   Inspection    │
├─────────────────┤
│ PK Id           │
│ FK EnterpriseId │
│    InspectionType
│    InspectionDate
│    Result       │
└────────┬────────┘
         │1
         │
         │*
         ▼
┌─────────────────┐
│   Violation     │
├─────────────────┤
│ PK Id           │
│ FK InspectionId │
│    ViolationType│
│    PenaltyAmount│
│    Status       │
└─────────────────┘
```

### 2.2. Authentication & Authorization Model

```
┌──────────────┐         ┌──────────────┐
│     User     │────*────│   UserRole   │────*────┐
├──────────────┤         ├──────────────┤         │
│ PK Id        │         │ FK UserId    │         │
│    Username  │         │ FK RoleId    │         │
│    Email     │         └──────────────┘         │
│    PasswordHash                                 │
└──────────────┘                                  │1
                                                  │
                                                  ▼
                                         ┌──────────────┐
                                         │     Role     │
                                         ├──────────────┤
                                         │ PK Id        │
                                         │    Name      │
                                         │    Description
                                         └──────┬───────┘
                                                │1
                                                │
                                                │*
                                         ┌──────┴───────┐
                                         │RolePermission│
                                         ├──────────────┤
                                         │ FK RoleId    │
                                         │ FK PermissionId
                                         └──────┬───────┘
                                                │*
                                                │
                                                ▼1
                                         ┌──────────────┐
                                         │  Permission  │
                                         ├──────────────┤
                                         │ PK Id        │
                                         │    Name      │
                                         │    Resource  │
                                         │    Action    │
                                         └──────────────┘
```

### 2.3. File Management Model

```
┌──────────────┐
│    Folder    │
├──────────────┤
│ PK Id        │
│ FK ParentId  │────┐ (Self-referencing)
│    Name      │    │
│ FK OwnerId   │    │
└──────┬───────┘    │
       │1           │
       │            │
       │*           │
┌──────┴───────┐    │
│     File     │    │
├──────────────┤    │
│ PK Id        │◄───┘
│ FK FolderId  │
│ FK OwnerId   │
│    Name      │
│    Size      │
│    StoragePath
│    Version   │
└──────┬───────┘
       │1
       │
       │*
┌──────┴───────┐
│  FileVersion │
├──────────────┤
│ PK Id        │
│ FK FileId    │
│    VersionNumber
│    StoragePath
│    CreatedAt │
└──────────────┘

┌──────────────┐
│  FileShare   │
├──────────────┤
│ PK Id        │
│ FK FileId    │
│ FK SharedWithUserId
│ FK SharedBy  │
│    Permission│
│    ExpiresAt │
└──────────────┘
```

---

## 3. Table Schemas

### 3.1. AuthDB - Authentication Database

#### Users Table

```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(100) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    FullName NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    IsActive BIT DEFAULT 1,
    IsEmailConfirmed BIT DEFAULT 0,
    LastLoginAt DATETIME2,
    FailedLoginAttempts INT DEFAULT 0,
    LockoutEnd DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    
    INDEX IX_Users_Username (Username),
    INDEX IX_Users_Email (Email),
    INDEX IX_Users_IsActive (IsActive),
    
    CONSTRAINT CHK_Users_Email CHECK (Email LIKE '%@%'),
    CONSTRAINT CHK_Users_PhoneNumber CHECK (PhoneNumber LIKE '[0-9]%')
);
```

#### Roles Table

```sql
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) UNIQUE NOT NULL,
    NormalizedName NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    IsSystem BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    
    INDEX IX_Roles_Name (Name),
    INDEX IX_Roles_NormalizedName (NormalizedName)
);
```

#### Permissions Table

```sql
CREATE TABLE Permissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Resource NVARCHAR(100) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    INDEX IX_Permissions_Resource (Resource),
    INDEX IX_Permissions_Name (Name),
    
    CONSTRAINT UQ_Permissions_Resource_Action UNIQUE (Resource, Action)
);
```

#### UserRoles Table

```sql
CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    AssignedAt DATETIME2 DEFAULT GETUTCDATE(),
    AssignedBy UNIQUEIDENTIFIER,
    
    PRIMARY KEY (UserId, RoleId),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    
    INDEX IX_UserRoles_UserId (UserId),
    INDEX IX_UserRoles_RoleId (RoleId)
);
```

#### RolePermissions Table

```sql
CREATE TABLE RolePermissions (
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    
    PRIMARY KEY (RoleId, PermissionId),
    
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
);
```

#### RefreshTokens Table

```sql
CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedByIp NVARCHAR(50),
    RevokedAt DATETIME2,
    RevokedByIp NVARCHAR(50),
    ReplacedByToken NVARCHAR(500),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    INDEX IX_RefreshTokens_Token (Token),
    INDEX IX_RefreshTokens_UserId (UserId),
    INDEX IX_RefreshTokens_ExpiresAt (ExpiresAt)
);
```

### 3.2. MasterDataDB - Master Data Database

#### Provinces Table

```sql
CREATE TABLE Provinces (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(10) UNIQUE NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    NameEn NVARCHAR(255),
    DisplayOrder INT,
    IsActive BIT DEFAULT 1,
    
    INDEX IX_Provinces_Code (Code)
);
```

#### Districts Table

```sql
CREATE TABLE Districts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProvinceId UNIQUEIDENTIFIER NOT NULL,
    Code NVARCHAR(10) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    NameEn NVARCHAR(255),
    DisplayOrder INT,
    IsActive BIT DEFAULT 1,
    
    FOREIGN KEY (ProvinceId) REFERENCES Provinces(Id),
    
    INDEX IX_Districts_ProvinceId (ProvinceId),
    INDEX IX_Districts_Code (Code),
    
    CONSTRAINT UQ_Districts_ProvinceCode UNIQUE (ProvinceId, Code)
);
```

#### Wards Table

```sql
CREATE TABLE Wards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DistrictId UNIQUEIDENTIFIER NOT NULL,
    Code NVARCHAR(10) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    NameEn NVARCHAR(255),
    DisplayOrder INT,
    IsActive BIT DEFAULT 1,
    
    FOREIGN KEY (DistrictId) REFERENCES Districts(Id),
    
    INDEX IX_Wards_DistrictId (DistrictId),
    INDEX IX_Wards_Code (Code),
    
    CONSTRAINT UQ_Wards_DistrictCode UNIQUE (DistrictId, Code)
);
```

#### IndustrialZones Table

```sql
CREATE TABLE IndustrialZones (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(50) UNIQUE NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    NameEn NVARCHAR(255),
    Type NVARCHAR(50), -- KCN, KKT, CCN
    TotalArea DECIMAL(15,2), -- hectares
    OccupiedArea DECIMAL(15,2),
    AvailableArea DECIMAL(15,2),
    Status NVARCHAR(50), -- Planning, UnderConstruction, Operating, Completed
    EstablishedDate DATE,
    ManagementUnit NVARCHAR(255),
    Address NVARCHAR(500),
    ProvinceId UNIQUEIDENTIFIER,
    DistrictId UNIQUEIDENTIFIER,
    Description NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    
    FOREIGN KEY (ProvinceId) REFERENCES Provinces(Id),
    FOREIGN KEY (DistrictId) REFERENCES Districts(Id),
    
    INDEX IX_IndustrialZones_Code (Code),
    INDEX IX_IndustrialZones_Status (Status)
);
```

#### Industries Table (VSIC 2018)

```sql
CREATE TABLE Industries (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(10) UNIQUE NOT NULL,
    Name NVARCHAR(500) NOT NULL,
    NameEn NVARCHAR(500),
    ParentId UNIQUEIDENTIFIER,
    Level INT, -- 1: Section, 2: Division, 3: Group, 4: Class
    Description NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    
    FOREIGN KEY (ParentId) REFERENCES Industries(Id),
    
    INDEX IX_Industries_Code (Code),
    INDEX IX_Industries_ParentId (ParentId),
    INDEX IX_Industries_Level (Level)
);
```

#### EnterpriseTypes Table

```sql
CREATE TABLE EnterpriseTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(20) UNIQUE NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500),
    DisplayOrder INT,
    IsActive BIT DEFAULT 1
);

-- Sample data: TNHH, Cổ phần, TNHH MTV, Tư nhân, Hợp danh, etc.
```

### 3.3. EnterpriseDB - Enterprise Database

#### Enterprises Table

```sql
CREATE TABLE Enterprises (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(50) UNIQUE,
    Name NVARCHAR(500) NOT NULL,
    NameEn NVARCHAR(500),
    ShortName NVARCHAR(255),
    TaxCode NVARCHAR(20) UNIQUE NOT NULL,
    EnterpriseTypeId UNIQUEIDENTIFIER,
    
    -- Location
    IndustrialZoneId UNIQUEIDENTIFIER NOT NULL,
    Address NVARCHAR(500),
    ProvinceId UNIQUEIDENTIFIER,
    DistrictId UNIQUEIDENTIFIER,
    WardId UNIQUEIDENTIFIER,
    
    -- Industry
    IndustryCodeMain NVARCHAR(10), -- Main industry code
    IndustryCodeSub NVARCHAR(1000), -- Comma-separated sub industry codes
    
    -- Contact
    Phone NVARCHAR(20),
    Fax NVARCHAR(20),
    Email NVARCHAR(255),
    Website NVARCHAR(255),
    
    -- Registration
    RegistrationNumber NVARCHAR(50),
    RegistrationDate DATE,
    RegistrationPlace NVARCHAR(255),
    RegisteredCapital DECIMAL(18,2), -- VND
    CharterCapital DECIMAL(18,2), -- VND
    
    -- Status
    Status NVARCHAR(50), -- Active, Suspended, Dissolved, Merged, Split
    StatusDate DATE,
    StatusReason NVARCHAR(500),
    
    -- Employment
    TotalEmployees INT,
    ForeignEmployees INT,
    
    -- Contact Person
    ContactPersonName NVARCHAR(255),
    ContactPersonPosition NVARCHAR(255),
    ContactPersonPhone NVARCHAR(20),
    ContactPersonEmail NVARCHAR(255),
    
    -- Legal Representative
    LegalRepName NVARCHAR(255),
    LegalRepPosition NVARCHAR(255),
    LegalRepPhone NVARCHAR(20),
    LegalRepEmail NVARCHAR(255),
    
    -- Metadata
    Notes NVARCHAR(MAX),
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    
    INDEX IX_Enterprises_TaxCode (TaxCode),
    INDEX IX_Enterprises_IndustrialZoneId (IndustrialZoneId),
    INDEX IX_Enterprises_Status (Status),
    INDEX IX_Enterprises_Name (Name),
    INDEX IX_Enterprises_IsDeleted (IsDeleted),
    
    -- Composite index for common queries
    INDEX IX_Enterprises_Zone_Status (IndustrialZoneId, Status) INCLUDE (Name, TaxCode),
    
    CONSTRAINT CHK_Enterprises_TaxCode CHECK (
        LEN(TaxCode) IN (10, 13) AND TaxCode LIKE '[0-9]%'
    ),
    CONSTRAINT CHK_Enterprises_RegisteredCapital CHECK (RegisteredCapital >= 0)
);
```

#### EnterpriseRewards Table

```sql
CREATE TABLE EnterpriseRewards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EnterpriseId UNIQUEIDENTIFIER NOT NULL,
    RewardType NVARCHAR(100), -- Bằng khen, Giấy khen, Cờ thi đua, etc.
    RewardLevel NVARCHAR(50), -- Tỉnh, Bộ, Nhà nước
    Title NVARCHAR(500),
    IssuedBy NVARCHAR(255),
    IssuedDate DATE,
    DecisionNumber NVARCHAR(100),
    Reason NVARCHAR(1000),
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (EnterpriseId) REFERENCES Enterprises(Id) ON DELETE CASCADE,
    
    INDEX IX_EnterpriseRewards_EnterpriseId (EnterpriseId),
    INDEX IX_EnterpriseRewards_IssuedDate (IssuedDate)
);
```

### 3.4. InvestmentDB - Investment Database

#### Projects Table

```sql
CREATE TABLE Projects (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(50) UNIQUE,
    Name NVARCHAR(500) NOT NULL,
    NameEn NVARCHAR(500),
    EnterpriseId UNIQUEIDENTIFIER NOT NULL,
    IndustrialZoneId UNIQUEIDENTIFIER,
    
    -- Investment
    TotalInvestment DECIMAL(18,2), -- VND
    RegisteredCapital DECIMAL(18,2), -- VND
    ForeignCapital DECIMAL(18,2), -- VND
    DomesticCapital DECIMAL(18,2), -- VND
    
    -- Timeline
    ImplementationPeriod INT, -- months
    StartDate DATE,
    ExpectedCompletionDate DATE,
    ActualCompletionDate DATE,
    
    -- Industry & Products
    IndustryCode NVARCHAR(10),
    MainProducts NVARCHAR(1000),
    Capacity NVARCHAR(500),
    
    -- Land & Construction
    LandArea DECIMAL(15,2), -- m2
    ConstructionArea DECIMAL(15,2), -- m2
    
    -- Status
    Status NVARCHAR(50), -- Planning, Approved, UnderConstruction, Operational, Completed, Suspended, Cancelled
    StatusDate DATE,
    
    -- Employment
    PlannedEmployees INT,
    ActualEmployees INT,
    
    Notes NVARCHAR(MAX),
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    
    INDEX IX_Projects_EnterpriseId (EnterpriseId),
    INDEX IX_Projects_Status (Status),
    INDEX IX_Projects_Code (Code)
);
```

#### InvestmentCertificates Table

```sql
CREATE TABLE InvestmentCertificates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CertificateNumber NVARCHAR(100) UNIQUE NOT NULL,
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    EnterpriseId UNIQUEIDENTIFIER NOT NULL,
    
    -- Certificate Type
    CertificateType NVARCHAR(50), -- New, Adjustment, Extension, Transfer
    ParentCertificateId UNIQUEIDENTIFIER, -- For adjustments/extensions
    
    -- Investment Details
    TotalInvestment DECIMAL(18,2),
    ImplementationPeriod INT, -- months
    
    -- Dates
    IssuedDate DATE NOT NULL,
    ExpiryDate DATE,
    EffectiveDate DATE,
    
    -- Issuer
    IssuedBy NVARCHAR(255),
    IssuingAuthority NVARCHAR(255),
    
    -- Status
    Status NVARCHAR(50), -- Active, Expired, Revoked, Superseded
    StatusDate DATE,
    RevokedReason NVARCHAR(500),
    
    -- Documents
    DecisionNumber NVARCHAR(100),
    DecisionDate DATE,
    
    Notes NVARCHAR(MAX),
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
    FOREIGN KEY (ParentCertificateId) REFERENCES InvestmentCertificates(Id),
    
    INDEX IX_InvestmentCertificates_CertificateNumber (CertificateNumber),
    INDEX IX_InvestmentCertificates_ProjectId (ProjectId),
    INDEX IX_InvestmentCertificates_Status (Status),
    INDEX IX_InvestmentCertificates_ExpiryDate (ExpiryDate)
);
```

#### CertificateAdjustments Table

```sql
CREATE TABLE CertificateAdjustments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CertificateId UNIQUEIDENTIFIER NOT NULL,
    AdjustmentType NVARCHAR(50), -- CapitalIncrease, PeriodExtension, ScopeChange, etc.
    AdjustmentDate DATE,
    
    OldValue NVARCHAR(MAX), -- JSON
    NewValue NVARCHAR(MAX), -- JSON
    
    Reason NVARCHAR(1000),
    ApprovalNumber NVARCHAR(100),
    ApprovalDate DATE,
    ApprovedBy NVARCHAR(255),
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    
    FOREIGN KEY (CertificateId) REFERENCES InvestmentCertificates(Id) ON DELETE CASCADE,
    
    INDEX IX_CertificateAdjustments_CertificateId (CertificateId)
);
```

### 3.5. FileMetaDB - File Metadata Database

#### Files Table

```sql
CREATE TABLE Files (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    OriginalName NVARCHAR(255) NOT NULL,
    Extension NVARCHAR(20),
    Size BIGINT NOT NULL,
    MimeType NVARCHAR(100),
    
    -- Storage
    StoragePath NVARCHAR(500) NOT NULL,
    StorageBucket NVARCHAR(100),
    
    -- Folder
    FolderId UNIQUEIDENTIFIER,
    Path NVARCHAR(1000), -- Full path for quick lookup
    
    -- Version
    Version INT DEFAULT 1,
    ParentFileId UNIQUEIDENTIFIER, -- For versioning
    
    -- Metadata
    Hash NVARCHAR(100), -- MD5/SHA256 hash
    Tags NVARCHAR(1000), -- Comma-separated tags
    Description NVARCHAR(MAX),
    
    -- Status
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2,
    
    -- Owner
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    
    -- Timestamps
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    LastAccessedAt DATETIME2,
    
    INDEX IX_Files_FolderId (FolderId),
    INDEX IX_Files_OwnerId (OwnerId),
    INDEX IX_Files_Name (Name),
    INDEX IX_Files_IsDeleted (IsDeleted),
    INDEX IX_Files_Hash (Hash),
    
    CONSTRAINT CHK_Files_Size CHECK (Size >= 0)
);
```

#### Folders Table

```sql
CREATE TABLE Folders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL,
    ParentId UNIQUEIDENTIFIER, -- Self-referencing
    Path NVARCHAR(1000), -- Full path: /folder1/folder2/folder3
    Level INT DEFAULT 0, -- Depth level
    
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    
    FOREIGN KEY (ParentId) REFERENCES Folders(Id),
    
    INDEX IX_Folders_ParentId (ParentId),
    INDEX IX_Folders_OwnerId (OwnerId),
    INDEX IX_Folders_Path (Path),
    
    CONSTRAINT UQ_Folders_ParentName UNIQUE (ParentId, Name)
);
```

#### FileShares Table

```sql
CREATE TABLE FileShares (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FileId UNIQUEIDENTIFIER,
    FolderId UNIQUEIDENTIFIER,
    
    SharedWithUserId UNIQUEIDENTIFIER,
    SharedWithRoleId UNIQUEIDENTIFIER,
    
    Permission NVARCHAR(20) NOT NULL, -- Read, Write, Delete, Owner
    
    SharedBy UNIQUEIDENTIFIER NOT NULL,
    SharedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    ExpiresAt DATETIME2,
    IsActive BIT DEFAULT 1,
    
    INDEX IX_FileShares_FileId (FileId),
    INDEX IX_FileShares_FolderId (FolderId),
    INDEX IX_FileShares_SharedWithUserId (SharedWithUserId),
    
    CONSTRAINT CHK_FileShares_FileOrFolder CHECK (
        (FileId IS NOT NULL AND FolderId IS NULL) OR
        (FileId IS NULL AND FolderId IS NOT NULL)
    )
);
```

### 3.6. GIS_DB - PostgreSQL with PostGIS

#### IndustrialZones (Spatial)

```sql
CREATE TABLE industrial_zones (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    area DECIMAL(15,2), -- hectares
    boundary GEOMETRY(POLYGON, 4326),
    centroid GEOMETRY(POINT, 4326),
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP,
    
    CONSTRAINT chk_area_positive CHECK (area > 0)
);

CREATE INDEX idx_industrial_zones_boundary 
    ON industrial_zones USING GIST(boundary);
    
CREATE INDEX idx_industrial_zones_centroid 
    ON industrial_zones USING GIST(centroid);
```

#### EnterpriseLocations (Spatial)

```sql
CREATE TABLE enterprise_locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    enterprise_id UUID NOT NULL,
    industrial_zone_id UUID,
    location GEOMETRY(POINT, 4326) NOT NULL,
    address TEXT,
    plot_number VARCHAR(50),
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP,
    
    FOREIGN KEY (industrial_zone_id) 
        REFERENCES industrial_zones(id)
);

CREATE INDEX idx_enterprise_locations_location 
    ON enterprise_locations USING GIST(location);
    
CREATE INDEX idx_enterprise_locations_enterprise_id 
    ON enterprise_locations(enterprise_id);
```

#### LandPlots (Spatial)

```sql
CREATE TABLE land_plots (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    plot_number VARCHAR(50),
    industrial_zone_id UUID NOT NULL,
    area DECIMAL(15,2), -- m2
    geometry GEOMETRY(POLYGON, 4326),
    status VARCHAR(50), -- Available, Occupied, Reserved
    enterprise_id UUID,
    lease_start_date DATE,
    lease_end_date DATE,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP,
    
    FOREIGN KEY (industrial_zone_id) 
        REFERENCES industrial_zones(id)
);

CREATE INDEX idx_land_plots_geometry 
    ON land_plots USING GIST(geometry);
    
CREATE INDEX idx_land_plots_zone 
    ON land_plots(industrial_zone_id);
    
CREATE INDEX idx_land_plots_status 
    ON land_plots(status);
```

---

## 4. Index Strategy

### 4.1. Index Types

#### Single-Column Indexes

```sql
-- Primary key (clustered index)
CREATE TABLE Enterprises (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID()
);

-- Unique constraint (non-clustered unique index)
CREATE TABLE Enterprises (
    TaxCode NVARCHAR(20) UNIQUE NOT NULL
);

-- Non-clustered index
CREATE INDEX IX_Enterprises_Status ON Enterprises(Status);
```

#### Composite Indexes

```sql
-- For queries: WHERE IndustrialZoneId = @id AND Status = @status
CREATE INDEX IX_Enterprises_Zone_Status 
    ON Enterprises (IndustrialZoneId, Status);

-- With INCLUDE for covering index
CREATE INDEX IX_Enterprises_Zone_Status_Covering
    ON Enterprises (IndustrialZoneId, Status)
    INCLUDE (Name, TaxCode, CreatedAt);
```

#### Filtered Indexes

```sql
-- Index only active records
CREATE INDEX IX_Enterprises_Active
    ON Enterprises (Id, Name)
    WHERE Status = 'Active' AND IsDeleted = 0;

-- Index only recently created records
CREATE INDEX IX_Enterprises_Recent
    ON Enterprises (CreatedAt, Id)
    WHERE CreatedAt >= DATEADD(MONTH, -6, GETUTCDATE());
```

#### Full-Text Indexes

```sql
-- Full-text catalog
CREATE FULLTEXT CATALOG EnterpriseCatalog AS DEFAULT;

-- Full-text index
CREATE FULLTEXT INDEX ON Enterprises(Name, Address)
    KEY INDEX PK_Enterprises
    ON EnterpriseCatalog;

-- Full-text query
SELECT * FROM Enterprises
WHERE CONTAINS(Name, 'công ty NEAR ABC');
```

### 4.2. Index Maintenance

```sql
-- Check index fragmentation
SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent,
    ips.page_count
FROM sys.dm_db_index_physical_stats(
    DB_ID(), NULL, NULL, NULL, 'LIMITED'
) ips
INNER JOIN sys.indexes i 
    ON ips.object_id = i.object_id 
    AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
    AND ips.page_count > 1000
ORDER BY ips.avg_fragmentation_in_percent DESC;

-- Rebuild index
ALTER INDEX IX_Enterprises_Zone_Status 
    ON Enterprises REBUILD;

-- Reorganize index
ALTER INDEX IX_Enterprises_Zone_Status 
    ON Enterprises REORGANIZE;

-- Update statistics
UPDATE STATISTICS Enterprises;
```

---

## 5. Migration Strategy

### 5.1. Entity Framework Core Migrations

**Add Migration:**

```bash
dotnet ef migrations add InitialCreate --project src/Services/Enterprise
dotnet ef migrations add AddEnterpriseRewards --project src/Services/Enterprise
```

**Update Database:**

```bash
dotnet ef database update --project src/Services/Enterprise
dotnet ef database update --project src/Services/Enterprise --connection "Server=..."
```

**Generate SQL Script:**

```bash
dotnet ef migrations script --project src/Services/Enterprise --output migration.sql
```

### 5.2. Migration Best Practices

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create tables
        migrationBuilder.CreateTable(
            name: "Enterprises",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                Name = table.Column<string>(maxLength: 500, nullable: false),
                TaxCode = table.Column<string>(maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Enterprises", x => x.Id);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_Enterprises_TaxCode",
            table: "Enterprises",
            column: "TaxCode",
            unique: true);

        // Seed data
        migrationBuilder.InsertData(
            table: "EnterpriseTypes",
            columns: new[] { "Id", "Code", "Name" },
            values: new object[,]
            {
                { Guid.NewGuid(), "TNHH", "Trách nhiệm hữu hạn" },
                { Guid.NewGuid(), "CP", "Cổ phần" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Enterprises");
    }
}
```

### 5.3. Zero-Downtime Migrations

**Step 1: Add new column (nullable)**

```csharp
migrationBuilder.AddColumn<string>(
    name: "NewColumn",
    table: "Enterprises",
    nullable: true);
```

**Step 2: Deploy application (can read/write both columns)**

**Step 3: Migrate data**

```sql
UPDATE Enterprises 
SET NewColumn = OldColumn 
WHERE NewColumn IS NULL;
```

**Step 4: Make column non-nullable**

```csharp
migrationBuilder.AlterColumn<string>(
    name: "NewColumn",
    table: "Enterprises",
    nullable: false);
```

**Step 5: Drop old column**

```csharp
migrationBuilder.DropColumn(
    name: "OldColumn",
    table: "Enterprises");
```

---

## 6. Data Seeding

### 6.1. Seed Master Data

```csharp
public class MasterDataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Provinces
        var dongNai = new Province
        {
            Id = Guid.Parse("..."),
            Code = "75",
            Name = "Đồng Nai"
        };
        
        modelBuilder.Entity<Province>().HasData(dongNai);
        
        // Industrial Zones
        modelBuilder.Entity<IndustrialZone>().HasData(
            new IndustrialZone
            {
                Id = Guid.NewGuid(),
                Code = "BH1",
                Name = "KCN Biên Hòa 1",
                TotalArea = 250.5m
            },
            new IndustrialZone
            {
                Id = Guid.NewGuid(),
                Code = "BH2",
                Name = "KCN Biên Hòa 2",
                TotalArea = 315.8m
            }
        );
        
        // Enterprise Types
        modelBuilder.Entity<EnterpriseType>().HasData(
            new EnterpriseType { Id = Guid.NewGuid(), Code = "TNHH", Name = "TNHH" },
            new EnterpriseType { Id = Guid.NewGuid(), Code = "CP", Name = "Cổ phần" }
        );
    }
}
```

### 6.2. Seed Default Roles & Permissions

```csharp
public class SecuritySeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Roles
        var superAdminRole = new Role
        {
            Id = Guid.Parse("..."),
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN"
        };
        
        var adminRole = new Role
        {
            Id = Guid.Parse("..."),
            Name = "Admin",
            NormalizedName = "ADMIN"
        };
        
        modelBuilder.Entity<Role>().HasData(superAdminRole, adminRole);
        
        // Permissions
        var permissions = new[]
        {
            new Permission { Id = Guid.NewGuid(), Resource = "Enterprise", Action = "Read" },
            new Permission { Id = Guid.NewGuid(), Resource = "Enterprise", Action = "Create" },
            new Permission { Id = Guid.NewGuid(), Resource = "Enterprise", Action = "Update" },
            new Permission { Id = Guid.NewGuid(), Resource = "Enterprise", Action = "Delete" }
        };
        
        modelBuilder.Entity<Permission>().HasData(permissions);
    }
}
```

---

## 7. Backup & Recovery

### 7.1. SQL Server Backup Strategy

**Full Backup (Weekly):**

```sql
BACKUP DATABASE EnterpriseDB
TO DISK = '/backup/EnterpriseDB_Full_20240115.bak'
WITH FORMAT, MEDIANAME = 'EnterpriseDB_Backup',
     NAME = 'Full Backup of EnterpriseDB';
```

**Differential Backup (Daily):**

```sql
BACKUP DATABASE EnterpriseDB
TO DISK = '/backup/EnterpriseDB_Diff_20240115.bak'
WITH DIFFERENTIAL, FORMAT,
     NAME = 'Differential Backup of EnterpriseDB';
```

**Transaction Log Backup (Hourly):**

```sql
BACKUP LOG EnterpriseDB
TO DISK = '/backup/EnterpriseDB_Log_202401151400.trn'
WITH FORMAT, NAME = 'Log Backup of EnterpriseDB';
```

### 7.2. PostgreSQL Backup

```bash
# Full backup
pg_dump -h localhost -U postgres -d GIS_DB > gis_backup_20240115.sql

# Compressed backup
pg_dump -h localhost -U postgres -Fc -d GIS_DB > gis_backup_20240115.dump

# Restore
pg_restore -h localhost -U postgres -d GIS_DB gis_backup_20240115.dump
```

### 7.3. Automated Backup Script

```bash
#!/bin/bash
# backup.sh

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backup"

# SQL Server
sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q "BACKUP DATABASE EnterpriseDB TO DISK = '$BACKUP_DIR/EnterpriseDB_$DATE.bak'"

# PostgreSQL
pg_dump -h localhost -U postgres -Fc -d GIS_DB > $BACKUP_DIR/GIS_DB_$DATE.dump

# MinIO
mc mirror minio/axdd-documents $BACKUP_DIR/minio/axdd-documents_$DATE

# Cleanup old backups (keep last 30 days)
find $BACKUP_DIR -name "*.bak" -mtime +30 -delete
find $BACKUP_DIR -name "*.dump" -mtime +30 -delete

# Upload to cloud storage
aws s3 sync $BACKUP_DIR s3://axdd-backups/
```

### 7.4. Disaster Recovery Plan

**RTO (Recovery Time Objective):** 4 hours  
**RPO (Recovery Point Objective):** 1 hour

**Recovery Steps:**

1. **Restore SQL Server databases** from latest full + differential + log backups
2. **Restore PostgreSQL database** from latest dump
3. **Restore MinIO data** from backup
4. **Rebuild Elasticsearch indexes** from database
5. **Verify data integrity**
6. **Test application functionality**

---

## Appendix A: Database Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Table | PascalCase, Plural | `Enterprises`, `InvestmentCertificates` |
| Column | PascalCase | `Name`, `TaxCode`, `CreatedAt` |
| Primary Key | `PK_{TableName}` | `PK_Enterprises` |
| Foreign Key | `FK_{TableName}_{ReferencedTable}` | `FK_Enterprises_IndustrialZones` |
| Index | `IX_{TableName}_{Columns}` | `IX_Enterprises_TaxCode` |
| Unique Constraint | `UQ_{TableName}_{Columns}` | `UQ_Enterprises_TaxCode` |
| Check Constraint | `CHK_{TableName}_{Column}` | `CHK_Enterprises_RegisteredCapital` |

---

## Appendix B: SQL Server Performance Tuning

```sql
-- Enable query store
ALTER DATABASE EnterpriseDB SET QUERY_STORE = ON;

-- Update statistics
EXEC sp_updatestats;

-- Check execution plans
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- Monitor expensive queries
SELECT TOP 10
    qs.execution_count,
    qs.total_worker_time / qs.execution_count AS avg_cpu_time,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2)+1) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY qs.total_worker_time DESC;
```

---

**Kết thúc tài liệu Database Design**
