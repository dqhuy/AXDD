# GIS Service Architecture

## System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         AXDD GIS Service                         │
│                         (ASP.NET Core 9.0)                       │
└─────────────────────────────────────────────────────────────────┘
                                 │
                    ┌────────────┴────────────┐
                    │                         │
            ┌───────▼────────┐       ┌───────▼────────┐
            │   Controllers   │       │   Middleware   │
            │   (REST API)    │       │   (Exception)  │
            └───────┬────────┘       └────────────────┘
                    │
            ┌───────▼────────┐
            │    Services     │
            │   (Business)    │
            └───────┬────────┘
                    │
            ┌───────▼────────┐
            │   DbContext     │
            │ (EF Core 9.0)   │
            └───────┬────────┘
                    │
         ┌──────────▼──────────┐
         │  PostgreSQL 16+     │
         │  + PostGIS 3.4      │
         │  (Spatial Database) │
         └─────────────────────┘
```

## Layer Architecture

### 1. API Layer (Controllers)
```
┌──────────────────────────────────────────────────────────┐
│                     Controllers                           │
├──────────────────────────────────────────────────────────┤
│  ┌────────────────┐  ┌───────────────────────┐          │
│  │ GisController  │  │ IndustrialZones       │          │
│  │                │  │ Controller            │          │
│  │ - Enterprises  │  │                       │          │
│  │ - Locations    │  │ - Zones CRUD          │          │
│  │ - Proximity    │  │ - Boundaries          │          │
│  └────────────────┘  └───────────────────────┘          │
│                                                           │
│  ┌────────────────┐  ┌───────────────────────┐          │
│  │ SpatialQuery   │  │ MapsController        │          │
│  │ Controller     │  │                       │          │
│  │                │  │ - OSM Tiles           │          │
│  │ - Queries      │  │ - Static Maps         │          │
│  │ - Validation   │  │ - Embed Info          │          │
│  └────────────────┘  └───────────────────────┘          │
└──────────────────────────────────────────────────────────┘
```

### 2. Service Layer (Business Logic)
```
┌──────────────────────────────────────────────────────────┐
│                    Services Layer                         │
├──────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────┐    │
│  │            IGisService                           │    │
│  │  - SaveEnterpriseLocationAsync                  │    │
│  │  - GetEnterpriseLocationAsync                   │    │
│  │  - GetEnterprisesByProximityAsync               │    │
│  │  - IsPointInZoneAsync                           │    │
│  └─────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────┐    │
│  │       IIndustrialZoneService                    │    │
│  │  - CreateZoneAsync                              │    │
│  │  - GetZoneBoundaryAsync                         │    │
│  │  - UpdateZoneBoundaryAsync                      │    │
│  │  - CalculateZoneAreaAsync                       │    │
│  └─────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────┐    │
│  │       ISpatialQueryService                      │    │
│  │  - PointInPolygon                               │    │
│  │  - DistanceBetween (Haversine)                  │    │
│  │  - BufferAroundPoint                            │    │
│  │  - CalculateAreaHectares                        │    │
│  └─────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────┐    │
│  │            IMapService                          │    │
│  │  - GetMapTileUrl                                │    │
│  │  - GenerateStaticMapUrl                         │    │
│  │  - GetBoundingBox                               │    │
│  └─────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────┘
```

### 3. Data Layer (Entities & DbContext)
```
┌──────────────────────────────────────────────────────────┐
│                  Data Access Layer                        │
├──────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────┐    │
│  │            GisDbContext                          │    │
│  │  (Inherits from BaseDbContext)                  │    │
│  │                                                  │    │
│  │  DbSets:                                        │    │
│  │  - EnterpriseLocations                          │    │
│  │  - IndustrialZones                              │    │
│  │  - LandPlots                                    │    │
│  └─────────────────────────────────────────────────┘    │
│                                                           │
│  ┌──────────────────┐  ┌───────────────────────────┐    │
│  │ Enterprise       │  │ IndustrialZone            │    │
│  │ Location         │  │                           │    │
│  │                  │  │ - Name, Code              │    │
│  │ - EnterpriseCode │  │ - Boundary (Polygon)      │    │
│  │ - Location(Point)│  │ - AreaHectares            │    │
│  │ - Lat/Lon        │  │ - Status                  │    │
│  │ - ZoneId         │  │ - Centroid                │    │
│  └──────────────────┘  └───────────────────────────┘    │
│                                                           │
│  ┌──────────────────┐                                    │
│  │ LandPlot         │                                    │
│  │                  │                                    │
│  │ - PlotNumber     │                                    │
│  │ - Geometry       │                                    │
│  │ - Area, Owner    │                                    │
│  │ - Status         │                                    │
│  └──────────────────┘                                    │
└──────────────────────────────────────────────────────────┘
```

## Spatial Data Flow

### 1. Save Enterprise Location
```
Client Request
    │
    ▼
