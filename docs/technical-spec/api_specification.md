# API Specification
# Hệ Thống AXDD - Quản Lý CSDL KCN Đồng Nai

**Phiên bản:** 2.0  
**Ngày cập nhật:** 05/02/2025  
**Base URL:** `https://api.axdd.dongnai.gov.vn`

---

## Mục Lục

1. [REST API Conventions](#1-rest-api-conventions)
2. [Authentication & Authorization](#2-authentication--authorization)
3. [Request/Response Format](#3-requestresponse-format)
4. [Error Handling](#4-error-handling)
5. [Pagination](#5-pagination)
6. [Filtering & Sorting](#6-filtering--sorting)
7. [Rate Limiting](#7-rate-limiting)
8. [API Endpoints](#8-api-endpoints)
9. [Webhooks](#9-webhooks)
10. [API Versioning](#10-api-versioning)

---

## 1. REST API Conventions

### 1.1. HTTP Methods

| Method | Usage | Idempotent | Safe |
|--------|-------|------------|------|
| `GET` | Retrieve resource(s) | Yes | Yes |
| `POST` | Create new resource | No | No |
| `PUT` | Update entire resource | Yes | No |
| `PATCH` | Update partial resource | No | No |
| `DELETE` | Delete resource | Yes | No |

### 1.2. URL Structure

```
https://{host}/{api-version}/{resource}/{id}/{sub-resource}

Examples:
GET    /api/v1/enterprises
GET    /api/v1/enterprises/{id}
GET    /api/v1/enterprises/{id}/projects
POST   /api/v1/enterprises
PUT    /api/v1/enterprises/{id}
PATCH  /api/v1/enterprises/{id}
DELETE /api/v1/enterprises/{id}
```

### 1.3. Naming Conventions

**Resources:**
- Use plural nouns: `/enterprises`, `/projects`
- Use kebab-case for multi-word resources: `/industrial-zones`
- Nested resources: `/enterprises/{id}/projects`

**Query Parameters:**
- Use camelCase: `?pageNumber=1&pageSize=20`
- Use descriptive names: `?status=active&sortBy=name`

**Request/Response Bodies:**
- Use camelCase for JSON properties

### 1.4. HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| `200 OK` | Success | Successful GET, PUT, PATCH |
| `201 Created` | Created | Successful POST |
| `204 No Content` | Success without body | Successful DELETE |
| `400 Bad Request` | Invalid request | Validation errors |
| `401 Unauthorized` | Not authenticated | Missing/invalid token |
| `403 Forbidden` | Not authorized | Insufficient permissions |
| `404 Not Found` | Resource not found | Resource doesn't exist |
| `409 Conflict` | Conflict | Duplicate resource |
| `422 Unprocessable Entity` | Validation failed | Business rule violation |
| `429 Too Many Requests` | Rate limit exceeded | Too many requests |
| `500 Internal Server Error` | Server error | Unexpected error |
| `503 Service Unavailable` | Service down | Service temporarily unavailable |

---

## 2. Authentication & Authorization

### 2.1. Authentication Flow

**1. Login:**

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "username": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "user@example.com",
    "fullName": "Nguyen Van A",
    "roles": ["Staff"]
  }
}
```

**2. Using Access Token:**

```http
GET /api/v1/enterprises
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**3. Refresh Token:**

```http
POST /api/v1/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
}
```

### 2.2. SSO with VNeID

```http
GET /api/v1/auth/sso/vneid
```

**Flow:**
1. Redirect to VNeID authorization endpoint
2. User authenticates with VNeID
3. VNeID redirects back with authorization code
4. Exchange code for access token
5. Get user info from VNeID
6. Create/update user in system
7. Return JWT token

### 2.3. JWT Token Structure

```json
{
  "header": {
    "alg": "RS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Nguyen Van A",
    "email": "user@example.com",
    "roles": ["Staff"],
    "permissions": [
      "Enterprise:Read",
      "Enterprise:Create"
    ],
    "iat": 1516239022,
    "exp": 1516242622,
    "iss": "AXDD.AuthService",
    "aud": "AXDD.Services"
  }
}
```

### 2.4. Permission Check

```http
GET /api/v1/enterprises
Authorization: Bearer {token}
X-Required-Permission: Enterprise:Read
```

**Permission Format:** `{Resource}:{Action}`

Examples:
- `Enterprise:Read`
- `Enterprise:Create`
- `Enterprise:Update`
- `Enterprise:Delete`
- `Investment:Approve`

---

## 3. Request/Response Format

### 3.1. Request Headers

**Required Headers:**

```http
Authorization: Bearer {access_token}
Content-Type: application/json
Accept: application/json
Accept-Language: vi-VN
```

**Optional Headers:**

```http
X-Request-ID: {unique-request-id}
X-Correlation-ID: {correlation-id}
X-Client-Version: 1.0.0
```

### 3.2. Request Body

**Example - Create Enterprise:**

```json
{
  "name": "Công ty TNHH ABC",
  "taxCode": "0123456789",
  "address": "123 Đường ABC, Phường XYZ",
  "industryCode": "C1011",
  "industrialZoneId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "registeredDate": "2024-01-15",
  "registeredCapital": 50000000000,
  "contactPerson": {
    "name": "Nguyen Van B",
    "position": "Giám đốc",
    "phone": "0901234567",
    "email": "contact@abc.com"
  }
}
```

### 3.3. Response Format

**Success Response:**

```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Công ty TNHH ABC",
    "taxCode": "0123456789",
    "status": "Active",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  },
  "message": "Enterprise created successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**List Response:**

```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Công ty TNHH ABC"
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalCount": 100,
    "hasPrevious": false,
    "hasNext": true
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 3.4. Response Envelope

**Standard Response Envelope:**

```typescript
interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: ApiError;
  message?: string;
  timestamp: string;
  requestId?: string;
}

interface ApiError {
  code: string;
  message: string;
  details?: any;
  validationErrors?: ValidationError[];
}

interface ValidationError {
  field: string;
  message: string;
  code: string;
}
```

---

## 4. Error Handling

### 4.1. Error Response Format

```json
{
  "success": false,
  "error": {
    "code": "ENTERPRISE_NOT_FOUND",
    "message": "Enterprise with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found",
    "details": {
      "enterpriseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  },
  "timestamp": "2024-01-15T10:30:00Z",
  "requestId": "req_abc123"
}
```

### 4.2. Validation Error Response

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "validationErrors": [
      {
        "field": "taxCode",
        "message": "Tax code must be 10 or 13 digits",
        "code": "INVALID_FORMAT"
      },
      {
        "field": "email",
        "message": "Email is required",
        "code": "REQUIRED"
      }
    ]
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 4.3. Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `INVALID_REQUEST` | 400 | Invalid request format |
| `VALIDATION_ERROR` | 400 | Request validation failed |
| `UNAUTHORIZED` | 401 | Authentication required |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `NOT_FOUND` | 404 | Resource not found |
| `ALREADY_EXISTS` | 409 | Resource already exists |
| `BUSINESS_RULE_VIOLATION` | 422 | Business rule violated |
| `RATE_LIMIT_EXCEEDED` | 429 | Too many requests |
| `INTERNAL_ERROR` | 500 | Internal server error |
| `SERVICE_UNAVAILABLE` | 503 | Service temporarily unavailable |

**Specific Error Codes:**

| Code | Description |
|------|-------------|
| `ENTERPRISE_NOT_FOUND` | Enterprise not found |
| `TAXCODE_DUPLICATE` | Tax code already exists |
| `INVALID_INDUSTRIAL_ZONE` | Invalid industrial zone |
| `CERTIFICATE_EXPIRED` | Certificate has expired |
| `INSUFFICIENT_CAPITAL` | Insufficient registered capital |

---

## 5. Pagination

### 5.1. Query Parameters

```http
GET /api/v1/enterprises?pageNumber=1&pageSize=20
```

**Parameters:**
- `pageNumber` (default: 1): Page number (1-based)
- `pageSize` (default: 20, max: 100): Items per page

### 5.2. Response with Pagination

```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalCount": 100,
    "hasPrevious": false,
    "hasNext": true,
    "links": {
      "self": "/api/v1/enterprises?pageNumber=1&pageSize=20",
      "first": "/api/v1/enterprises?pageNumber=1&pageSize=20",
      "last": "/api/v1/enterprises?pageNumber=5&pageSize=20",
      "next": "/api/v1/enterprises?pageNumber=2&pageSize=20",
      "previous": null
    }
  }
}
```

### 5.3. Cursor-Based Pagination (for large datasets)

```http
GET /api/v1/audit-logs?cursor=eyJpZCI6MTIzNDU2fQ&limit=50
```

**Response:**

```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "nextCursor": "eyJpZCI6MTIzNTA2fQ",
    "prevCursor": "eyJpZCI6MTIzNDA2fQ",
    "hasMore": true
  }
}
```

---

## 6. Filtering & Sorting

### 6.1. Filtering

```http
GET /api/v1/enterprises?status=Active&industrialZoneId={id}
GET /api/v1/enterprises?search=ABC
GET /api/v1/enterprises?createdFrom=2024-01-01&createdTo=2024-12-31
```

**Common Filter Parameters:**
- `search`: Full-text search
- `status`: Filter by status
- `createdFrom`, `createdTo`: Date range filter
- Resource-specific filters

### 6.2. Sorting

```http
GET /api/v1/enterprises?sortBy=name&sortDirection=asc
GET /api/v1/enterprises?sortBy=createdAt&sortDirection=desc
```

**Parameters:**
- `sortBy`: Field name
- `sortDirection`: `asc` or `desc` (default: `asc`)

**Multiple Sort:**

```http
GET /api/v1/enterprises?sortBy=status,name&sortDirection=asc,asc
```

### 6.3. Field Selection (Sparse Fieldsets)

```http
GET /api/v1/enterprises?fields=id,name,taxCode,status
```

**Response:**

```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Công ty TNHH ABC",
      "taxCode": "0123456789",
      "status": "Active"
    }
  ]
}
```

---

## 7. Rate Limiting

### 7.1. Rate Limit Headers

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1609459200
```

### 7.2. Rate Limits

| User Type | Limit | Window |
|-----------|-------|--------|
| Anonymous | 20 req | 1 minute |
| Authenticated | 100 req | 1 minute |
| Enterprise | 60 req | 1 minute |
| Admin | 500 req | 1 minute |

### 7.3. Rate Limit Exceeded Response

```http
HTTP/1.1 429 Too Many Requests
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1609459200
Retry-After: 60

{
  "success": false,
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded. Please try again in 60 seconds"
  }
}
```

---

## 8. API Endpoints

### 8.1. Authentication API

#### Login

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "username": "user@example.com",
  "password": "password"
}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJ...",
    "refreshToken": "7c9e6679...",
    "expiresIn": 3600,
    "user": {
      "id": "uuid",
      "username": "user@example.com",
      "fullName": "Nguyen Van A"
    }
  }
}
```

#### Logout

```http
POST /api/v1/auth/logout
Authorization: Bearer {token}
```

#### Refresh Token

```http
POST /api/v1/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
}
```

#### Get User Info

```http
GET /api/v1/auth/user-info
Authorization: Bearer {token}
```

---

### 8.2. Enterprise API

#### List Enterprises

```http
GET /api/v1/enterprises?pageNumber=1&pageSize=20&status=Active
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "name": "Công ty TNHH ABC",
      "taxCode": "0123456789",
      "address": "123 Đường ABC",
      "status": "Active",
      "industrialZone": {
        "id": "uuid",
        "name": "KCN Biên Hòa 1"
      },
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 100
  }
}
```

#### Get Enterprise by ID

```http
GET /api/v1/enterprises/{id}
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "Công ty TNHH ABC",
    "taxCode": "0123456789",
    "address": "123 Đường ABC, Phường XYZ",
    "phone": "0901234567",
    "email": "contact@abc.com",
    "website": "https://abc.com",
    "industryCode": "C1011",
    "industryName": "Chế biến thịt",
    "status": "Active",
    "registeredDate": "2024-01-15",
    "registeredCapital": 50000000000,
    "industrialZone": {
      "id": "uuid",
      "name": "KCN Biên Hòa 1",
      "code": "BH1"
    },
    "contactPerson": {
      "name": "Nguyen Van B",
      "position": "Giám đốc",
      "phone": "0901234567",
      "email": "director@abc.com"
    },
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }
}
```

#### Create Enterprise

```http
POST /api/v1/enterprises
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Công ty TNHH ABC",
  "taxCode": "0123456789",
  "address": "123 Đường ABC, Phường XYZ",
  "phone": "0901234567",
  "email": "contact@abc.com",
  "industryCode": "C1011",
  "industrialZoneId": "uuid",
  "registeredDate": "2024-01-15",
  "registeredCapital": 50000000000,
  "contactPerson": {
    "name": "Nguyen Van B",
    "position": "Giám đốc",
    "phone": "0901234567",
    "email": "director@abc.com"
  }
}
```

**Response:**

```http
HTTP/1.1 201 Created
Location: /api/v1/enterprises/{id}

{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "Công ty TNHH ABC",
    ...
  },
  "message": "Enterprise created successfully"
}
```

#### Update Enterprise

```http
PUT /api/v1/enterprises/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Công ty TNHH ABC Updated",
  "address": "456 Đường DEF",
  ...
}
```

#### Partial Update Enterprise

```http
PATCH /api/v1/enterprises/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Suspended",
  "notes": "Tạm ngừng hoạt động"
}
```

#### Delete Enterprise

```http
DELETE /api/v1/enterprises/{id}
Authorization: Bearer {token}
```

**Response:**

```http
HTTP/1.1 204 No Content
```

---

### 8.3. Investment API

#### List Investment Certificates

```http
GET /api/v1/investment/certificates?enterpriseId={id}
Authorization: Bearer {token}
```

#### Get Certificate by ID

```http
GET /api/v1/investment/certificates/{id}
Authorization: Bearer {token}
```

#### Create Investment Certificate

```http
POST /api/v1/investment/certificates
Authorization: Bearer {token}
Content-Type: application/json

{
  "certificateNumber": "GCNĐKĐT-2024-001",
  "enterpriseId": "uuid",
  "projectName": "Dự án sản xuất ABC",
  "totalInvestment": 100000000000,
  "implementationPeriod": 24,
  "issuedDate": "2024-01-15",
  "expiryDate": "2026-01-15",
  "status": "Active"
}
```

#### Adjust Investment Certificate

```http
POST /api/v1/investment/certificates/{id}/adjust
Authorization: Bearer {token}
Content-Type: application/json

{
  "adjustmentType": "CapitalIncrease",
  "adjustmentDate": "2024-06-15",
  "newTotalInvestment": 150000000000,
  "reason": "Mở rộng quy mô sản xuất",
  "attachments": [
    {
      "fileId": "uuid",
      "fileName": "Điều chỉnh dự án.pdf"
    }
  ]
}
```

---

### 8.4. File API

#### Upload File

```http
POST /api/v1/files/upload
Authorization: Bearer {token}
Content-Type: multipart/form-data

--boundary
Content-Disposition: form-data; name="file"; filename="document.pdf"
Content-Type: application/pdf

[file content]
--boundary
Content-Disposition: form-data; name="folderId"

uuid
--boundary--
```

**Response:**

```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "document.pdf",
    "size": 1024000,
    "mimeType": "application/pdf",
    "url": "/api/v1/files/{id}/download",
    "uploadedAt": "2024-01-15T10:30:00Z"
  }
}
```

#### Download File

```http
GET /api/v1/files/{id}/download
Authorization: Bearer {token}
```

**Response:**

```http
HTTP/1.1 200 OK
Content-Type: application/pdf
Content-Disposition: attachment; filename="document.pdf"
Content-Length: 1024000

[file content]
```

#### Share File

```http
POST /api/v1/files/{id}/share
Authorization: Bearer {token}
Content-Type: application/json

{
  "sharedWithUserId": "uuid",
  "permission": "Read",
  "expiresAt": "2024-12-31T23:59:59Z"
}
```

---

### 8.5. Search API

#### Search Enterprises

```http
POST /api/v1/search/enterprises
Authorization: Bearer {token}
Content-Type: application/json

{
  "query": "công ty ABC",
  "filters": {
    "status": "Active",
    "industrialZoneId": "uuid"
  },
  "pageNumber": 1,
  "pageSize": 20
}
```

**Response:**

```json
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "name": "Công ty TNHH ABC",
      "taxCode": "0123456789",
      "highlight": {
        "name": "Công ty TNHH <em>ABC</em>"
      },
      "score": 0.95
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 5
  },
  "took": 15
}
```

#### Get Suggestions

```http
GET /api/v1/search/suggestions?q=công+ty&type=enterprise
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "data": [
    "Công ty TNHH ABC",
    "Công ty Cổ phần DEF",
    "Công ty TNHH XYZ"
  ]
}
```

---

### 8.6. GIS API

#### Get Industrial Zone Boundary

```http
GET /api/v1/gis/industrial-zones/{id}/boundary
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "KCN Biên Hòa 1",
    "geometry": {
      "type": "Polygon",
      "coordinates": [
        [
          [106.8123, 10.9456],
          [106.8234, 10.9456],
          [106.8234, 10.9567],
          [106.8123, 10.9567],
          [106.8123, 10.9456]
        ]
      ]
    },
    "area": 250.5
  }
}
```

#### Spatial Query

```http
POST /api/v1/gis/spatial-query
Authorization: Bearer {token}
Content-Type: application/json

{
  "type": "PointInPolygon",
  "point": {
    "latitude": 10.9500,
    "longitude": 106.8180
  }
}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "industrialZone": {
      "id": "uuid",
      "name": "KCN Biên Hòa 1"
    },
    "distance": 0
  }
}
```

---

### 8.7. Report API

#### Get Dashboard

```http
GET /api/v1/reports/dashboard
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "totalEnterprises": 2100,
    "activeEnterprises": 1950,
    "totalInvestment": 500000000000000,
    "totalEmployees": 450000,
    "byIndustrialZone": [
      {
        "name": "KCN Biên Hòa 1",
        "enterprises": 350,
        "investment": 80000000000000
      }
    ],
    "byIndustry": [
      {
        "code": "C10",
        "name": "Sản xuất thực phẩm",
        "enterprises": 120
      }
    ]
  }
}
```

#### Generate Report

```http
POST /api/v1/reports/generate
Authorization: Bearer {token}
Content-Type: application/json

{
  "reportType": "EnterpriseList",
  "format": "Excel",
  "filters": {
    "status": "Active",
    "industrialZoneId": "uuid"
  }
}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "jobId": "uuid",
    "status": "Processing",
    "estimatedTime": 30
  }
}
```

#### Download Generated Report

```http
GET /api/v1/reports/download/{jobId}
Authorization: Bearer {token}
```

---

## 9. Webhooks

### 9.1. Webhook Events

| Event | Description |
|-------|-------------|
| `enterprise.created` | New enterprise created |
| `enterprise.updated` | Enterprise updated |
| `enterprise.deleted` | Enterprise deleted |
| `certificate.issued` | Certificate issued |
| `certificate.expired` | Certificate expired |
| `document.uploaded` | Document uploaded |
| `ocr.completed` | OCR processing completed |

### 9.2. Webhook Payload

```json
{
  "id": "evt_abc123",
  "type": "enterprise.created",
  "timestamp": "2024-01-15T10:30:00Z",
  "data": {
    "id": "uuid",
    "name": "Công ty TNHH ABC",
    "taxCode": "0123456789"
  }
}
```

### 9.3. Webhook Signature

```http
X-Webhook-Signature: sha256=abc123...
X-Webhook-Timestamp: 1609459200
```

**Verification:**

```csharp
var payload = await Request.Body.ReadAsStringAsync();
var timestamp = Request.Headers["X-Webhook-Timestamp"];
var signature = Request.Headers["X-Webhook-Signature"];

var computedSignature = HMAC_SHA256(
    $"{timestamp}.{payload}", 
    webhookSecret
);

if (signature != $"sha256={computedSignature}")
{
    return Unauthorized();
}
```

---

## 10. API Versioning

### 10.1. URL Versioning

```http
GET /api/v1/enterprises
GET /api/v2/enterprises
```

### 10.2. Header Versioning (Alternative)

```http
GET /api/enterprises
Accept: application/vnd.axdd.v1+json
```

### 10.3. Deprecation Notice

```http
HTTP/1.1 200 OK
Sunset: Sat, 31 Dec 2024 23:59:59 GMT
Link: </api/v2/enterprises>; rel="successor-version"
Deprecation: true

{
  "success": true,
  "data": [...],
  "deprecation": {
    "deprecated": true,
    "sunsetDate": "2024-12-31",
    "message": "This version will be retired on 2024-12-31. Please migrate to v2.",
    "documentationUrl": "https://docs.axdd.dongnai.gov.vn/api/v2/migration"
  }
}
```

---

## Appendix A: Complete API List

### Authentication
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/refresh-token`
- `POST /api/v1/auth/forgot-password`
- `POST /api/v1/auth/reset-password`
- `GET /api/v1/auth/user-info`

### Enterprise
- `GET /api/v1/enterprises`
- `POST /api/v1/enterprises`
- `GET /api/v1/enterprises/{id}`
- `PUT /api/v1/enterprises/{id}`
- `PATCH /api/v1/enterprises/{id}`
- `DELETE /api/v1/enterprises/{id}`

### Investment
- `GET /api/v1/investment/certificates`
- `POST /api/v1/investment/certificates`
- `GET /api/v1/investment/certificates/{id}`
- `PUT /api/v1/investment/certificates/{id}`
- `POST /api/v1/investment/certificates/{id}/adjust`
- `POST /api/v1/investment/certificates/{id}/extend`
- `POST /api/v1/investment/certificates/{id}/revoke`

### Files
- `POST /api/v1/files/upload`
- `GET /api/v1/files/{id}`
- `GET /api/v1/files/{id}/download`
- `DELETE /api/v1/files/{id}`
- `POST /api/v1/files/{id}/share`

### Search
- `POST /api/v1/search/enterprises`
- `POST /api/v1/search/projects`
- `POST /api/v1/search/documents`
- `GET /api/v1/search/suggestions`

### GIS
- `GET /api/v1/gis/industrial-zones`
- `GET /api/v1/gis/industrial-zones/{id}/boundary`
- `POST /api/v1/gis/spatial-query`

### Reports
- `GET /api/v1/reports/dashboard`
- `POST /api/v1/reports/generate`
- `GET /api/v1/reports/download/{jobId}`

---

## Appendix B: HTTP Status Codes Reference

| Code | Name | Usage |
|------|------|-------|
| 200 | OK | Successful request |
| 201 | Created | Resource created |
| 204 | No Content | Successful deletion |
| 400 | Bad Request | Invalid request |
| 401 | Unauthorized | Authentication required |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Resource conflict |
| 422 | Unprocessable Entity | Validation failed |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server error |
| 503 | Service Unavailable | Service down |

---

**Kết thúc tài liệu API Specification**
