# Document Management Module - Technical Specification
## Module Quản lý Hồ sơ Tài liệu (Doceye)

**Version:** 1.0  
**Date:** 2026-02-07  
**Author:** GitHub Copilot Agent

---

## 1. Overview

### 1.1 Purpose
Xây dựng module quản lý tài liệu số hoàn chỉnh, tương tự Google Drive/Dropbox, với các tính năng:
- Quản lý hồ sơ, tài liệu số
- Phân cấp thư mục
- Phân quyền truy cập
- Số hóa tài liệu (OCR)
- Mượn trả tài liệu

### 1.2 Scope
Module này mở rộng từ FileManager Service hiện có với các tính năng bổ sung.

### 1.3 Target Framework
- .NET 9.0
- Entity Framework Core 9.0
- MinIO Object Storage
- SQL Server 2022

---

## 2. Architecture

### 2.1 Service Architecture
```
src/Services/FileManager/
├── AXDD.Services.FileManager.Api/
│   ├── Controllers/          # API Controllers
│   ├── Data/                 # DbContext & Migrations
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Entities/             # Domain Entities
│   ├── Enums/                # Enumerations
│   ├── Services/             # Business Logic
│   │   ├── Interfaces/
│   │   └── Implementations/
│   ├── Settings/             # Configuration Classes
│   └── Exceptions/           # Custom Exceptions
```

### 2.2 Database Schema

#### Core Entities (Already Implemented)
- FileMetadata
- Folder  
- FileVersion
- FileShare
- StorageQuota
- DocumentProfile
- ProfileMetadataField
- DocumentProfileDocument
- DocumentMetadataValue

#### New Entities to Add
- DocumentType (Loại tài liệu)
- FolderType (Loại hồ sơ)
- DigitalStorage (Kho tài liệu điện tử)
- PhysicalStorage (Kho lưu trữ vật lý)
- PhysicalStorageLocation (Vị trí trong kho vật lý)
- DocumentApproval (Kiểm duyệt tài liệu)
- DocumentLoan (Mượn tài liệu)
- DocumentLoanItem (Chi tiết phiếu mượn)
- FolderPermission (Phân quyền thư mục)
- SecurityLevel (Cấp độ bảo mật)
- AuditLog (Nhật ký hệ thống)

---

## 3. Feature Specifications

### 3.1 Document Type Management (Quản lý loại tài liệu)

#### Entities
```csharp
public class DocumentType : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    // Relationships
    public ICollection<DocumentTypeMetadataField> MetadataFields { get; set; }
}

public class DocumentTypeMetadataField : BaseEntity
{
    public Guid DocumentTypeId { get; set; }
    public string FieldName { get; set; }
    public string DisplayName { get; set; }
    public string DataType { get; set; } // Text, Number, Date, List, etc.
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public int DisplayOrder { get; set; }
}
```

#### API Endpoints
- GET /api/v1/document-types - List all document types
- GET /api/v1/document-types/{id} - Get document type by ID
- POST /api/v1/document-types - Create document type
- PUT /api/v1/document-types/{id} - Update document type
- DELETE /api/v1/document-types/{id} - Delete document type
- GET /api/v1/document-types/{id}/metadata-fields - Get metadata fields

### 3.2 Folder Type Management (Quản lý loại hồ sơ)

#### Entities
```csharp
public class FolderType : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int RetentionPeriodMonths { get; set; }
    public int DisplayOrder { get; set; }
    // Relationships
    public ICollection<FolderTypeMetadataField> MetadataFields { get; set; }
}
```

#### API Endpoints
- GET /api/v1/folder-types - List all folder types
- POST /api/v1/folder-types - Create folder type
- PUT /api/v1/folder-types/{id} - Update folder type
- DELETE /api/v1/folder-types/{id} - Delete folder type

### 3.3 Digital Storage Management (Kho tài liệu điện tử)

#### Entities
```csharp
public class DigitalStorage : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string BucketName { get; set; }
    public long TotalCapacityBytes { get; set; }
    public long UsedCapacityBytes { get; set; }
    public bool IsActive { get; set; }
    public string EnterpriseCode { get; set; }
}
```

#### API Endpoints
- GET /api/v1/digital-storages - List digital storages
- POST /api/v1/digital-storages - Create digital storage
- PUT /api/v1/digital-storages/{id} - Update digital storage
- DELETE /api/v1/digital-storages/{id} - Delete digital storage

### 3.4 Physical Storage Management (Kho lưu trữ vật lý)

#### Entities
```csharp
public class PhysicalStorage : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    // Relationships
    public ICollection<PhysicalStorageLocation> Locations { get; set; }
}

public class PhysicalStorageLocation : BaseEntity
{
    public Guid PhysicalStorageId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsOccupied { get; set; }
}
```

