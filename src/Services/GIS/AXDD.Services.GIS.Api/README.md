# AXDD GIS Service

Geographic Information System (GIS) service for managing enterprise locations and industrial zones with PostgreSQL + PostGIS integration.

## Features

### ✅ Enterprise Location Management
- **Store and retrieve** geographic locations for enterprises
- **Spatial queries** - Find enterprises by proximity (radius search)
- **Zone detection** - Automatically detect which industrial zone an enterprise is in
- **Coordinate validation** - Validate coordinates within Vietnam bounds
- **GPS accuracy tracking** - Store accuracy information from GPS devices

### ✅ Industrial Zone Management
- **Zone boundaries** - Define industrial zones with polygon geometries
- **GeoJSON support** - Import/export zone boundaries in GeoJSON format
- **Area calculation** - Automatic area calculation in hectares
- **Zone search** - Search zones by name, code, or province
- **Enterprise listing** - List all enterprises within a zone

### ✅ Spatial Operations
- **Point-in-polygon** queries
- **Distance calculations** using Haversine formula
- **Buffer creation** around points
- **Polygon intersection** detection
- **Bounding box** calculations

### ✅ Map Integration
- **OpenStreetMap tiles** - Access OSM map tiles
- **Static maps** - Generate static map URLs
- **Map embed information** - Get data for embedding maps in applications

## Technology Stack

- **.NET 9.0** - Latest .NET framework
- **PostgreSQL 16+** with **PostGIS** extension
- **NetTopologySuite 2.5** - Spatial data types for .NET
- **Entity Framework Core 9.0** with Npgsql provider
- **Swagger/OpenAPI** - Interactive API documentation

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 16+ with PostGIS extension
- Docker (optional, for running PostgreSQL)

### Running PostgreSQL with PostGIS using Docker

```bash
docker run --name axdd-gis-postgres \
  -e POSTGRES_PASSWORD=postgres123 \
  -e POSTGRES_DB=AXDD_GIS \
  -p 5432:5432 \
  -d postgis/postgis:16-3.4
```

### Configuration

Update `appsettings.json` with your database connection:

```json
{
  "ConnectionStrings": {
    "GisDatabase": "Host=localhost;Port=5432;Database=AXDD_GIS;Username=postgres;Password=postgres123"
  }
}
```

### Database Migration

The service automatically applies migrations and creates the PostGIS extension on startup. If you need to run migrations manually:

```bash
dotnet ef database update
```

### Running the Service

```bash
dotnet run
```

The service will start on `http://localhost:5003` (or `https://localhost:7003` for HTTPS).

Access Swagger UI at: `http://localhost:5003/swagger`

## API Endpoints

### Enterprise Locations

#### Save/Update Enterprise Location
```http
POST /api/v1/gis/enterprises/{enterpriseCode}/location
Content-Type: application/json

{
  "latitude": 10.9500,
  "longitude": 106.8200,
  "address": "123 Industrial St, Biên Hòa, Đồng Nai",
  "accuracy": 10.0,
  "notes": "Main factory location",
  "isPrimary": true
}
```

#### Get Enterprise Location
```http
GET /api/v1/gis/enterprises/{enterpriseCode}/location
```

#### Find Nearby Enterprises
```http
GET /api/v1/gis/enterprises/nearby?latitude=10.9500&longitude=106.8200&radiusKm=5.0
```

Response includes distance in kilometers:
```json
[
  {
    "enterpriseCode": "DN001",
    "enterpriseName": "ABC Electronics",
    "latitude": 10.9520,
    "longitude": 106.8180,
    "distanceKm": 0.45,
    "industrialZoneName": "Khu Công Nghiệp Biên Hòa 1"
  }
]
```

#### Check if Point is in Industrial Zone
```http
GET /api/v1/gis/point-in-zone?latitude=10.9500&longitude=106.8200
```

### Industrial Zones

#### Get All Zones
```http
GET /api/v1/gis/industrial-zones
```

#### Get Zone Details
```http
GET /api/v1/gis/industrial-zones/{id}
```

#### Get Zone Boundary (GeoJSON)
```http
GET /api/v1/gis/industrial-zones/{id}/boundary
```

Returns GeoJSON Polygon:
```json
{
  "type": "Polygon",
  "coordinates": [
    [
      [106.8150, 10.9550],
      [106.8250, 10.9550],
      [106.8250, 10.9450],
      [106.8150, 10.9450],
      [106.8150, 10.9550]
    ]
  ]
}
```