GisController.SaveEnterpriseLocationAsync
    │
    ▼
GisService.SaveEnterpriseLocationAsync
    │
    ├─► SpatialQueryService.ValidateCoordinates()
    ├─► SpatialQueryService.CreatePoint(lat, lon)
    ├─► Check which zone contains point
    │   └─► DbContext.IndustrialZones.Where(z => z.Boundary.Contains(point))
    │
    ▼
Save to Database
    │
    ▼
Return LocationDto
```

### 2. Proximity Search
```
Client Request (lat, lon, radius)
    │
    ▼
GisController.GetEnterprisesByProximityAsync
    │
    ▼
GisService.GetEnterprisesByProximityAsync
    │
    ├─► SpatialQueryService.CreatePoint(lat, lon)
    ├─► SpatialQueryService.BufferAroundPoint(point, radius)
    │
    ▼
Spatial Query with Buffer
    │   DbContext.EnterpriseLocations
    │       .Where(l => buffer.Contains(l.Location))
    │
    ▼
Calculate Distances
    │   For each result:
    │   └─► SpatialQueryService.DistanceBetween(centerPoint, location)
    │
    ▼
Sort by Distance and Return
```

### 3. Create Industrial Zone
```
Client Request (GeoJSON boundary)
    │
    ▼
IndustrialZonesController.CreateZoneAsync
    │
    ▼
IndustrialZoneService.CreateZoneAsync
    │
    ├─► Convert GeoJSON to NetTopologySuite Polygon
    ├─► SpatialQueryService.GetCentroid(polygon)
    ├─► SpatialQueryService.CalculateAreaHectares(polygon)
    │
    ▼
Save Zone to Database
    │
    ▼
Return ZoneDto with calculated values
```

## Technology Stack

```
┌─────────────────────────────────────────────────────────┐
│                   Frontend / Client                      │
│         (Any HTTP client, Browser, Mobile App)          │
└────────────────────┬────────────────────────────────────┘
                     │ HTTP/REST
┌────────────────────▼────────────────────────────────────┐
│                  ASP.NET Core 9.0                        │
│  ┌───────────────────────────────────────────────┐     │
│  │  Controllers (REST API)                       │     │
│  └───────────────────────────────────────────────┘     │
│  ┌───────────────────────────────────────────────┐     │
│  │  Services (Business Logic)                    │     │
│  └───────────────────────────────────────────────┘     │
│  ┌───────────────────────────────────────────────┐     │
│  │  NetTopologySuite 2.5                         │     │
│  │  (Spatial Types: Point, Polygon, LineString)  │     │
│  └───────────────────────────────────────────────┘     │
│  ┌───────────────────────────────────────────────┐     │
│  │  Entity Framework Core 9.0                    │     │
│  │  + Npgsql Provider                            │     │
│  │  + NetTopologySuite Integration               │     │
│  └───────────────────────────────────────────────┘     │
└────────────────────┬────────────────────────────────────┘
                     │ Npgsql Driver