#### API Endpoints
- GET /api/v1/physical-storages - List physical storages
- POST /api/v1/physical-storages - Create physical storage
- PUT /api/v1/physical-storages/{id} - Update physical storage
- DELETE /api/v1/physical-storages/{id} - Delete physical storage
- GET /api/v1/physical-storages/{id}/locations - List locations
- POST /api/v1/physical-storages/{id}/locations - Add location

### 3.5 Document Approval (Kiểm duyệt tài liệu)

#### Entities
```csharp
public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public class DocumentApproval : BaseEntity
{
    public Guid DocumentId { get; set; }
    public string RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
}
```

#### API Endpoints
- GET /api/v1/approvals - List approvals with filters
- GET /api/v1/approvals/{id} - Get approval detail
- POST /api/v1/approvals - Submit for approval
- PUT /api/v1/approvals/{id}/approve - Approve document
- PUT /api/v1/approvals/{id}/reject - Reject document

### 3.6 Document Loan Management (Mượn trả tài liệu)

#### Entities
```csharp
public enum LoanStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Borrowed = 3,
    Returned = 4,
    Overdue = 5
}

public enum LoanType
{
    HardCopy = 0,
    Copy = 1,
    Backup = 2,
    ReadOnly = 3,
    CertifiedCopy = 4
}

public class DocumentLoan : BaseEntity
{
    public string LoanCode { get; set; }
    public string BorrowerUserId { get; set; }
    public string BorrowerName { get; set; }
    public string? BorrowerDepartment { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public LoanStatus Status { get; set; }
    public LoanType LoanType { get; set; }
    public string? Purpose { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    // Relationships
    public ICollection<DocumentLoanItem> Items { get; set; }
}

public class DocumentLoanItem : BaseEntity
{
    public Guid DocumentLoanId { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; }
    public bool IsReturned { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string? Notes { get; set; }
}
```

#### API Endpoints
- GET /api/v1/loans - List loans with filters (pending, approved, rejected, borrowed, returned, overdue)
- GET /api/v1/loans/{id} - Get loan detail
- POST /api/v1/loans - Create loan request
- PUT /api/v1/loans/{id}/approve - Approve loan
- PUT /api/v1/loans/{id}/reject - Reject loan
- PUT /api/v1/loans/{id}/borrow - Mark as borrowed
- PUT /api/v1/loans/{id}/return - Return documents
- GET /api/v1/loans/statistics - Get loan statistics

### 3.7 Folder Permission (Phân quyền thư mục)

#### Entities
```csharp
public enum PermissionLevel
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 3,
    Admin = 4
}

public class FolderPermission : BaseEntity
{
    public Guid FolderId { get; set; }
    public string UserId { get; set; }
    public string? UserGroupId { get; set; }
    public PermissionLevel Permission { get; set; }
    public bool CanShare { get; set; }
    public bool CanDownload { get; set; }
    public bool CanPrint { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string GrantedBy { get; set; }
    public DateTime GrantedAt { get; set; }
}
```

#### API Endpoints
- GET /api/v1/folders/{folderId}/permissions - Get folder permissions
- POST /api/v1/folders/{folderId}/permissions - Grant permission
- PUT /api/v1/folders/{folderId}/permissions/{id} - Update permission
- DELETE /api/v1/folders/{folderId}/permissions/{id} - Revoke permission

### 3.8 Security Level (Cấp độ bảo mật)

#### Entities
```csharp
public class SecurityLevel : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; } // 1 = Public, 2 = Internal, 3 = Confidential, 4 = Restricted
    public bool RequiresApproval { get; set; }
    public bool IsActive { get; set; }
}
```

#### API Endpoints
- GET /api/v1/security-levels - List security levels
- POST /api/v1/security-levels - Create security level
- PUT /api/v1/security-levels/{id} - Update security level
- DELETE /api/v1/security-levels/{id} - Delete security level

### 3.9 Audit Log (Nhật ký hệ thống)

#### Entities
```csharp
public enum AuditAction
{
    Create = 0,
    Read = 1,
    Update = 2,
    Delete = 3,
    Download = 4,
    Upload = 5,
    Share = 6,
    Move = 7,
    Approve = 8,
    Reject = 9,
    Login = 10,
    Logout = 11
}

public class AuditLog : BaseEntity
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public AuditAction Action { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}
```

#### API Endpoints
- GET /api/v1/audit-logs - List audit logs with filters
- GET /api/v1/audit-logs/access - Access logs
- GET /api/v1/audit-logs/login - Login logs
- GET /api/v1/audit-logs/changes - Content change logs
- DELETE /api/v1/audit-logs - Delete logs (admin only)

---

## 4. Enhanced Folder Management

