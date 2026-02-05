# AXDD Quick Reference

## Services & Ports

| Service | Port | Swagger | Health Check |
|---------|------|---------|--------------|
| API Gateway | 5000 | - | http://localhost:5000/health |
| Auth | 5001 | http://localhost:5001/swagger | http://localhost:5001/health |
| MasterData | 5002 | http://localhost:5002/swagger | http://localhost:5002/health |
| Enterprise | 5003 | http://localhost:5003/swagger | http://localhost:5003/health |
| Investment | 5004 | http://localhost:5004/swagger | http://localhost:5004/health |
| FileManager | 5005 | http://localhost:5005/swagger | http://localhost:5005/health |
| Report | 5006 | http://localhost:5006/swagger | http://localhost:5006/health |

## Quick Commands

```bash
# Build entire solution
cd src && dotnet build

# Run tests
cd src && dotnet test

# Run with Docker
docker-compose up --build

# Run single service
cd src/Services/Auth/AXDD.Services.Auth.Api && dotnet run

# Clean build
cd src && dotnet clean && dotnet build

# Build script
./build.sh
```

## API Endpoints Quick Reference

### Auth Service
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Register

### MasterData Service
- `GET /api/masterdata/provinces` - Get provinces
- `GET /api/masterdata/industries` - Get industries

### Enterprise Service
- `GET /api/enterprise` - List enterprises (paged)
- `GET /api/enterprise/{id}` - Get enterprise by ID

### Investment Service
- `GET /api/investment/projects` - List projects

### FileManager Service
- `GET /api/filemanager/files` - List files
- `POST /api/filemanager/upload` - Upload file

### Report Service
- `GET /api/report/summary` - Get summary
- `GET /api/report/export?format=pdf` - Export report

## Common Issues & Solutions

### Port Already in Use
```bash
# Change port in Properties/launchSettings.json
# Or kill process: lsof -ti:5001 | xargs kill -9
```

### Build Errors
```bash
cd src
dotnet clean
dotnet restore
dotnet build
```

### Docker Issues
```bash
docker-compose down
docker-compose up --build
```

## Project Structure

```
src/
├── BuildingBlocks/
│   ├── Common/          # DTOs, Extensions, Middleware
│   ├── Domain/          # Entities, Value Objects
│   └── Infrastructure/  # Repositories, EF Core
├── ApiGateway/          # YARP Gateway
├── Services/
│   ├── Auth/
│   ├── MasterData/
│   ├── Enterprise/
│   ├── Investment/
│   ├── FileManager/
│   └── Report/
└── Tests/
    └── Unit/
```

## Useful Links

- [Architecture Docs](docs/architecture.md)
- [Development Guide](docs/development-guide.md)
- [Docker Compose File](docker-compose.yml)
