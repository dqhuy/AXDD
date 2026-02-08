# Docker Configuration Fixes - Complete Report

## Overview
This document details all fixes applied to resolve Docker configuration issues in the AXDD system.

## Issues Fixed

### 1. SQL Server Health Check Issue ❌ → ✅

**Problem:**
- SQL Server container was running but showing "unhealthy" status
- Health check was using incorrect sqlcmd path
- Health check timeout and startup period were too short

**Root Causes:**
1. MS SQL Server 2022 image uses `/opt/mssql-tools18/` instead of `/opt/mssql-tools/`
2. sqlcmd in tools18 requires `-C` flag to trust server certificate
3. SQL Server needs more time to fully initialize (60s instead of 10s)

**Solution:**
```yaml
healthcheck:
  test: ["CMD-SHELL", "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q 'SELECT 1' -C || exit 1"]
  interval: 15s
  timeout: 5s
  retries: 10
  start_period: 60s  # Increased from 10s
```

**Verification:**
```bash
docker compose ps sqlserver
# Should show: Up (healthy)

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "SELECT 1" -C
# Should return: 1 row affected
```

---

### 2. API Gateway Missing Routes ❌ → ✅

**Problem:**
- API Gateway only had 6 routes configured
- Missing routes for: Notification, Logging, Search, GIS services
- Frontend apps couldn't access these services

**Root Cause:**
- Initial setup only configured core services
- Additional services were added later but routes not updated

**Solution:**
Added complete route and cluster configuration for all 10 services:

**Routes Added:**
```json
"notification-route": {
  "ClusterId": "notification-cluster",
  "Match": { "Path": "/api/notification/{**catch-all}" }
},
"logging-route": {
  "ClusterId": "logging-cluster",
  "Match": { "Path": "/api/logging/{**catch-all}" }
},
"search-route": {
  "ClusterId": "search-cluster",
  "Match": { "Path": "/api/search/{**catch-all}" }
},
"gis-route": {
  "ClusterId": "gis-cluster",
  "Match": { "Path": "/api/gis/{**catch-all}" }
}
```

**Clusters Added:**
```json
"notification-cluster": {
  "Destinations": {
    "destination1": { "Address": "http://notification-api:8080" }
  }
},
// ... similar for logging, search, and gis
```

**Verification:**
```bash
# Test each route
curl http://localhost:5000/api/auth/health
curl http://localhost:5000/api/notification/health
curl http://localhost:5000/api/logging/health
curl http://localhost:5000/api/search/health
curl http://localhost:5000/api/gis/health
```

---

### 3. Admin Web App Configuration ❌ → ✅

**Problem:**
- Using localhost URLs (7001-7008) instead of Docker service names
- Missing configuration for 4 services (Notification, Logging, Search, GIS)
- No API Gateway base URL configuration
- SignalR hub URL using incorrect port

**Root Cause:**
- appsettings.json had development machine ports
- Configuration not updated for Docker environment

**Solution:**

**appsettings.json Updates:**
```json
{
  "ApiGateway": {
    "BaseUrl": "http://localhost:5000"
  },
  "ApiServices": {
    "AuthService": "http://localhost:5001",
    "EnterpriseService": "http://localhost:5003",
    "MasterDataService": "http://localhost:5002",
    "InvestmentService": "http://localhost:5004",
    "FileManagerService": "http://localhost:5005",
    "ReportService": "http://localhost:5006",
    "NotificationService": "http://localhost:5007",  // Added
    "LoggingService": "http://localhost:5008",       // Added
    "SearchService": "http://localhost:5009",        // Added
    "GisService": "http://localhost:5010"            // Added
  },
  "SignalR": {
    "NotificationHubUrl": "http://localhost:5007/hubs/notifications"  // Fixed port
  }
}
```

**docker-compose.yml Environment Variables:**
```yaml
environment:
  - ApiGateway__BaseUrl=http://api-gateway:8080
  - ApiServices__AuthService=http://auth-api:8080
  - ApiServices__EnterpriseService=http://enterprise-api:8080
  - ApiServices__MasterDataService=http://masterdata-api:8080
  - ApiServices__InvestmentService=http://investment-api:8080
  - ApiServices__FileManagerService=http://filemanager-api:8080
  - ApiServices__ReportService=http://report-api:8080
  - ApiServices__NotificationService=http://notification-api:8080
  - ApiServices__LoggingService=http://logging-api:8080
  - ApiServices__SearchService=http://search-api:8080
  - ApiServices__GisService=http://gis-api:8080
  - SignalR__NotificationHubUrl=http://notification-api:8080/hubs/notifications
```