### 4.1 Update Folder Entity
Add additional fields to existing Folder entity:
```csharp
public class Folder : BaseEntity
{
    // Existing fields...
    
    // New fields
    public Guid? FolderTypeId { get; set; }
    public Guid? DigitalStorageId { get; set; }
    public Guid? SecurityLevelId { get; set; }
    public string? Password { get; set; }
    public bool IsPasswordProtected { get; set; }
    public string? QRCode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReminderDate { get; set; }
    // Relationships
    public FolderType? FolderType { get; set; }
    public DigitalStorage? DigitalStorage { get; set; }
    public SecurityLevel? SecurityLevel { get; set; }
    public ICollection<FolderPermission> Permissions { get; set; }
}
```

### 4.2 Enhanced Folder API
- POST /api/v1/folders/{id}/move - Move folder to another location
- POST /api/v1/folders/{id}/copy - Copy folder
- POST /api/v1/folders/{id}/password - Set password
- DELETE /api/v1/folders/{id}/password - Remove password
- GET /api/v1/folders/{id}/qrcode - Generate QR code
- GET /api/v1/folders/{id}/download - Download folder as ZIP

---

## 5. Enhanced File Management

### 5.1 Update FileMetadata Entity
Add additional fields:
```csharp
public class FileMetadata : BaseEntity
{
    // Existing fields...
    
    // New fields
    public Guid? DocumentTypeId { get; set; }
    public Guid? SecurityLevelId { get; set; }
    public string? Password { get; set; }
    public bool IsPasswordProtected { get; set; }
    public string? QRCode { get; set; }
    public string? OcrContent { get; set; }
    public bool IsOcrProcessed { get; set; }
    public DateTime? DocumentDate { get; set; }
    public string? DocumentNumber { get; set; }
    public Guid? PhysicalStorageLocationId { get; set; }
    // Relationships
    public DocumentType? DocumentType { get; set; }
    public SecurityLevel? SecurityLevel { get; set; }
    public PhysicalStorageLocation? PhysicalStorageLocation { get; set; }
    public ICollection<DocumentApproval> Approvals { get; set; }
}
```

### 5.2 Enhanced File API
- POST /api/v1/files/{id}/move - Move file
- POST /api/v1/files/{id}/copy - Copy file
- POST /api/v1/files/{id}/password - Set password
- DELETE /api/v1/files/{id}/password - Remove password
- GET /api/v1/files/{id}/qrcode - Generate QR code
- POST /api/v1/files/{id}/ocr - Trigger OCR processing
- GET /api/v1/files/{id}/summary - Get AI summary (placeholder)
- GET /api/v1/files/search - Advanced search with full-text

---

## 6. Statistics & Dashboard

### 6.1 Statistics Endpoints
- GET /api/v1/statistics/documents - Document statistics
  - Total digitized
  - Pending cataloging
  - Cataloged
  - Approved
  - Rejected
  - By document type
  - Storage usage

- GET /api/v1/statistics/storage - Storage statistics
  - Total folders
  - By type
  - By storage
  - User usage

- GET /api/v1/statistics/loans - Loan statistics
  - By loan type
  - Total borrowers
  - Overdue count

---

## 7. Implementation Plan

### Phase 1: Core Entities & Database (Priority: High)
1. Create new entities
2. Update existing entities
3. Create migrations
4. Seed data

### Phase 2: Services & Controllers (Priority: High)
1. Document Type Service
2. Folder Type Service
3. Digital Storage Service
4. Physical Storage Service
5. Document Approval Service
6. Document Loan Service
7. Folder Permission Service
8. Security Level Service
9. Audit Log Service
10. Statistics Service

### Phase 3: Frontend Integration (Priority: Medium)
1. Admin views for document management
2. File upload/download interface
3. Folder tree navigation
4. Search interface
5. Statistics dashboard

### Phase 4: Advanced Features (Priority: Low)
1. OCR integration placeholder
2. AI summary placeholder
3. Real-time collaboration placeholder

---

## 8. Security Considerations

1. **Authentication**: JWT-based authentication
2. **Authorization**: Role-based and permission-based access
3. **Password Protection**: Folder and file level passwords
4. **Security Levels**: Multiple security classifications
5. **Audit Logging**: Complete audit trail
6. **Data Encryption**: Sensitive data encryption at rest

---

## 9. Performance Considerations

1. **Pagination**: All list endpoints support pagination
2. **Caching**: Redis caching for frequently accessed data
3. **Indexing**: Database indexes on frequently queried fields
4. **Streaming**: Large file operations use streaming
5. **Async Operations**: All I/O operations are async

---

## 10. Dependencies

### NuGet Packages
- Minio (6.0.3) - Already installed
- QRCoder - For QR code generation
- ZipArchive - For folder download as ZIP

---

## 11. References

- PDF Requirements: /docs/requirement/yeu-cau-module-quan-ly-ho-so-tai-lieu-Doceye.pdf
- Existing FileManager Implementation
- Building Blocks Library
