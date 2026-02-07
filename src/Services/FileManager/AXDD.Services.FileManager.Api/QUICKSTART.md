# Quick Start Guide - FileManager Service

## 1. Start MinIO (Docker)

```bash
docker run -d \
  -p 9000:9000 \
  -p 9001:9001 \
  --name minio \
  -e "MINIO_ROOT_USER=minioadmin" \
  -e "MINIO_ROOT_PASSWORD=minioadmin" \
  minio/minio server /data --console-address ":9001"
```

Access MinIO Console: http://localhost:9001
- Username: minioadmin
- Password: minioadmin

## 2. Setup Database

```bash
cd src/Services/FileManager/AXDD.Services.FileManager.Api

# Apply migrations
dotnet ef database update
```

## 3. Run the Service

```bash
dotnet run
```

Service will start at: http://localhost:5000
Swagger UI: http://localhost:5000/swagger

## 4. Test the API

### Upload a File

```bash
# Create a test file
echo "Hello World" > test.txt

# Upload it
curl -X POST "http://localhost:5000/api/v1/files/upload" \
  -F "file=@test.txt" \
  -F "enterpriseCode=TEST" \
  -F "description=Test file" \
  -F "tags=test,sample"
```

Response:
```json
{
  "success": true,
  "message": "File uploaded successfully",
  "data": {
    "id": "guid-here",
    "fileName": "test.txt",
    "fileSize": 12,
    "mimeType": "text/plain",
    "extension": ".txt",
    "enterpriseCode": "TEST",
    "version": 1,
    "uploadedBy": "anonymous",
    "uploadedAt": "2024-02-06T10:30:00Z"
  }
}
```

### Download the File

```bash
# Replace {file-id} with the ID from upload response
curl -X GET "http://localhost:5000/api/v1/files/{file-id}/download" \
  --output downloaded-test.txt
```

### Get File Metadata

```bash
curl -X GET "http://localhost:5000/api/v1/files/{file-id}"
```

### List Files

```bash
curl -X GET "http://localhost:5000/api/v1/files?enterpriseCode=TEST&pageSize=10"
```

### Get Storage Quota

```bash
curl -X GET "http://localhost:5000/api/v1/quota?enterpriseCode=TEST"
```

Response:
```json
{
  "success": true,
  "data": {
    "enterpriseCode": "TEST",
    "quotaBytes": 107374182400,
    "usedBytes": 12,
    "availableBytes": 107374182388,
    "usagePercentage": 0.000000011175870895385742,
    "isExceeded": false,
    "isWarningThresholdReached": false
  }
}
```

## 5. Create Folders

```bash
# Create root folder (automatic)
curl -X GET "http://localhost:5000/api/v1/folders/root?enterpriseCode=TEST"

# Create a subfolder
curl -X POST "http://localhost:5000/api/v1/folders" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Documents",
    "enterpriseCode": "TEST",
    "description": "Document storage"
  }'
```

## 6. Version Control

```bash
# Create a new version
echo "Hello World v2" > test-v2.txt

curl -X POST "http://localhost:5000/api/v1/files/{file-id}/versions" \
  -F "file=@test-v2.txt" \
  -F "notes=Second version with updates"

# List versions
curl -X GET "http://localhost:5000/api/v1/files/{file-id}/versions"

# Restore version
curl -X POST "http://localhost:5000/api/v1/files/{file-id}/versions/1/restore"
```

## 7. Share Files

```bash
# Share a file
curl -X POST "http://localhost:5000/api/v1/shares" \
  -H "Content-Type: application/json" \
  -d '{
    "fileId": "{file-id}",
    "sharedWithUserId": "user@example.com",
    "permission": "Read",
    "expiresAt": "2024-12-31T23:59:59Z"
  }'

# Get shared files
curl -X GET "http://localhost:5000/api/v1/shares?userId=user@example.com"
```

## 8. Get Presigned URL

```bash
# Get a temporary download URL (valid for 60 minutes)
curl -X GET "http://localhost:5000/api/v1/files/{file-id}/view?expiryMinutes=60"
```

Response:
```json
{
  "success": true,
  "data": "http://localhost:9000/axdd-documents-dev/TEST/20240206/xxx_test.txt?X-Amz-Algorithm=..."
}
```

You can use this URL directly in a browser or download tool.

## 9. Delete File

```bash
curl -X DELETE "http://localhost:5000/api/v1/files/{file-id}"
```

Note: This is a soft delete. The file metadata is marked as deleted but the physical file remains in MinIO.

## Common Issues

### "No file uploaded" Error
Ensure your form field is named "file" and you're using `-F` (form data) not `-d` (JSON).

### "Storage quota exceeded"
Check quota with `/api/v1/quota` endpoint and increase if needed.

### "File type not allowed"
Check the `AllowedExtensions` in appsettings.json.

### MinIO Connection Error
Verify MinIO is running and accessible at the configured endpoint.

## Next Steps

1. Integrate with Auth Service for authentication
2. Configure production MinIO with SSL
3. Set up file retention policies
4. Configure backups for both SQL Server and MinIO
5. Implement virus scanning (when enabled)
6. Set up monitoring and alerting

## Development Tips

- Use Swagger UI at `/swagger` for interactive API testing
- Check logs for detailed error information
- Use SQL Server Management Studio to view database records
- Use MinIO Console to browse uploaded files
- Test with different file types and sizes

Enjoy using the FileManager Service! 🚀
