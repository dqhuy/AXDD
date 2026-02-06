# AXDD FileManager Service

A comprehensive file management service built with ASP.NET Core 9, MinIO object storage, and SQL Server for metadata management.

## Features

- ✅ **File Upload & Download** - Support for multiple file types with size validation
- ✅ **MinIO Integration** - Object storage with automatic bucket creation
- ✅ **File Versioning** - Track and restore previous versions of files
- ✅ **Folder Management** - Hierarchical folder structure per enterprise
- ✅ **File Sharing** - Share files with permissions and expiration dates
- ✅ **Storage Quotas** - Per-enterprise storage limits with usage tracking
- ✅ **Presigned URLs** - Secure temporary download links
- ✅ **Streaming** - Memory-efficient file operations
- ✅ **Soft Delete** - Recoverable file deletions
- ✅ **Checksum Validation** - MD5 checksums for file integrity
- ✅ **Full-text Search** - Search files by name, description, and tags

## Technology Stack

- **Framework**: .NET 9.0
- **Object Storage**: MinIO 6.0.3
- **Database**: SQL Server with Entity Framework Core 9.0
- **API Documentation**: Swagger/OpenAPI
- **Health Checks**: SQL Server and custom checks

## Prerequisites

- .NET 9.0 SDK
- SQL Server (or Azure SQL)
- MinIO Server (for development: `docker run -p 9000:9000 -p 9001:9001 minio/minio server /data --console-address ":9001"`)

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AXDD_FileManager;Integrated Security=true;TrustServerCertificate=true;"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketNames": {
      "Documents": "axdd-documents",
      "Attachments": "axdd-attachments",
      "Temp": "axdd-temp",
      "Archives": "axdd-archives"
    }
  },
  "FileUpload": {
    "MaxFileSizeBytes": 104857600,
    "AllowedExtensions": [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg"],
    "EnableVirusScanning": false,
    "PresignedUrlExpiryMinutes": 60
  },
  "StorageQuota": {
    "DefaultQuotaPerEnterpriseGB": 100,
    "WarningThresholdPercentage": 80
  }
}
```

## Database Setup

### Apply Migrations

```bash
cd src/Services/FileManager/AXDD.Services.FileManager.Api
dotnet ef database update
```

## Running the Service

### Development

```bash
cd src/Services/FileManager/AXDD.Services.FileManager.Api
dotnet run
```

The service will be available at:
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`

### Docker

```bash
docker-compose up filemanager
```

## API Endpoints

### Files

#### Upload File
```http
POST /api/v1/files/upload
Content-Type: multipart/form-data

Form fields:
- file: <file>
- enterpriseCode: string
- folderId: guid (optional)
- description: string (optional)
- tags: string (optional)
```

#### Download File
```http
GET /api/v1/files/{id}/download
```

#### Get File URL (Presigned)
```http
GET /api/v1/files/{id}/view?expiryMinutes=60
```

#### Get File Metadata
```http
GET /api/v1/files/{id}
```

#### List Files
```http
GET /api/v1/files?enterpriseCode=ACME&folderId={guid}&searchTerm=report&pageNumber=1&pageSize=10
```

#### Delete File
```http
DELETE /api/v1/files/{id}
```

### Folders

#### Create Folder
```http
POST /api/v1/folders
Content-Type: application/json

{
  "name": "Reports",
  "enterpriseCode": "ACME",
  "parentFolderId": "guid-or-null",
  "description": "Monthly reports"
}
```

#### Get Folder
```http
GET /api/v1/folders/{id}
```

#### Get Root Folder
```http
GET /api/v1/folders/root?enterpriseCode=ACME
```

#### List Folders
```http
GET /api/v1/folders?enterpriseCode=ACME&parentFolderId={guid}&pageNumber=1&pageSize=10
```

#### Delete Folder
```http
DELETE /api/v1/folders/{id}
```

### File Versions

#### Get Versions
```http
GET /api/v1/files/{fileId}/versions
```

#### Create Version
```http
POST /api/v1/files/{fileId}/versions
Content-Type: multipart/form-data

Form fields:
- file: <file>
- notes: string (optional)
```

#### Restore Version
```http
POST /api/v1/files/{fileId}/versions/{version}/restore
```

### File Shares

#### Share File
```http
POST /api/v1/shares
Content-Type: application/json

{
  "fileId": "guid",
  "sharedWithUserId": "user@example.com",
  "permission": "Read",
  "expiresAt": "2024-12-31T23:59:59Z"
}
```

#### Get Shared Files
```http
GET /api/v1/shares?userId=user@example.com
```

#### Revoke Share
```http
DELETE /api/v1/shares/{id}
```

### Storage Quota

#### Get Quota
```http
GET /api/v1/quota?enterpriseCode=ACME
```

#### Initialize Quota
```http
POST /api/v1/quota/initialize?enterpriseCode=ACME&quotaGB=100
```

## Usage Examples

### Upload a File with cURL

