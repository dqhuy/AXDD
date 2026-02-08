# AXDD System - Docker Deployment Guide

This guide provides instructions for deploying the complete AXDD system using Docker and Docker Compose.

## System Overview

The AXDD system consists of:
- **10 Microservices**: Auth, MasterData, Enterprise, Investment, FileManager, Report, Notification, Logging, Search, GIS
- **1 API Gateway**: Central entry point for all services
- **2 Web Applications**: Admin Portal (MVC), Enterprise Portal (Angular)
- **6 Infrastructure Components**: SQL Server, PostgreSQL+PostGIS, Redis, RabbitMQ, MinIO, Elasticsearch

## Prerequisites

- Docker Engine 20.10+ 
- Docker Compose 2.0+
- Minimum 8GB RAM (16GB recommended)
- Minimum 50GB disk space

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/dqhuy/AXDD.git
cd AXDD
```

### 2. Start All Services

```bash
docker-compose up -d
```

This command will:
- Pull required base images
- Build all microservices and web applications
- Start infrastructure services
- Create necessary networks and volumes
- Configure service dependencies

### 3. Monitor Service Startup

```bash
# View logs from all services
docker-compose logs -f

# View logs from specific service
docker-compose logs -f auth-api

# Check service status
docker-compose ps
```

### 4. Wait for Services to be Ready

The system uses health checks to ensure services are ready. Wait for all health checks to pass:

```bash
watch docker-compose ps
```

## Service Endpoints

### Web Applications
- **Admin Portal**: http://localhost:8080
- **Enterprise Portal**: http://localhost:4200

### API Gateway
- **API Gateway**: http://localhost:5000

### Microservices (Direct Access)
- **Auth Service**: http://localhost:5001
- **MasterData Service**: http://localhost:5002
- **Enterprise Service**: http://localhost:5003
- **Investment Service**: http://localhost:5004
- **FileManager Service**: http://localhost:5005
- **Report Service**: http://localhost:5006
- **Notification Service**: http://localhost:5007
- **Logging Service**: http://localhost:5008
- **Search Service**: http://localhost:5009
- **GIS Service**: http://localhost:5010

### Infrastructure Services
- **SQL Server**: localhost:1433 (sa/YourStrong@Passw0rd)
- **PostgreSQL+PostGIS**: localhost:5432 (postgres/postgres)
- **Redis**: localhost:6379
- **RabbitMQ**: http://localhost:15672 (admin/admin)
- **MinIO Console**: http://localhost:9001 (minioadmin/minioadmin)
- **Elasticsearch**: http://localhost:9200

## Database Initialization

The databases will be automatically created when services first connect. To initialize with seed data:

```bash
# Connect to SQL Server and run migrations
docker exec -it axdd-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd"

# Connect to PostgreSQL
docker exec -it axdd-postgres-gis psql -U postgres -d gisdb
```

## Data Persistence

All data is persisted using Docker volumes:
- `sqlserver-data`: SQL Server databases
- `postgres-data`: PostgreSQL databases
- `redis-data`: Redis cache
- `rabbitmq-data`: RabbitMQ messages
- `minio-data`: Object storage files
- `elasticsearch-data`: Search indices

To view volumes:
```bash
docker volume ls | grep axdd
```

## Common Operations

### Stop All Services
```bash
docker-compose down
```

### Stop and Remove Volumes (Delete All Data)
```bash
docker-compose down -v
```

### Rebuild Specific Service
```bash
docker-compose build auth-api
docker-compose up -d auth-api
```

### Scale Services
```bash
docker-compose up -d --scale enterprise-api=3
```

### View Resource Usage
```bash
docker stats
```

## Troubleshooting

### Service Won't Start

Check logs:
```bash
docker-compose logs <service-name>
```

Check if port is already in use:
```bash
netstat -ano | findstr :<port>  # Windows
lsof -i :<port>                  # Linux/Mac
```

### Database Connection Issues

Verify SQL Server is healthy:
```bash
docker exec -it axdd-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT @@VERSION"
```

Verify PostgreSQL is healthy:
```bash
docker exec -it axdd-postgres-gis pg_isready -U postgres
```

### Out of Memory

Increase Docker memory limit in Docker Desktop settings or:
```bash
# Stop some non-essential services
docker-compose stop elasticsearch
docker-compose stop search-api
```

### Network Issues

Recreate the network:
```bash
docker-compose down
docker network prune
docker-compose up -d
```

## Development Mode

For development with hot reload:

```bash
# Start only infrastructure services
docker-compose up -d sqlserver postgres-gis redis rabbitmq minio elasticsearch

