# AXDD Enterprise Service

## Overview
The Enterprise Service is the core business domain service for managing 2,100+ enterprises in industrial zones. It provides comprehensive CRUD operations, status workflow management, contact management, license tracking, and complete audit history.

## Features

### Core Functionality
- ✅ **Full CRUD Operations** for enterprises, contacts, and licenses
- ✅ **Advanced Search & Filtering** by name, tax code, zone, industry, status
- ✅ **Pagination & Sorting** for all list endpoints
- ✅ **Status Workflow** with validation (Active, Suspended, Closed, Liquidated, UnderConstruction)
- ✅ **Contact Management** with main contact designation
- ✅ **License Tracking** with expiry alerts
- ✅ **Complete Audit Trail** for all changes
- ✅ **Statistics & Analytics** endpoint

### Business Rules
- Tax code must be 10 or 13 digits (Vietnamese format)
- Enterprise code must be unique
- At least one main contact required per enterprise
- Registered capital must be positive
- Status transitions are validated
- Cannot delete enterprise with active licenses

## API Endpoints

### Enterprises
```
GET    /api/v1/enterprises                    - List enterprises (paginated, filtered)
POST   /api/v1/enterprises                    - Create enterprise
GET    /api/v1/enterprises/{id}               - Get enterprise by ID
PUT    /api/v1/enterprises/{id}               - Update enterprise
DELETE /api/v1/enterprises/{id}               - Delete enterprise (soft)
GET    /api/v1/enterprises/code/{code}        - Get by code
GET    /api/v1/enterprises/taxcode/{taxCode}  - Get by tax code
POST   /api/v1/enterprises/{id}/status        - Change status
GET    /api/v1/enterprises/statistics         - Get statistics
GET    /api/v1/enterprises/{id}/contacts      - Get contacts
GET    /api/v1/enterprises/{id}/licenses      - Get licenses
GET    /api/v1/enterprises/{id}/history       - Get change history
```

### Contact Persons
```
GET    /api/v1/contactpersons/{id}            - Get contact by ID
POST   /api/v1/contactpersons                 - Create contact
PUT    /api/v1/contactpersons/{id}            - Update contact
DELETE /api/v1/contactpersons/{id}            - Delete contact
POST   /api/v1/contactpersons/{id}/set-main   - Set as main contact
```

### Licenses
```
GET    /api/v1/licenses/{id}                  - Get license by ID
POST   /api/v1/licenses                       - Create license
PUT    /api/v1/licenses/{id}                  - Update license
DELETE /api/v1/licenses/{id}                  - Delete license
GET    /api/v1/licenses/expiring?days=30      - Get expiring licenses
```

## API Examples

### Create Enterprise
```bash
POST /api/v1/enterprises
Content-Type: application/json

{
  "code": "DN-BH1-001",
  "name": "Công ty TNHH Sản xuất ABC",
  "taxCode": "0123456789",
  "industryCode": "C1011",
  "industryName": "Chế biến và bảo quản thịt",
  "industrialZoneId": "guid-here",
  "status": "Active",
  "address": "Lô A1, KCN Biên Hòa 1",
  "district": "Biên Hòa",
  "province": "Đồng Nai",
  "phone": "0251-3-xxx-xxx",
  "email": "info@abc.com.vn",
  "registeredCapital": 50000000000,
  "registeredDate": "2015-06-15"
}
```

### List Enterprises (Filtered & Paginated)
```bash
GET /api/v1/enterprises?pageNumber=1&pageSize=20&status=Active&searchTerm=ABC&sortBy=name&descending=false
```

### Change Status
```bash
POST /api/v1/enterprises/{id}/status
Content-Type: application/json

{
  "newStatus": "Suspended",
  "reason": "Environmental compliance review"
}
```

### Create Contact
```bash
POST /api/v1/contactpersons
Content-Type: application/json

{
  "enterpriseId": "guid-here",
  "fullName": "Nguyễn Văn A",
  "position": "Giám đốc",
  "department": "Ban Giám đốc",
  "phone": "0909-xxx-xxx",
  "email": "nvA@abc.com.vn",
  "isMain": true
}
```

### Create License
```bash
POST /api/v1/licenses
Content-Type: application/json

{
  "enterpriseId": "guid-here",
  "licenseType": "Environment",
  "licenseNumber": "1234/GP-STNMT",
  "issuedDate": "2023-01-15",
  "expiryDate": "2028-01-14",
  "issuingAuthority": "Sở Tài nguyên và Môi trường Đồng Nai",
  "status": "Active"
}
```

## Response Format

All responses follow the standard `ApiResponse<T>` format:

```json
{
  "isSuccess": true,
  "message": "Success",
  "data": { ... },
  "errors": null,
  "timestamp": "2024-02-06T10:30:00Z",
  "statusCode": 200,
  "metadata": null
}
```

## Database Schema

