# AXDD Docker Troubleshooting Guide

This guide helps you troubleshoot common issues when running the AXDD system with Docker.

## Table of Contents
1. [Service Won't Start](#service-wont-start)
2. [Port Already in Use](#port-already-in-use)
3. [Database Connection Issues](#database-connection-issues)
4. [Out of Memory](#out-of-memory)
5. [Slow Performance](#slow-performance)
6. [Build Failures](#build-failures)
7. [Network Issues](#network-issues)
8. [Data Persistence Issues](#data-persistence-issues)

---

## Service Won't Start

### Symptoms
- Service status shows "unhealthy" or "exited"
- Container keeps restarting

### Solutions

1. **Check the logs**:
```bash
docker compose logs <service-name>
```

2. **Check if dependencies are healthy**:
```bash
docker compose ps
```

3. **Verify environment variables**:
```bash
docker compose config
```

4. **Restart the service**:
```bash
docker compose restart <service-name>
```

5. **Rebuild the service**:
```bash
docker compose build --no-cache <service-name>
docker compose up -d <service-name>
```

---

## Port Already in Use

### Symptoms
- Error: "port is already allocated"
- Service fails to start with binding errors

### Solutions

1. **Find which process is using the port** (Linux/Mac):
```bash
lsof -i :<port>
```

2. **Find which process is using the port** (Windows):
```cmd
netstat -ano | findstr :<port>
```

3. **Change the port in docker-compose.yml**:
```yaml
services:
  auth-api:
    ports:
      - "5051:8080"  # Changed from 5001 to 5051
```

4. **Stop the conflicting service**:
```bash
# If it's another Docker container
docker stop <container-name>

# If it's a system service
sudo systemctl stop <service-name>
```

---

## Database Connection Issues

### SQL Server Connection Issues

#### Symptoms
- "A connection was successfully established with the server, but then an error occurred"
- "Login failed for user 'sa'"

#### Solutions

1. **Verify SQL Server is running**:
```bash
docker compose ps sqlserver
docker compose logs sqlserver
```

2. **Test connection manually**:
```bash
docker exec -it axdd-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT @@VERSION"
```

3. **Check if database exists**:
```bash
docker exec -it axdd-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT name FROM sys.databases"
```

4. **Recreate the database**:
```bash
docker exec -it axdd-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Passw0rd" \
  -Q "CREATE DATABASE AuthDB"
```

5. **Verify connection string**:
- Check `TrustServerCertificate=True` is present
- Verify password matches

### PostgreSQL Connection Issues

#### Symptoms
- "could not connect to server"
- "password authentication failed"

#### Solutions

1. **Verify PostgreSQL is running**:
```bash
docker compose ps postgres-gis
docker compose logs postgres-gis
```

2. **Test connection**:
```bash
docker exec -it axdd-postgres-gis psql -U postgres -d gisdb
```

3. **Check PostGIS extension**:
```bash
docker exec -it axdd-postgres-gis psql -U postgres -d gisdb \
  -c "SELECT PostGIS_version();"
```

4. **Recreate database with PostGIS**:
```sql
CREATE DATABASE gisdb;
\c gisdb
CREATE EXTENSION postgis;
```

---

## Out of Memory

### Symptoms
- Services randomly exiting
- System becomes unresponsive
- "Cannot allocate memory" errors

### Solutions

1. **Check Docker memory usage**:
```bash
docker stats
```

2. **Increase Docker memory limit**:
- Docker Desktop: Settings → Resources → Memory (increase to 8GB+)

3. **Reduce Elasticsearch heap size** in docker-compose.yml:
```yaml
elasticsearch:
  environment:
    - "ES_JAVA_OPTS=-Xms256m -Xmx256m"  # Reduced from 512m
```

4. **Stop non-essential services**:
```bash
# Stop search service if not needed
docker compose stop search-api elasticsearch
```

5. **Run services in stages**:
```bash
# Start infrastructure only
docker compose up -d sqlserver postgres-gis redis rabbitmq minio

# Start core services
docker compose up -d auth-api masterdata-api enterprise-api

# Start other services as needed
docker compose up -d api-gateway admin-webapp
```

---

## Slow Performance

### Symptoms
- Long startup times
- Slow API responses
- High CPU usage

### Solutions

1. **Check resource usage**:
```bash
docker stats
```

2. **Increase allocated resources** in docker-compose.yml:
```yaml
services:
  auth-api:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 1024M
```

3. **Use Docker's BuildKit** for faster builds:
```bash
export DOCKER_BUILDKIT=1
docker compose build
```

4. **Enable caching** properly:
- Ensure .dockerignore is properly configured
- Use multi-stage builds (already done in Dockerfiles)

5. **Clear Docker cache**:
```bash
docker builder prune
docker system prune -a
```

---

## Build Failures

### Symptoms
- "Failed to build" errors
- Compilation errors during build
- NuGet restore failures

### Solutions

1. **Clear build cache**:
```bash
docker compose build --no-cache <service-name>
```

2. **Check .dockerignore**:
- Ensure bin/, obj/ folders are ignored
- Verify necessary files are not ignored

3. **Verify SDK version**:
```bash
docker run --rm mcr.microsoft.com/dotnet/sdk:9.0 dotnet --version
```

4. **Check network connectivity**:
```bash
# Test NuGet connectivity
docker run --rm mcr.microsoft.com/dotnet/sdk:9.0 \
  dotnet nuget list source
```

5. **Build locally first**:
```bash
dotnet restore
dotnet build
```

---

## Network Issues

### Symptoms
- Services cannot communicate
- "Connection refused" errors
- DNS resolution failures

### Solutions

1. **Verify network exists**:
```bash
docker network ls | grep axdd
```

2. **Inspect network**:
```bash
docker network inspect axdd_axdd-network
```

3. **Recreate network**:
```bash
docker compose down
docker network prune -f
docker compose up -d
```

4. **Test connectivity between services**:
```bash
# From one container to another
docker exec -it axdd-auth-api ping redis
docker exec -it axdd-auth-api curl http://sqlserver:1433
```

5. **Check service names**:
- Use service names defined in docker-compose.yml
- Don't use container names for inter-service communication

---

## Data Persistence Issues

### Symptoms
- Data disappears after restart
- Volumes not mounting correctly
- Permission errors

### Solutions

1. **List volumes**:
```bash
docker volume ls | grep axdd
```

2. **Inspect volume**:
```bash
docker volume inspect axdd_sqlserver-data
```

3. **Check volume permissions**:
```bash
docker run --rm -v axdd_sqlserver-data:/data alpine ls -la /data
```

4. **Backup volume before recreating**:
```bash
docker run --rm -v axdd_sqlserver-data:/data -v $(pwd):/backup \
  alpine tar czf /backup/sqlserver-backup.tar.gz -C /data .
```

5. **Remove and recreate volume** (WARNING: Data loss):
```bash
docker compose down
docker volume rm axdd_sqlserver-data
docker compose up -d
```

---

## Advanced Troubleshooting

### Enable Debug Logging

Add to docker-compose.yml:
```yaml
services:
  auth-api:
    environment:
      - Logging__LogLevel__Default=Debug
      - Logging__LogLevel__Microsoft=Debug
```

### Access Container Shell

```bash
# .NET Services
docker exec -it axdd-auth-api /bin/bash

# Infrastructure services
docker exec -it axdd-sqlserver /bin/bash
docker exec -it axdd-postgres-gis /bin/sh
```

### View All Logs

```bash
# All services
docker compose logs -f

# Specific service with timestamps
docker compose logs -f --timestamps auth-api

# Last 100 lines
docker compose logs --tail=100 auth-api
```

### Export Logs

```bash
docker compose logs > axdd-logs.txt
```

---

## Getting Help

If you're still experiencing issues:

1. **Collect diagnostic information**:
```bash
# System info
docker version
docker compose version
docker info

# Service status
docker compose ps

# Logs
docker compose logs > diagnostic-logs.txt
```

2. **Check documentation**:
- README.md
- DOCKER_DEPLOYMENT.md
- Architecture docs in `/docs`

3. **Create an issue** on GitHub with:
- Description of the problem
- Steps to reproduce
- Diagnostic information
- Error messages from logs

---

## Prevention Tips

1. **Use health checks**: Already configured in docker-compose.yml
2. **Monitor resources**: Use `docker stats` regularly
3. **Regular maintenance**:
```bash
# Clean up unused resources weekly
docker system prune -f

# Update base images monthly
docker compose pull
docker compose up -d --build
```
4. **Backup data**: Regular backups of Docker volumes
5. **Use .env file**: Keep configuration separate from code
