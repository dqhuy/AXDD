# Quick Start Guide - AXDD Logging Service

## 🚀 5-Minute Setup

### 1. Prerequisites
- .NET 9.0 SDK installed
- SQL Server running (local or remote)

### 2. Configure Database

Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "LogDatabase": "Server=localhost;Database=AXDD_Logging_Dev;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. Run the Service

```bash
cd src/Services/Logging/AXDD.Services.Logging.Api
dotnet run
```

The service will:
- ✅ Create the database if it doesn't exist
- ✅ Run migrations automatically
- ✅ Seed sample data (30+ log entries)
- ✅ Start listening on https://localhost:5001

### 4. Explore the API

Open your browser: **https://localhost:5001/swagger**

### 5. Try Your First Request

#### Get Dashboard Summary
```bash
curl https://localhost:5001/api/v1/logs/dashboard/summary
```

#### View Audit Logs
```bash
curl "https://localhost:5001/api/v1/logs/audit?pageNumber=1&pageSize=10"
```

## 📝 Common Tasks

### Log an Audit Entry
```bash
curl -X POST https://localhost:5001/api/v1/logs/audit \
  -H "Content-Type: application/json" \
  -d '{
    "level": "Info",
    "serviceName": "MyService",
    "message": "Test log entry",
    "statusCode": 200
  }'
```

### View Errors
```bash
curl https://localhost:5001/api/v1/logs/errors?isResolved=false
```

### Get Performance Stats
```bash
curl "https://localhost:5001/api/v1/logs/performance/statistics?serviceName=Enterprise.Api"
```

### Track User Activity
```bash
curl -X POST https://localhost:5001/api/v1/logs/activities \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "11111111-1111-1111-1111-111111111111",
    "username": "testuser",
    "activityType": "Create",
    "activityDescription": "Created test resource"
  }'
```

## 🎯 Key Features to Try

1. **Trace Requests**: Use correlation IDs to track requests across services
2. **Filter Logs**: Use query parameters for advanced filtering
3. **Monitor Performance**: Track slow endpoints with `/performance/slow`
4. **Error Tracking**: Monitor critical errors with `/errors/critical`
5. **Dashboard**: Get real-time overview with `/dashboard/summary`

## 🔧 Configuration

### Adjust Retention Period
In `appsettings.json`:
```json
{
  "LoggingSettings": {
    "RetentionDays": 30  // Keep logs for 30 days
  }
}
```

### Enable Request/Response Logging (Development Only)
```json
{
  "LoggingSettings": {
    "EnableRequestBodyLogging": true,
    "EnableResponseBodyLogging": true
  }
}
```

## 📊 Sample Data

The service includes sample data:
- 5+ audit log entries
- 7+ user activity logs
- 4+ error logs (with resolutions)
- 6+ performance logs

Perfect for testing and demonstration!

## 🐛 Troubleshooting

### "Cannot connect to database"
- Check SQL Server is running
- Verify connection string in `appsettings.Development.json`

### "Port already in use"
- Change port in `Properties/launchSettings.json`
- Or kill the process using the port

### "Migration failed"
- Delete the database and rerun: `dotnet run`
- Or run manually: `dotnet ef database update`

## 📚 Next Steps

- Read the [full README](README.md) for detailed documentation
- Explore the [API endpoints](README.md#api-endpoints)
- Learn about [best practices](README.md#best-practices)
- Set up [monitoring and alerts](README.md#monitoring--alerts)

## 🎉 You're All Set!

The Logging Service is now running and ready to capture logs from all AXDD services.
