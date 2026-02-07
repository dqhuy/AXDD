# AXDD MasterData Service

Reference Data / Master Data Service for the AXDD Platform.

## Overview

The MasterData Service provides centralized reference data management for all other services in the AXDD platform. It handles administrative divisions, industrial zones, industry codes, certificate types, document types, status codes, and system configurations.

## Features

### Core Entities

1. **Administrative Divisions**
   - 63 Provinces of Vietnam
   - Districts and Wards
   - Hierarchical relationships (Province → District → Ward)
   - Full address formatting

2. **Industrial Zones**
   - Comprehensive industrial zone catalog
   - Location information (Province, District)
   - Area, status, management details
   - Pre-seeded with Dong Nai industrial zones

3. **Industry Codes (VSIC)**
   - Vietnamese Standard Industrial Classification
   - 4-level hierarchy (Section → Division → Group → Class)
   - Search functionality
   - Pre-seeded with 50+ common codes

4. **Certificate Types**
   - Investment certificates
   - Business registration
   - Environmental permits
   - Construction permits
   - And more...

5. **Document Types**
   - Categorized by purpose (Investment, Legal, Financial, Environment)
   - File type restrictions
   - Size limits

6. **Status Codes**
   - Entity-specific statuses (Enterprise, Project, Report)
   - UI-friendly colors
   - Final/non-final indicators

7. **Configurations**
   - System-wide settings
   - Category-based organization
   - Typed values (String, Int, Boolean, JSON)

### Key Features

- **Redis Caching**: All master data is cached for optimal performance
- **Read-Heavy Optimization**: Designed for high-read, low-write scenarios
- **Comprehensive Seed Data**: Pre-populated with real Vietnamese data
- **RESTful API**: Clean, well-documented endpoints
- **Swagger Documentation**: Interactive API documentation
- **Health Checks**: Monitoring endpoints

## Technology Stack