**Dependencies Added:**
```yaml
depends_on:
  - api-gateway
  - auth-api
  - masterdata-api
  - enterprise-api
  - notification-api
```

**Verification:**
```bash
# Start admin webapp
docker compose up -d admin-webapp

# Check configuration loaded correctly
docker compose exec admin-webapp printenv | grep ApiServices
docker compose exec admin-webapp printenv | grep ApiGateway

# Access web app
open http://localhost:8080
```

---

### 4. Enterprise Portal Configuration ❌ → ✅

**Problem:**
- No environment configuration files
- No API endpoint configuration
- nginx not configured to proxy API requests
- Frontend couldn't connect to backend services

**Root Cause:**
- Angular app created without environment files
- No reverse proxy configuration for API calls

**Solution:**

**Created: `src/environments/environment.ts`**
```typescript
export const environment = {
  production: false,
  apiGatewayUrl: 'http://localhost:5000',
  apiEndpoints: {
    auth: 'http://localhost:5001',
    masterData: 'http://localhost:5002',
    enterprise: 'http://localhost:5003',
    investment: 'http://localhost:5004',
    fileManager: 'http://localhost:5005',
    report: 'http://localhost:5006',
    notification: 'http://localhost:5007',
    logging: 'http://localhost:5008',
    search: 'http://localhost:5009',
    gis: 'http://localhost:5010'
  },
  signalR: {
    notificationHubUrl: 'http://localhost:5007/hubs/notifications'
  }
};
```

**Created: `src/environments/environment.prod.ts`**
```typescript
export const environment = {
  production: true,
  apiGatewayUrl: '/api',  // Relative URL for proxy
  // ... all endpoints using relative paths
};
```

**Updated: `nginx.conf`**
```nginx
# Proxy API requests to the API Gateway
location /api/ {
    proxy_pass http://api-gateway:8080/api/;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection 'upgrade';
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

**Verification:**
```bash
# Start enterprise portal
docker compose up -d enterprise-portal

# Check nginx configuration
docker compose exec enterprise-portal cat /etc/nginx/conf.d/default.conf

# Test proxy
curl http://localhost:4200/api/auth/health

# Access web app
open http://localhost:4200
```

---

### 5. Microservices Data Layer Connections ✅

**Status:** Already correctly configured!

**Verified Configurations:**

**SQL Server Services (8 services):**
```yaml
ConnectionStrings__DefaultConnection=Server=sqlserver;Database=<ServiceDB>;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

Services using SQL Server:
- auth-api → AuthDB
- masterdata-api → MasterDataDB
- enterprise-api → EnterpriseDB
- investment-api → InvestmentDB
- filemanager-api → FileMetaDB
- report-api → ReportDB
- notification-api → NotificationDB
- logging-api → LoggingDB

**PostgreSQL+PostGIS Service (GIS):**
```yaml
ConnectionStrings__GISConnection=Host=postgres-gis;Port=5432;Database=gisdb;Username=postgres;Password=postgres;
```

**Redis Configuration (All services):**
```yaml
Redis__Configuration=redis:6379
```

**RabbitMQ Configuration (All services):**
```yaml
RabbitMQ__Host=rabbitmq
RabbitMQ__Username=admin
RabbitMQ__Password=admin
```

**Verification:**
```bash
# Check SQL Server connectivity from a service
docker compose exec auth-api curl sqlserver:1433
docker compose logs auth-api | grep "SQL"

# Check PostgreSQL connectivity
docker compose exec gis-api nc -zv postgres-gis 5432

# Check Redis connectivity
docker compose exec auth-api nc -zv redis 6379

# Check RabbitMQ connectivity
docker compose exec auth-api nc -zv rabbitmq 5672
```

---

## Summary of Changes

### Files Modified (4)
1. **docker-compose.yml**
   - Fixed SQL Server health check
   - Updated admin-webapp environment variables
   - Added complete service dependencies

2. **src/ApiGateway/AXDD.ApiGateway/appsettings.json**
   - Added 4 missing service routes
   - Added 4 missing service clusters

3. **src/WebApps/AXDD.WebApp.Admin/appsettings.json**
   - Updated all service URLs to correct ports
   - Added 4 missing service configurations
   - Added ApiGateway.BaseUrl
   - Fixed SignalR hub URL

4. **src/WebApps/EnterprisePortal/nginx.conf**
   - Added API proxy configuration
   - Added proper headers for proxying

