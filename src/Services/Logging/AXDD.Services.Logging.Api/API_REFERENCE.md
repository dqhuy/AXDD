# AXDD Logging Service - API Reference

## Base URL
```
https://localhost:5001/api/v1/logs
```

---

## Audit Logs API

### List Audit Logs
```http
GET /audit?pageNumber=1&pageSize=50&level=Error&serviceName=Enterprise.Api
```

**Query Parameters:**
- `pageNumber` (int, optional) - Page number, default: 1
- `pageSize` (int, optional) - Page size, default: 50
- `dateFrom` (datetime, optional) - Start date filter
- `dateTo` (datetime, optional) - End date filter
- `level` (enum, optional) - Log level: Trace, Debug, Info, Warning, Error, Critical
- `serviceName` (string, optional) - Filter by service name
- `userId` (guid, optional) - Filter by user ID
- `correlationId` (string, optional) - Filter by correlation ID
- `searchTerm` (string, optional) - Full-text search in messages
- `sortBy` (string, optional) - Sort field, default: "Timestamp"
- `sortDirection` (string, optional) - Sort direction: "asc" or "desc", default: "desc"

**Response:** 200 OK
```json
{
  "items": [
    {
      "id": "guid",
      "timestamp": "2024-01-15T10:30:00Z",
      "level": "Info",
      "userId": "guid",
      "username": "admin@axdd.com",
      "userRole": "Administrator",
      "serviceName": "Enterprise.Api",
      "actionName": "CreateEnterprise",
      "entityType": "Enterprise",
      "entityId": "guid",
      "httpMethod": "POST",
      "requestPath": "/api/v1/enterprises",
      "ipAddress": "192.168.1.100",
      "statusCode": 201,
      "durationMs": 245,
      "message": "Enterprise created successfully",
      "correlationId": "abc-123-def"
    }
  ],
  "totalCount": 1234,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 25,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Get Audit Log by ID
```http
GET /audit/{id}
```

**Response:** 200 OK, 404 Not Found

### Create Audit Log
```http
POST /audit
Content-Type: application/json
```

**Request Body:**
```json
{
  "level": "Info",
  "userId": "guid",
  "username": "admin@axdd.com",
  "userRole": "Administrator",
  "serviceName": "MyService",
  "actionName": "MyAction",
  "entityType": "MyEntity",
  "entityId": "guid",
  "httpMethod": "POST",
  "requestPath": "/api/v1/resource",
  "ipAddress": "192.168.1.100",
  "userAgent": "Mozilla/5.0...",
  "requestBody": "{...}",
  "responseBody": "{...}",
  "statusCode": 201,
  "durationMs": 150,
  "message": "Action completed successfully",
  "exceptionMessage": null,
  "stackTrace": null,
  "correlationId": "abc-123-def",
  "additionalData": "{...}"
}
```

**Response:** 201 Created

### Get Logs by User
```http
GET /audit/user/{userId}?pageNumber=1&pageSize=50
```

**Response:** 200 OK

### Get Logs by Service
```http
GET /audit/service/{serviceName}?pageNumber=1&pageSize=50
```

**Response:** 200 OK

### Trace by Correlation ID
```http
GET /audit/trace/{correlationId}
```

Returns all logs with the same correlation ID, useful for tracing requests across services.

**Response:** 200 OK
```json
[
  {
    "id": "guid",
    "timestamp": "2024-01-15T10:30:00Z",
    "serviceName": "ApiGateway",
    "message": "Request received",
    "correlationId": "abc-123-def"
  },
  {
    "id": "guid",
    "timestamp": "2024-01-15T10:30:01Z",
    "serviceName": "Enterprise.Api",
    "message": "Processing request",
    "correlationId": "abc-123-def"
  }
]
```

### Cleanup Old Logs
```http
DELETE /audit/cleanup?olderThanDays=90
```

Deletes logs older than the specified number of days.

**Response:** 200 OK
```json
456
```
(Number of logs deleted)

---

## User Activities API

### List All Activities
```http
GET /activities?pageNumber=1&pageSize=50&activityType=Create&dateFrom=2024-01-01
```

**Query Parameters:**
- `pageNumber` (int, optional)
- `pageSize` (int, optional)
- `activityType` (enum, optional) - Login, Logout, Create, Update, Delete, View, Search, Download, Upload, Export, Import
- `dateFrom` (datetime, optional)
- `dateTo` (datetime, optional)

**Response:** 200 OK

### Get User Activities
```http
GET /activities/user/{userId}?pageNumber=1&pageSize=50&activityType=Create
```

**Response:** 200 OK

### Get Recent Activities
```http
GET /activities/recent?count=20
```

**Response:** 200 OK

### Get Activities by Resource
```http
GET /activities/resource/{resourceType}/{resourceId}?pageNumber=1&pageSize=50
```

**Response:** 200 OK

### Log Activity
```http
POST /activities
Content-Type: application/json
```

**Request Body:**
```json
{
  "userId": "guid",
  "username": "admin@axdd.com",
  "activityType": "Create",
  "activityDescription": "Created new resource",
  "ipAddress": "192.168.1.100",
  "deviceInfo": "Chrome 120 on Windows 10",
  "resourceType": "Enterprise",
  "resourceId": "guid",
  "oldValue": null,
  "newValue": "{...}",
  "additionalData": "{...}"
}
```

**Response:** 201 Created

---

## Error Logs API

### List Errors
```http
GET /errors?pageNumber=1&pageSize=50&severity=Critical&isResolved=false
```

**Query Parameters:**
- `pageNumber` (int, optional)
- `pageSize` (int, optional)
- `severity` (enum, optional) - Low, Medium, High, Critical
- `isResolved` (bool, optional)
- `dateFrom` (datetime, optional)
- `dateTo` (datetime, optional)

**Response:** 200 OK

### Get Error by ID
```http
GET /errors/{id}
```

**Response:** 200 OK, 404 Not Found

### Log Error
```http
POST /errors
Content-Type: application/json
```

**Request Body:**
```json
{
  "serviceName": "MyService",
  "errorMessage": "An error occurred",
  "stackTrace": "at MyService.Method()...",
  "severity": "High",
  "userId": "guid",
  "requestPath": "/api/v1/resource",
  "exceptionType": "InvalidOperationException",
  "correlationId": "abc-123-def",
  "additionalData": "{...}"
}
```

**Response:** 201 Created

### Get Unresolved Errors
```http
GET /errors/unresolved?pageNumber=1&pageSize=50
```

**Response:** 200 OK

### Resolve Error
```http
PUT /errors/{id}/resolve
Content-Type: application/json
```

**Request Body:**
```json
{
  "resolution": "Fixed by restarting the service",
  "resolvedBy": "guid"
}
```

**Response:** 200 OK

### Get Errors by Service
```http
GET /errors/service/{serviceName}?pageNumber=1&pageSize=50
```

**Response:** 200 OK

### Get Critical Errors
```http
GET /errors/critical?pageNumber=1&pageSize=50
```

**Response:** 200 OK

---

## Performance Logs API

### List Performance Logs
```http
GET /performance?pageNumber=1&pageSize=50&serviceName=Enterprise.Api
```

**Query Parameters:**
- `pageNumber` (int, optional)
- `pageSize` (int, optional)
- `serviceName` (string, optional)
- `dateFrom` (datetime, optional)
- `dateTo` (datetime, optional)

**Response:** 200 OK

### Log Performance
```http
POST /performance
Content-Type: application/json
```

**Request Body:**
```json
{
  "serviceName": "Enterprise.Api",
  "endpointName": "/api/v1/enterprises",
  "durationMs": 245,
  "memoryUsedMB": 128.5,
  "cpuUsagePercent": 35.2,
  "requestCount": 1,
  "successCount": 1,
  "errorCount": 0,
  "httpMethod": "POST",
  "statusCode": 201,
  "additionalData": "{...}"
}
```

**Response:** 201 Created

### Get Service Statistics
```http
GET /performance/statistics?serviceName=Enterprise.Api&startDate=2024-01-01&endDate=2024-01-31
```

**Response:** 200 OK
```json
{
  "serviceName": "Enterprise.Api",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-01-31T23:59:59Z",
  "totalRequests": 1250,
  "successCount": 1200,
  "errorCount": 50,
  "averageDurationMs": 234.5,
  "minDurationMs": 45,
  "maxDurationMs": 2150,
  "errorRate": 4.0
}
```

### Get Slow Requests
```http
GET /performance/slow?thresholdMs=1000&pageNumber=1&pageSize=50
```

**Response:** 200 OK

---

## Dashboard API

### Get Dashboard Summary
```http
GET /dashboard/summary
```

**Response:** 200 OK
```json
{
  "totalLogsToday": 1523,
  "errorsToday": 42,
  "activeUsersToday": 87,
  "averageResponseTimeMs": 245.6,
  "criticalErrorsUnresolved": 3,
  "logsByService": [
    {
      "serviceName": "Enterprise.Api",
      "logCount": 523,
      "errorCount": 12
    }
  ],
  "logsByHour": [
    {
      "hour": 9,
      "logCount": 145
    }
  ],
  "topUsers": [
    {
      "userId": "guid",
      "username": "admin@axdd.com",
      "activityCount": 234
    }
  ],
  "slowestEndpoints": [
    {
      "serviceName": "Search.Api",
      "endpointName": "/api/v1/search",
      "averageDurationMs": 1250.5,
      "requestCount": 89
    }
  ]
}
```

---

## Health Check

### Check Service Health
```http
GET /health
```

**Response:** 200 OK (Healthy), 503 Service Unavailable (Unhealthy)

---

## Common Response Codes

| Code | Description |
|------|-------------|
| 200 | OK - Request successful |
| 201 | Created - Resource created successfully |
| 400 | Bad Request - Invalid input |
| 404 | Not Found - Resource not found |
| 500 | Internal Server Error - Server error |

---

## Data Types

### AuditLogLevel Enum
- `Trace` = 0
- `Debug` = 1
- `Info` = 2
- `Warning` = 3
- `Error` = 4
- `Critical` = 5

### ActivityType Enum
- `Login` = 0
- `Logout` = 1
- `Create` = 2
- `Update` = 3
- `Delete` = 4
- `View` = 5
- `Search` = 6
- `Download` = 7
- `Upload` = 8
- `Export` = 9
- `Import` = 10

### ErrorSeverity Enum
- `Low` = 0
- `Medium` = 1
- `High` = 2
- `Critical` = 3

---

## Pagination

All list endpoints support pagination with the following parameters:
- `pageNumber` (int) - Page number, 1-based (default: 1)
- `pageSize` (int) - Items per page (default: 50, max: 100)

Response includes:
- `items` - Array of items
- `totalCount` - Total number of items
- `pageNumber` - Current page number
- `pageSize` - Items per page
- `totalPages` - Total number of pages
- `hasPreviousPage` - Boolean
- `hasNextPage` - Boolean

---

## Best Practices

### 1. Use Correlation IDs
Always include a correlation ID when logging to trace requests across services:
```json
{
  "correlationId": "unique-id-per-request"
}
```

### 2. Don't Log Sensitive Data
Never log passwords, tokens, credit card numbers, or other sensitive information.

### 3. Use Appropriate Log Levels
- **Trace/Debug**: Development only
- **Info**: Normal operations
- **Warning**: Unexpected but handled situations
- **Error**: Error conditions
- **Critical**: System failures

### 4. Include Context
Provide enough context to understand what happened:
```json
{
  "message": "Order created",
  "entityType": "Order",
  "entityId": "order-id",
  "userId": "user-id"
}
```

### 5. Handle Errors Gracefully
If logging fails, don't crash your application. Log locally and retry later.

---

## Rate Limiting

Currently, no rate limiting is implemented. Consider adding rate limiting in production:
- Per IP address: 100 requests/minute
- Per service: 1000 requests/minute

---

## Authentication

Currently, no authentication is implemented. In production, secure the API with:
- JWT tokens
- API keys
- OAuth 2.0

Example:
```http
Authorization: Bearer your-jwt-token
```

---

## Support

For more information, see:
- [README.md](README.md) - Comprehensive guide
- [QUICK_START.md](QUICK_START.md) - Quick setup
- [TECHNICAL_DOCUMENTATION.md](TECHNICAL_DOCUMENTATION.md) - Technical details
- [Swagger UI](https://localhost:5001/swagger) - Interactive API documentation