# Run services locally with `dotnet run` or `dotnet watch`
```

## Production Considerations

For production deployment:

1. **Change Default Passwords**: Update all default passwords in docker-compose.yml
2. **Use Environment Files**: Store sensitive configuration in `.env` files
3. **Enable HTTPS**: Configure SSL certificates
4. **Resource Limits**: Add CPU and memory limits to services
5. **Backup Strategy**: Implement regular volume backups
6. **Monitoring**: Add health monitoring and logging aggregation
7. **Load Balancing**: Use a reverse proxy like Nginx or Traefik

Example with resource limits:
```yaml
services:
  auth-api:
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
        reservations:
          cpus: '0.25'
          memory: 256M
```

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                         Client Layer                         │
│   ┌──────────────────┐        ┌─────────────────────┐       │
│   │  Admin Portal    │        │ Enterprise Portal   │       │
│   │   (MVC :8080)    │        │   (Angular :4200)   │       │
│   └────────┬─────────┘        └──────────┬──────────┘       │
└────────────┼────────────────────────────┼──────────────────┘
             │                            │
             └────────────┬───────────────┘
                          │
┌─────────────────────────┼─────────────────────────────────┐
│                    ┌────▼─────┐                            │
│                    │   API    │                            │
│                    │ Gateway  │                            │
│                    │  :5000   │                            │
│                    └────┬─────┘                            │
│          ┌──────────────┼──────────────┐                   │
│          │              │              │                   │
│    ┌─────▼─────┐  ┌────▼────┐  ┌─────▼─────┐             │
│    │   Auth    │  │Enterprise│  │Investment │             │
│    │   :5001   │  │  :5003   │  │   :5004   │             │
│    └───────────┘  └──────────┘  └───────────┘             │
│    ┌───────────┐  ┌──────────┐  ┌───────────┐             │
│    │MasterData │  │FileManager│  │  Report   │             │
│    │   :5002   │  │  :5005   │  │   :5006   │             │
│    └───────────┘  └──────────┘  └───────────┘             │
│    ┌───────────┐  ┌──────────┐  ┌───────────┐             │
│    │Notification│  │ Logging  │  │  Search   │             │
│    │   :5007   │  │  :5008   │  │   :5009   │             │
│    └───────────┘  └──────────┘  └───────────┘             │
│    ┌───────────┐                                           │
│    │    GIS    │                                           │
│    │   :5010   │                                           │
│    └───────────┘                                           │
└────────────────────────────────────────────────────────────┘
             │
┌────────────┼─────────────────────────────────────────────┐
│            │        Infrastructure Layer                  │
│  ┌─────────▼────────┐  ┌──────────────┐                  │
│  │   SQL Server     │  │ PostgreSQL   │                  │
│  │     :1433        │  │  + PostGIS   │                  │
│  └──────────────────┘  │    :5432     │                  │
│  ┌──────────────────┐  └──────────────┘                  │
│  │     Redis        │  ┌──────────────┐                  │
│  │     :6379        │  │  RabbitMQ    │                  │
│  └──────────────────┘  │ :5672/:15672 │                  │
│  ┌──────────────────┐  └──────────────┘                  │
│  │     MinIO        │  ┌──────────────┐                  │
│  │  :9000/:9001     │  │Elasticsearch │                  │
│  └──────────────────┘  │    :9200     │                  │
│                        └──────────────┘                  │
└──────────────────────────────────────────────────────────┘
```

## Support

For issues or questions:
- Check logs: `docker-compose logs <service-name>`
- Review documentation in `/docs` folder
- Create an issue on GitHub

## License

[Your License Here]