```bash
curl -X POST "http://localhost:5000/api/v1/files/upload" \
  -F "file=@document.pdf" \
  -F "enterpriseCode=ACME" \
  -F "description=Annual Report 2024" \
  -F "tags=report,annual,2024"
```

### Upload a File with C#

```csharp
using var client = new HttpClient();
using var form = new MultipartFormDataContent();

var fileStream = File.OpenRead("document.pdf");
form.Add(new StreamContent(fileStream), "file", "document.pdf");
form.Add(new StringContent("ACME"), "enterpriseCode");
form.Add(new StringContent("Annual Report 2024"), "description");

var response = await client.PostAsync(
    "http://localhost:5000/api/v1/files/upload",
    form);

var result = await response.Content.ReadAsStringAsync();
```

### Download a File

```bash
curl -X GET "http://localhost:5000/api/v1/files/{file-id}/download" \
  --output downloaded-file.pdf
```

### Get Presigned URL

```bash
curl -X GET "http://localhost:5000/api/v1/files/{file-id}/view?expiryMinutes=30"
```

### Create a New File Version

```bash
curl -X POST "http://localhost:5000/api/v1/files/{file-id}/versions" \
  -F "file=@document-v2.pdf" \
  -F "notes=Updated with Q4 data"
```

## Architecture

### Components

1. **Controllers** - REST API endpoints
2. **Services** - Business logic implementation
   - `IFileService` - File operations
   - `IFolderService` - Folder management
   - `IFileVersionService` - Version control
   - `IFileShareService` - Sharing functionality
   - `IStorageQuotaService` - Quota management
   - `IMinioService` - MinIO integration
3. **Entities** - Database models
   - `FileMetadata` - File information
   - `Folder` - Folder hierarchy
   - `FileVersion` - Version history
   - `FileShare` - Sharing permissions
   - `StorageQuota` - Enterprise quotas
4. **DTOs** - Data transfer objects
5. **Settings** - Configuration classes

### Data Flow

```
Client Request
    ↓
Controller (validates input)
    ↓
Service Layer (business logic)
    ↓
┌─────────────────┬──────────────────┐
│  SQL Server     │    MinIO         │
│  (Metadata)     │    (Files)       │
└─────────────────┴──────────────────┘
```

### File Storage Organization

Files are stored in MinIO with the following structure:
```
{bucketName}/
  {enterpriseCode}/
    {YYYYMMDD}/
      {uniqueId}_{sanitizedFileName}.ext
```

Example:
```
axdd-documents/
  ACME/
    20240206/
      a1b2c3d4_annual_report.pdf
```

## Security Considerations

1. **File Validation**
   - Extension whitelist
   - Size limits
   - MIME type validation

2. **Access Control**
   - Enterprise-level isolation
   - User authentication (integrate with Auth service)
   - Permission-based file sharing

3. **Data Protection**
   - Checksums for integrity
   - Soft delete for recovery
   - Presigned URLs for secure access

4. **Storage Security**
   - MinIO access keys
   - Encrypted connections (SSL)
   - Bucket policies

## Performance Optimizations

1. **Streaming** - Files are streamed, not loaded into memory
2. **Async Operations** - All I/O is asynchronous
3. **Pagination** - Large result sets are paginated
4. **Indexing** - Database indexes on frequently queried fields
5. **Presigned URLs** - Direct client-to-storage downloads

## Error Handling

The service uses custom exceptions:
- `FileNotFoundException` - HTTP 404
- `QuotaExceededException` - HTTP 507 (Insufficient Storage)
- `FileSizeTooLargeException` - HTTP 413 (Payload Too Large)
- `InvalidFileTypeException` - HTTP 400
- `FolderNotFoundException` - HTTP 404

## Health Checks

- **Endpoint**: `/health`
- **Checks**:
  - SQL Server connectivity
  - MinIO connectivity (via bucket existence check)

## Logging

The service uses structured logging with the following information:
- File operations (upload, download, delete)
- Storage quota changes
- Errors and exceptions
- Performance metrics

## Future Enhancements

- [ ] Virus scanning integration
- [ ] Image thumbnails generation
- [ ] Document preview generation
- [ ] Bulk operations (zip download, bulk delete)
- [ ] File tagging and metadata
- [ ] Advanced search with filters
- [ ] Audit log for compliance
- [ ] File encryption at rest
- [ ] CDN integration for public files
- [ ] Background job for cleanup of soft-deleted files

## Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportsOutputDir=./coverage
```

## Troubleshooting

### MinIO Connection Issues
- Verify MinIO is running: `http://localhost:9001`
- Check access keys in configuration
- Ensure buckets are created (automatic on startup)

### Database Connection Issues
- Verify SQL Server is running
- Check connection string
- Ensure migrations are applied

### File Upload Fails
- Check file size against `MaxFileSizeBytes`
- Verify file extension is in `AllowedExtensions`
- Check storage quota for enterprise

## Support

For issues and questions:
- GitHub Issues: [repository-url]
- Documentation: [wiki-url]
- Email: support@axdd.com

## License

Copyright © 2024 AXDD. All rights reserved.
