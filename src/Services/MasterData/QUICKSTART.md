# MasterData Service - Quick Start Guide

## 🚀 Quick Start (5 minutes)

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- *Optional:* Redis (for caching)

### Step 1: Configure Connection String

Edit `appsettings.json` or set environment variable:

```json
{
  "ConnectionStrings": {
    "MasterDataDatabase": "Server=(localdb)\\mssqllocaldb;Database=AXDD_MasterData;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Or use environment variable:
```bash
export ConnectionStrings__MasterDataDatabase="Server=localhost;Database=AXDD_MasterData;..."
```

### Step 2: Run the Service

```bash
cd src/Services/MasterData/AXDD.Services.MasterData.Api
dotnet run
```

**That's it!** 
- Database will be created automatically
- All seed data will be populated
- Service will start on https://localhost:5001

### Step 3: Test the API

Open browser: **https://localhost:5001/swagger**

Or use curl:

```bash
# Get all provinces
curl https://localhost:5001/api/v1/masterdata/administrativedivisions/provinces

# Search industry codes
curl "https://localhost:5001/api/v1/masterdata/industrycodes/search?q=food"

# Get project statuses
curl "https://localhost:5001/api/v1/masterdata/statuscodes?entityType=Project"
```

## 📊 What You Get

After first run, your database contains:
- ✅ 63 Vietnamese provinces
- ✅ 11 districts (Dong Nai)
- ✅ 5 industrial zones
- ✅ 50+ industry codes (VSIC)
- ✅ 6 certificate types
- ✅ 8 document types
- ✅ 13 status codes
- ✅ 6 system configurations

## 🔧 Optional: Enable Redis Caching

1. Install and start Redis:
   ```bash
   docker run -d -p 6379:6379 redis
   ```

2. Update `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Redis": "localhost:6379"
     }
   }
   ```

3. Restart the service

## 📚 Key Endpoints

| Purpose | Endpoint |
|---------|----------|
| All provinces | `GET /api/v1/masterdata/administrativedivisions/provinces` |
| Districts by province | `GET /api/v1/masterdata/administrativedivisions/provinces/{id}/districts` |
| Search industry codes | `GET /api/v1/masterdata/industrycodes/search?q={query}` |
| Status codes | `GET /api/v1/masterdata/statuscodes?entityType={type}` |
| Industrial zones | `GET /api/v1/masterdata/industrialzones` |
| Certificate types | `GET /api/v1/masterdata/certificatetypes` |
| Document types | `GET /api/v1/masterdata/documenttypes` |
| Configurations | `GET /api/v1/masterdata/configurations` |

## 🐛 Troubleshooting

### Database Connection Error
- Verify SQL Server is running
- Check connection string
- Ensure LocalDB is installed (SQL Server Express)

### Port Already in Use
Change port in `Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:5002;http://localhost:5003"
}
```

### Migration Issues
Recreate database:
```bash
dotnet ef database drop --force
dotnet run
```

## 📖 Learn More

- Full documentation: [README.md](README.md)
- Implementation details: [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
- API documentation: Run service and visit `/swagger`

## 💡 Pro Tips

1. **Caching**: Redis is optional but recommended for production
2. **Health Check**: Monitor service at `/health`
3. **Swagger**: Use `/swagger` for interactive API testing
4. **Seed Data**: Customize `Data/MasterDataSeeder.cs` for your needs
5. **Performance**: First request is slower (cache warming), subsequent requests are fast

## 🎯 Next Steps

1. Integrate with other AXDD services
2. Add authentication/authorization
3. Set up monitoring and logging
4. Configure for production environment
5. Add additional master data types as needed

---

**Ready to use!** The service is now providing reference data for your AXDD platform.