- **.NET 9.0**
- **Entity Framework Core 9.0**
- **SQL Server**
- **Redis** (for caching)
- **Swagger/OpenAPI**

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (local or remote)
- Redis (optional, falls back to in-memory cache)

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MasterDataDatabase": "Server=localhost;Database=AXDD_MasterData;Trusted_Connection=True;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "CacheSettings": {
    "DefaultExpiryMinutes": 60,
    "ProvincesCacheMinutes": 1440,
    "IndustryCodesCacheMinutes": 1440
  }
}
```

### Running the Service

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Apply migrations (creates database and seeds data):**
   ```bash
   dotnet ef database update
   ```
   
   Note: Migrations run automatically when the application starts.

3. **Run the service:**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI:**
   ```
   https://localhost:5001/swagger
   ```

## API Endpoints

### Administrative Divisions

- `GET /api/v1/masterdata/administrativedivisions/provinces` - Get all provinces
- `GET /api/v1/masterdata/administrativedivisions/provinces/{id}` - Get province by ID
- `GET /api/v1/masterdata/administrativedivisions/provinces/{id}/districts` - Get districts by province
- `GET /api/v1/masterdata/administrativedivisions/districts/{id}/wards` - Get wards by district
- `GET /api/v1/masterdata/administrativedivisions/wards/{id}/full-address` - Get full address

### Industrial Zones

- `GET /api/v1/masterdata/industrialzones` - Get all industrial zones
- `GET /api/v1/masterdata/industrialzones/{id}` - Get by ID
- `GET /api/v1/masterdata/industrialzones/by-code/{code}` - Get by code
- `POST /api/v1/masterdata/industrialzones` - Create new (Admin)
- `PUT /api/v1/masterdata/industrialzones/{id}` - Update (Admin)

### Industry Codes (VSIC)

- `GET /api/v1/masterdata/industrycodes` - Get all codes
- `GET /api/v1/masterdata/industrycodes/{code}` - Get by code
- `GET /api/v1/masterdata/industrycodes/{code}/hierarchy` - Get hierarchy
- `GET /api/v1/masterdata/industrycodes/search?q={query}` - Search codes

### Certificate Types

- `GET /api/v1/masterdata/certificatetypes` - Get all types
- `GET /api/v1/masterdata/certificatetypes/{code}` - Get by code

### Document Types

- `GET /api/v1/masterdata/documenttypes` - Get all types
- `GET /api/v1/masterdata/documenttypes?category={category}` - Get by category
- `GET /api/v1/masterdata/documenttypes/{code}` - Get by code

### Status Codes

- `GET /api/v1/masterdata/statuscodes` - Get all status codes
- `GET /api/v1/masterdata/statuscodes?entityType={type}` - Get by entity type

### Configurations

- `GET /api/v1/masterdata/configurations` - Get all configurations
- `GET /api/v1/masterdata/configurations?category={category}` - Get by category
- `GET /api/v1/masterdata/configurations/{key}` - Get by key
- `PUT /api/v1/masterdata/configurations/{key}` - Update value (Admin)

## Usage Examples

### Get All Provinces

```bash
curl -X GET "https://localhost:5001/api/v1/masterdata/administrativedivisions/provinces"
```

Response:
```json
[
  {
    "id": "01000000-0000-0000-0000-000000000001",
    "code": "01",
    "name": "Hà Nội",
    "region": "North",
    "displayOrder": 1
  },
  {
    "id": "75000000-0000-0000-0000-000000000075",
    "code": "75",
    "name": "Đồng Nai",
    "region": "South",
    "displayOrder": 48
  }
]
```

### Search Industry Codes

```bash
curl -X GET "https://localhost:5001/api/v1/masterdata/industrycodes/search?q=thực%20phẩm"
```

### Get Status Codes for Projects

```bash
curl -X GET "https://localhost:5001/api/v1/masterdata/statuscodes?entityType=Project"
```

Response:
```json
[
  {
    "code": "DRAFT",
    "name": "Nháp",
    "entityType": "Project",
    "color": "#6c757d",
    "isFinal": false
  },
  {
    "code": "APPROVED",
    "name": "Đã phê duyệt",
    "entityType": "Project",
    "color": "#28a745",
    "isFinal": false
  }
]
```

## Caching Strategy

The service implements a **cache-aside** pattern:

1. Check cache first
2. If not found, query database
3. Store result in cache
4. Return data

### Cache Expiry Times

- **Provinces/Districts/Wards**: 24 hours
- **Industry Codes**: 24 hours
- **Other master data**: 1 hour
- **Configurations**: 1 hour

### Cache Keys

- Provinces: `masterdata:provinces:all`
- Districts: `masterdata:districts:province:{provinceId}`
- Industry Code: `masterdata:industrycode:{code}`
- Configuration: `masterdata:configuration:{key}`

## Seed Data

The service comes with comprehensive seed data:

- **63 provinces** of Vietnam
- **11 districts** of Dong Nai province
- **5 industrial zones** in Dong Nai
- **50+ industry codes** (VSIC classification)
- **6 certificate types**
- **8 document types**
- **13 status codes** for different entity types
- **6 system configurations**

## Database Migrations

### Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

### Apply migrations:
```bash
dotnet ef database update
```

### Rollback:
```bash
dotnet ef database update PreviousMigrationName
```

## Development

### Project Structure

```
AXDD.Services.MasterData.Api/
├── Configurations/        # EF Core entity configurations
├── Controllers/          # API controllers
├── Data/                 # DbContext and seeder
├── DTOs/                 # Data transfer objects
├── Entities/             # Domain entities
├── Exceptions/           # Custom exceptions
├── Migrations/           # EF Core migrations
├── Services/             # Business logic services
├── appsettings.json      # Configuration
└── Program.cs            # Application entry point
```

### Adding New Master Data Types

1. Create entity in `Entities/`
2. Create entity configuration in `Configurations/`
3. Add DbSet to `MasterDataDbContext`
4. Create DTOs in `DTOs/`
5. Create service interface and implementation in `Services/`
6. Create controller in `Controllers/`
7. Add seed data in `MasterDataSeeder`
8. Create and apply migration

## Performance Considerations

- All read operations use caching
- Indexes on frequently queried columns (Code, Name, EntityType)
- Composite indexes where appropriate
- Read-only queries use `.AsNoTracking()` (via services)
- Connection pooling enabled
- Retry logic for transient failures

## Monitoring

### Health Check Endpoint

```bash
curl -X GET "https://localhost:5001/health"
```

Response:
```json
{
  "status": "Healthy"
}
```

## Security

- Input validation on all endpoints
- Soft delete for all entities
- Audit fields (CreatedBy, UpdatedBy, etc.)
- Admin-only endpoints for write operations (TODO: Add authentication)

## Troubleshooting

### Redis Connection Issues

If Redis is not available, the service automatically falls back to in-memory caching. Check logs for warnings.

### Migration Issues

If you encounter migration issues, try:
```bash
dotnet ef database drop --force
dotnet ef database update
```

### Performance Issues

1. Check Redis connection
2. Review cache expiry settings
3. Enable SQL logging in `appsettings.Development.json`:
   ```json
   "Microsoft.EntityFrameworkCore": "Information"
   ```

## Contributing

When adding new master data types:
1. Follow existing patterns
2. Add comprehensive seed data
3. Implement caching
4. Add XML documentation
5. Update this README

## License

Copyright © 2024 AXDD Platform

## Contact

For questions or support, contact the AXDD development team.