┌────────────────────▼────────────────────────────────────┐
│              PostgreSQL 16+ Database                     │
│  ┌───────────────────────────────────────────────┐     │
│  │  PostGIS 3.4 Extension                        │     │
│  │  (Spatial Database Functions)                 │     │
│  └───────────────────────────────────────────────┘     │
│  ┌───────────────────────────────────────────────┐     │
│  │  Tables with Geometry Columns                 │     │
│  │  - EnterpriseLocations (Point)                │     │
│  │  - IndustrialZones (Polygon)                  │     │
│  │  - LandPlots (Polygon)                        │     │
│  └───────────────────────────────────────────────┘     │
│  ┌───────────────────────────────────────────────┐     │
│  │  GIST Spatial Indexes                         │     │
│  │  (For efficient spatial queries)              │     │
│  └───────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────┘
```

## Coordinate Systems

```
┌─────────────────────────────────────────────────┐
│         WGS84 (SRID 4326)                       │
│     World Geodetic System 1984                  │
├─────────────────────────────────────────────────┤
│  Latitude:  -90° to +90° (South to North)       │
│  Longitude: -180° to +180° (West to East)       │
│                                                  │
│  Vietnam Bounds:                                │
│  - Latitude:  8° N to 24° N                     │
│  - Longitude: 102° E to 110° E                  │
│                                                  │
│  Format: Decimal Degrees                        │
│  Example: 10.9500°N, 106.8200°E                │
└─────────────────────────────────────────────────┘
```

## GeoJSON Format

```json
{
  "type": "Polygon",
  "coordinates": [
    [
      [longitude1, latitude1],
      [longitude2, latitude2],
      [longitude3, latitude3],
      [longitude4, latitude4],
      [longitude1, latitude1]  // Close the ring
    ]
  ]
}
```

**Note**: GeoJSON uses `[longitude, latitude]` order (reverse of common usage)

## Spatial Indexes

```
PostgreSQL GIST Index Structure:

EnterpriseLocations
  └─ Location (Point) [GIST Index]
     - Fast point-in-polygon queries
     - Fast proximity searches
     - Fast distance calculations

IndustrialZones
  └─ Boundary (Polygon) [GIST Index]
     - Fast point containment checks
     - Fast polygon intersection
     - Fast bounding box queries

LandPlots
  └─ Geometry (Polygon) [GIST Index]
     - Fast spatial relationships
     - Fast area queries
```

## Integration Points

```
┌──────────────────────────────────────────────────┐
│            AXDD GIS Service                      │
└───┬──────────────────────────────────────────┬───┘
    │                                          │
    ▼                                          ▼
┌────────────────┐                    ┌────────────────┐
│ Enterprise     │  Future            │ Auth Service   │
│ Service        │  Integration       │                │
│                │◄───────────────────┤ - JWT Tokens   │
│ - Sync Names   │                    │ - User Context │
│ - Details      │                    └────────────────┘
└────────────────┘                             │
                                               ▼
    ┌──────────────────────────────────────────────┐
    │          OpenStreetMap                       │
    │     (Map Tiles & Static Maps)                │
    └──────────────────────────────────────────────┘
```

## Performance Considerations

### Database Query Optimization
1. **Spatial Indexes (GIST)**: All geometry columns indexed
2. **Covering Indexes**: Additional indexes on frequently queried columns
3. **Query Plans**: Use PostGIS spatial functions for best performance
4. **Connection Pooling**: EF Core handles connection pooling

### Application Level
1. **Async/Await**: All I/O operations are async
2. **Caching**: Consider caching zone boundaries (rarely change)
3. **Pagination**: Implemented for large result sets
4. **Lazy Loading**: Disabled to avoid N+1 queries

### Spatial Calculations
1. **Haversine**: Used for accurate distance calculations
2. **Approximation**: Area calculations approximate for lat/lon
3. **PostGIS**: Leverage database for complex spatial operations
4. **Projection**: Consider projected coordinate systems for precision

## Security

### Current
- Input validation (coordinates, GeoJSON)
- Exception handling middleware
- SQL injection protection (EF Core parameterization)
- Health checks

### Future
- JWT authentication
- Role-based authorization
- Rate limiting
- API key management
- CORS configuration

## Monitoring & Observability

### Health Checks
- PostgreSQL connection
- PostGIS extension availability
- Database query performance

### Logging
- Structured logging (built-in .NET logging)
- Operation logs (saves, updates, queries)
- Error logs with context

### Metrics (Future)
- Request rates by endpoint
- Query execution times
- Spatial query complexity
- Cache hit rates