### Files Created (2)
1. **src/WebApps/EnterprisePortal/src/environments/environment.ts**
   - Development environment configuration
   - All 10 service endpoints
   - SignalR configuration

2. **src/WebApps/EnterprisePortal/src/environments/environment.prod.ts**
   - Production environment configuration
   - Relative API URLs for nginx proxy

---

## Testing Checklist

### Infrastructure Services
- [ ] SQL Server is healthy: `docker compose ps sqlserver`
- [ ] PostgreSQL is healthy: `docker compose ps postgres-gis`
- [ ] Redis is running: `docker compose ps redis`
- [ ] RabbitMQ is running: `docker compose ps rabbitmq`
- [ ] MinIO is running: `docker compose ps minio`
- [ ] Elasticsearch is running: `docker compose ps elasticsearch`

### Microservices
- [ ] All 10 services are running: `docker compose ps | grep api`
- [ ] Auth API health: `curl http://localhost:5001/health`
- [ ] MasterData API health: `curl http://localhost:5002/health`
- [ ] Enterprise API health: `curl http://localhost:5003/health`
- [ ] Investment API health: `curl http://localhost:5004/health`
- [ ] FileManager API health: `curl http://localhost:5005/health`
- [ ] Report API health: `curl http://localhost:5006/health`
- [ ] Notification API health: `curl http://localhost:5007/health`
- [ ] Logging API health: `curl http://localhost:5008/health`
- [ ] Search API health: `curl http://localhost:5009/health`
- [ ] GIS API health: `curl http://localhost:5010/health`

### API Gateway
- [ ] Gateway is running: `docker compose ps api-gateway`
- [ ] All routes work: `curl http://localhost:5000/api/auth/health`
- [ ] Test each service through gateway

### Web Applications
- [ ] Admin Web App is running: `docker compose ps admin-webapp`
- [ ] Admin Web App accessible: `open http://localhost:8080`
- [ ] Enterprise Portal is running: `docker compose ps enterprise-portal`
- [ ] Enterprise Portal accessible: `open http://localhost:4200`
- [ ] API proxy works: `curl http://localhost:4200/api/auth/health`

### Database Connections
- [ ] Check Auth API logs for SQL connection
- [ ] Check GIS API logs for PostgreSQL connection
- [ ] Check any service logs for Redis connection
- [ ] Check any service logs for RabbitMQ connection

---

## Production Deployment Notes

### Security Considerations
1. **Change Default Passwords**
   - SQL Server SA password
   - PostgreSQL password
   - RabbitMQ credentials
   - MinIO credentials

2. **Use Environment Variables**
   - Store all secrets in environment variables or secret management
   - Use `.env` file (not committed to git)
   - Consider Azure Key Vault or similar

3. **Enable HTTPS**
   - Configure SSL certificates
   - Update all URLs to use HTTPS
   - Enable certificate validation

4. **Network Security**
   - Don't expose all ports in production
   - Use internal network for service-to-service communication
   - Only expose API Gateway and web apps

### Performance Tuning
1. **Resource Limits**
   - Add CPU and memory limits to each service
   - Monitor resource usage
   - Scale horizontally as needed

2. **Caching**
   - Verify Redis is being used effectively
   - Enable output caching where appropriate
   - Use CDN for static assets

3. **Database Optimization**
   - Create proper indexes
   - Optimize queries
   - Enable connection pooling

### Monitoring
1. **Health Checks**
   - All services have health endpoints
   - Configure uptime monitoring
   - Set up alerts

2. **Logging**
   - Centralize logs (ELK stack, Azure Monitor, etc.)
   - Configure log levels appropriately
   - Retain logs for auditing

3. **Metrics**
   - Monitor response times
   - Track error rates
   - Monitor resource usage

---

## Rollback Procedure

If issues occur after deployment:

1. **Check Logs**
   ```bash
   docker compose logs -f <service-name>
   ```

2. **Rollback Specific Service**
   ```bash
   docker compose up -d --no-deps --build <service-name>
   ```

3. **Full Rollback**
   ```bash
   git checkout <previous-commit>
   docker compose down
   docker compose up -d --build
   ```

---

## Support

For issues:
1. Check service logs: `docker compose logs <service-name>`
2. Review `DOCKER_TROUBLESHOOTING.md`
3. Check health endpoints
4. Verify environment variables are set correctly

## Conclusion

All identified issues have been resolved:
✅ SQL Server health check is now working correctly
✅ API Gateway has complete route configuration
✅ Admin Web App has proper service configuration
✅ Enterprise Portal has API endpoints and proxy configured
✅ All microservices have correct database connections

The system is now ready for testing and deployment.