#### Create Industrial Zone
```http
POST /api/v1/gis/industrial-zones
Content-Type: application/json

{
  "name": "Khu Công Nghiệp Mẫu",
  "code": "KCN-SAMPLE",
  "boundary": {
    "type": "Polygon",
    "coordinates": [...]
  },
  "province": "Đồng Nai",
  "district": "Biên Hòa",
  "description": "Industrial zone description",
  "establishedYear": 2020
}
```

#### List Enterprises in Zone
```http
GET /api/v1/gis/industrial-zones/{id}/enterprises
```

### Spatial Queries

#### Execute Spatial Query
```http
POST /api/v1/gis/spatial-query
Content-Type: application/json

{
  "type": "DistanceBetweenPoints",
  "point1": { "latitude": 10.9500, "longitude": 106.8200 },
  "point2": { "latitude": 10.7500, "longitude": 106.9500 }
}
```

Available query types:
- `DistanceBetweenPoints` - Calculate distance between two points (km)
- `PointInPolygon` - Check if point is within polygon
- `BufferAroundPoint` - Create circular buffer around point
- `PolygonIntersection` - Check if two polygons intersect

#### Validate Coordinates
```http
GET /api/v1/gis/spatial-query/validate-coordinates?latitude=10.9500&longitude=106.8200
```

### Maps

#### Get Map Tile URL
```http
GET /api/v1/maps/tiles/{zoom}/{x}/{y}
```

#### Generate Static Map
```http
GET /api/v1/maps/static?latitude=10.9500&longitude=106.8200&zoom=12
```

## Sample Data

The service includes seed data for three industrial zones in Đồng Nai:

1. **KCN Biên Hòa 1** (KCN-BH1)
   - Established: 1993
   - Location: Biên Hòa, Đồng Nai
   - Focus: Electronics and mechanical manufacturing

2. **KCN Long Thành** (KCN-LT)
   - Established: 2005
   - Location: Long Thành, Đồng Nai (near Long Thành Airport)
   - Focus: Logistics and high-tech manufacturing

3. **KCN Nhơn Trạch** (KCN-NT)
   - Established: 2018
   - Location: Nhơn Trạch, Đồng Nai
   - Focus: Heavy industry and energy
   - Status: Under construction

## Coordinate System

- **SRID**: 4326 (WGS84)
- **Format**: Decimal degrees
- **Order**: GeoJSON uses `[longitude, latitude]` order
- **Validation**: Coordinates are validated against Vietnam bounds (8-24°N, 102-110°E)

## Performance Considerations

- **Spatial Indexes**: GIST indexes are automatically created on all geometry columns
- **Query Optimization**: Use proximity searches with appropriate radius values
- **Caching**: Consider caching industrial zone boundaries
- **PostGIS Functions**: Leverage PostGIS built-in functions for complex spatial operations

## Error Handling

The service provides specific exception types:
- `InvalidCoordinatesException` - Invalid latitude/longitude values
- `LocationNotFoundException` - Enterprise location not found
- `IndustrialZoneNotFoundException` - Industrial zone not found
- `SpatialQueryException` - Spatial query operation failed

## Development

### Project Structure
```
AXDD.Services.GIS.Api/
├── Controllers/           # API controllers
├── Services/             
│   ├── Interfaces/       # Service interfaces
│   └── Implementations/  # Service implementations
├── Entities/             # Database entities
├── Data/                 # DbContext and migrations
├── DTOs/                 # Data transfer objects
├── Settings/             # Configuration classes
├── Exceptions/           # Custom exceptions
└── Program.cs            # Application entry point
```

### Running Tests

```bash
dotnet test
```

### Database Queries

Example PostGIS queries used by the service:

```sql
-- Find enterprises within 5km radius
SELECT *, ST_Distance(Location, ST_MakePoint(106.8200, 10.9500)::geography) / 1000 as distance_km
FROM "EnterpriseLocations"
WHERE ST_DWithin(Location::geography, ST_MakePoint(106.8200, 10.9500)::geography, 5000)
ORDER BY distance_km;

-- Find zone containing a point
SELECT * FROM "IndustrialZones"
WHERE ST_Contains(Boundary, ST_MakePoint(106.8200, 10.9500));
```

## OpenStreetMap Usage

This service uses OpenStreetMap tiles. Please note:
- **Usage Policy**: https://operations.osmfoundation.org/policies/tiles/
- **Rate Limiting**: OSM has usage limits for tile servers
- **Production**: Consider using a local tile server or commercial provider for production
- **Attribution**: Always provide proper attribution when displaying OSM tiles

## License

Copyright © 2024 AXDD Platform

## Support

For issues and questions, please open an issue in the repository or contact the development team.