### Enterprises Table
- Id (Guid, PK)
- Code (string, unique, indexed)
- Name, TaxCode (indexed)
- Industry information (Code, Name)
- Industrial Zone (Id, Name)
- Status (enum: Active, Suspended, Closed, Liquidated, UnderConstruction)
- Legal representative details
- Address fields (Address, Ward, District, Province)
- Contact information (Phone, Fax, Email, Website)
- Financial data (RegisteredCapital, CharterCapital, AnnualRevenue)
- Employee counts (Total, Vietnam, Foreign)
- Audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, DeletedAt, DeletedBy, IsDeleted)

### ContactPersons Table
- Id (Guid, PK)
- EnterpriseId (Guid, FK, indexed)
- FullName, Position, Department
- Phone, Email
- IsMain (bool, indexed)
- Audit fields

### EnterpriseLicenses Table
- Id (Guid, PK)
- EnterpriseId (Guid, FK, indexed)
- LicenseType (enum, indexed)
- LicenseNumber, IssuedDate, ExpiryDate (indexed)
- IssuingAuthority, Status
- FileId (Guid, nullable)
- Audit fields

### EnterpriseHistories Table
- Id (Guid, PK)
- EnterpriseId (Guid, FK, indexed)
- ChangedAt (DateTime, indexed), ChangedBy
- ChangeType (enum, indexed)
- FieldName, OldValue, NewValue
- Reason, Details
- Audit fields

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "EnterpriseDatabase": "Server=localhost;Database=AXDD_Enterprise;..."
  },
  "EnterpriseSettings": {
    "DefaultPageSize": 20,
    "MaxPageSize": 100,
    "ExpiryAlertDays": 30
  },
  "IntegrationEndpoints": {
    "SearchService": "http://localhost:5007",
    "GisService": "http://localhost:5003",
    "FileService": "http://localhost:5005"
  }
}
```

## Running the Service

### Prerequisites
- .NET 9 SDK
- SQL Server (2019 or later)
- Entity Framework Core tools

### Setup
```bash
# Navigate to project directory
cd src/Services/Enterprise/AXDD.Services.Enterprise.Api

# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run the service
dotnet run
```

### Swagger UI
Once running, access Swagger documentation at:
```
https://localhost:5001/swagger
```

## Database Migrations

### Create Migration
```bash
dotnet ef migrations add MigrationName --output-dir Infrastructure/Data/Migrations
```

### Apply Migration
```bash
dotnet ef database update
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName
```

## Validation Rules

### Enterprise
- **Code**: Required, max 50 chars, uppercase letters/numbers/hyphens only
- **Name**: Required, max 500 chars
- **TaxCode**: Required, 10 or 13 digits only
- **IndustryCode**: Required, max 20 chars
- **Address**: Required, max 1000 chars
- **Email**: Valid email format when provided
- **Phone**: Valid Vietnamese phone format when provided
- **RegisteredCapital**: Must be > 0 when provided
- **Employees**: Vietnam + Foreign <= Total when provided

### Contact Person
- **FullName**: Required, max 200 chars
- **Email**: Valid email format when provided
- **IsMain**: Only one main contact per enterprise

### License
- **LicenseNumber**: Required, max 100 chars
- **LicenseType**: Required enum value
- **ExpiryDate**: Must be after IssuedDate when both provided

## Integration Points

### Search Service
- Auto-index enterprises on create/update
- Remove from index on delete
- Endpoint: `POST {SearchService}/api/v1/index/enterprise`

### GIS Service  
- Update location on address change
- Endpoint: `PUT {GisService}/api/v1/locations/enterprise/{id}`

### File Service
- Store license documents
- Endpoint: `POST {FileService}/api/v1/files`

## Health Checks

```bash
GET /health
```

Returns health status of the service and database connectivity.

## Error Handling

All errors are handled by the global `ExceptionHandlingMiddleware` and return consistent error responses:

```json
{
  "isSuccess": false,
  "message": "Error description",
  "errors": ["Detailed error 1", "Detailed error 2"],
  "timestamp": "2024-02-06T10:30:00Z",
  "statusCode": 400
}
```

## Performance Considerations

- **Indexes**: Created on frequently queried fields (Code, TaxCode, IndustrialZoneId, IndustryCode, Status, Name)
- **Soft Delete**: Global query filter automatically excludes deleted records
- **Pagination**: Required for list endpoints (max 100 items per page)
- **Async Operations**: All database operations use async/await
- **Eager Loading**: Contacts and licenses loaded with GetById for complete view

## Security

- **Soft Delete**: Records marked as deleted, not physically removed
- **Audit Trail**: Complete history of all changes with user tracking
- **Validation**: Server-side validation using FluentValidation
- **CORS**: Configured for cross-origin requests

## Future Enhancements

- [ ] Add import/export functionality (Excel, CSV)
- [ ] Implement real-time notifications for license expiry
- [ ] Add attachment support for enterprise documents
- [ ] Implement advanced analytics dashboards
- [ ] Add bulk operations (bulk update status, etc.)
- [ ] Implement caching for frequently accessed data
- [ ] Add GraphQL endpoint for flexible queries
