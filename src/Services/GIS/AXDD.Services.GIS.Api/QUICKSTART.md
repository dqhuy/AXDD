# GIS Service Quick Start Guide

## Prerequisites

1. **PostgreSQL with PostGIS** running (see Docker command below)
2. **.NET 9.0 SDK** installed

## Step 1: Start PostgreSQL with PostGIS

```bash
docker run --name axdd-gis-postgres \
  -e POSTGRES_PASSWORD=postgres123 \
  -e POSTGRES_DB=AXDD_GIS \
  -p 5432:5432 \
  -d postgis/postgis:16-3.4
```

## Step 2: Update Configuration (Optional)

The default configuration in `appsettings.Development.json` is already set for the Docker container above. If you're using a different database, update the connection string:

```json
{
  "ConnectionStrings": {
    "GisDatabase": "Host=localhost;Port=5432;Database=AXDD_GIS_Dev;Username=postgres;Password=postgres123"
  }
}
```

## Step 3: Run the Service

```bash
cd src/Services/GIS/AXDD.Services.GIS.Api
dotnet run
```

The service will:
1. Automatically apply database migrations
2. Create the PostGIS extension
3. Seed sample industrial zones
4. Start the API on http://localhost:5003

## Step 4: Access Swagger UI

Open your browser and navigate to:
```
http://localhost:5003/swagger
```

## Quick Test Examples

### 1. Check Point in Zone

Check if coordinates are in an industrial zone:
```bash
curl "http://localhost:5003/api/v1/gis/point-in-zone?latitude=10.9500&longitude=106.8200"
```

### 2. Get All Industrial Zones

```bash
curl "http://localhost:5003/api/v1/gis/industrial-zones"
```

### 3. Save Enterprise Location

```bash
curl -X POST "http://localhost:5003/api/v1/gis/enterprises/TEST001/location" \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 10.9520,
    "longitude": 106.8180,
    "address": "Test Address, Bien Hoa",
    "isPrimary": true
  }'
```

### 4. Find Nearby Enterprises

```bash
curl "http://localhost:5003/api/v1/gis/enterprises/nearby?latitude=10.9500&longitude=106.8200&radiusKm=5"
```

### 5. Calculate Distance Between Points

```bash
curl -X POST "http://localhost:5003/api/v1/gis/spatial-query" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "DistanceBetweenPoints",
    "point1": { "latitude": 10.9500, "longitude": 106.8200 },
    "point2": { "latitude": 10.7500, "longitude": 106.9500 }
  }'
```

## Seeded Data

The service includes 3 sample industrial zones:

1. **KCN Biên Hòa 1** (code: KCN-BH1)
   - Center: ~10.95°N, 106.82°E
   - Includes 2 sample enterprises

2. **KCN Long Thành** (code: KCN-LT)
   - Center: ~10.75°N, 106.95°E

3. **KCN Nhơn Trạch** (code: KCN-NT)
   - Center: ~10.715°N, 106.915°E
   - Status: Under Construction

## Map Visualization

### Get Map Embed Info

```bash
curl "http://localhost:5003/api/v1/maps/embed-info?latitude=10.9500&longitude=106.8200&zoom=12"
```

This returns URLs for:
- OSM tile server
- Embed URL for the map
- Attribution text

## Health Check

```bash
curl "http://localhost:5003/health"
```

Should return `Healthy` if PostgreSQL connection is working.

## Troubleshooting

### Database Connection Failed
- Ensure PostgreSQL container is running: `docker ps`
- Check logs: `docker logs axdd-gis-postgres`

### PostGIS Extension Not Found
The service automatically creates the extension. If it fails:
```sql
-- Connect to the database and run:
CREATE EXTENSION IF NOT EXISTS postgis;
```

### Port Already in Use
Change the port in `Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:5004"
```

## Next Steps

- Explore the full API documentation in Swagger
- Check `README.md` for detailed API reference
- Review the spatial query capabilities
- Test with your own enterprise data

## Development Mode

For development with hot reload:
```bash
dotnet watch run
```

## Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## Docker Deployment

Build the Docker image:
```bash
docker build -t axdd-gis-service -f Dockerfile ../../../..
```

Run the container:
```bash
docker run -d \
  --name axdd-gis-api \
  -p 5003:80 \
  -e ConnectionStrings__GisDatabase="Host=host.docker.internal;Database=AXDD_GIS;Username=postgres;Password=postgres123" \
  axdd-gis-service
```

## Support

For issues or questions, please refer to the main README.md or contact the development team.
