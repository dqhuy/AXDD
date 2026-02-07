# AXDD Logging Service

A comprehensive logging service for the AXDD platform with CRUD operations for audit trails, user activities, error tracking, and performance monitoring.

## Features

### 🔍 Audit Logging
- Complete audit trail for all system activities
- Track user actions, API calls, and system events
- Correlation ID support for tracing requests across microservices
- Advanced filtering and search capabilities
- Automatic cleanup of old logs

### 👤 User Activity Tracking
- Track user login/logout events
- Monitor CRUD operations on resources
- Capture file uploads/downloads
- Track search activities and data exports
- Resource-level activity history

### ❌ Error Tracking
- Log errors with severity levels (Low, Medium, High, Critical)
- Track error resolution status
- Stack trace and exception details
- Service-level error aggregation
- Critical error monitoring

### ⚡ Performance Monitoring
- Track API endpoint response times
- Monitor slow requests (configurable threshold)
- CPU and memory usage metrics
- Service-level statistics and analytics
- Request success/error rates

### 📊 Dashboard
- Real-time system overview
- Logs and errors by service
- Active user statistics
- Performance metrics and slow endpoints
- Hourly activity charts

## API Endpoints

### Audit Logs (`/api/v1/logs/audit`)
- `GET /` - Get audit logs with filtering and pagination
- `GET /{id}` - Get audit log by ID
- `POST /` - Create audit log entry
- `GET /user/{userId}` - Get logs by user
- `GET /service/{serviceName}` - Get logs by service
- `GET /trace/{correlationId}` - Trace requests across services
- `DELETE /cleanup?olderThanDays={days}` - Clean up old logs

### User Activities (`/api/v1/logs/activities`)
- `GET /` - Get all activities with filtering
- `GET /user/{userId}` - Get user activities
- `GET /recent` - Get recent activities
- `GET /resource/{resourceType}/{resourceId}` - Get activities by resource
- `POST /` - Log user activity

### Error Logs (`/api/v1/logs/errors`)
- `GET /` - Get errors with filtering
- `GET /{id}` - Get error by ID
- `POST /` - Log error
- `GET /unresolved` - Get unresolved errors
- `PUT /{id}/resolve` - Resolve error
- `GET /service/{serviceName}` - Get errors by service
- `GET /critical` - Get critical errors

### Performance Logs (`/api/v1/logs/performance`)
- `GET /` - Get performance logs
- `POST /` - Log performance metrics
- `GET /statistics?serviceName={name}` - Get service statistics
- `GET /slow?thresholdMs={ms}` - Get slow requests

### Dashboard (`/api/v1/logs/dashboard`)
- `GET /summary` - Get dashboard summary with statistics and charts

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- SQL Server (2019 or later)

### Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LogDatabase": "Server=localhost;Database=AXDD_Logging;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "LoggingSettings": {
    "RetentionDays": 90,
    "PerformanceThresholdMs": 1000,
    "MaxLogSize": 10000,
    "EnableRequestBodyLogging": false,
    "EnableResponseBodyLogging": false
  }
}
```

### Run the Service

```bash
# Navigate to the project directory
cd src/Services/Logging/AXDD.Services.Logging.Api

# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update

# Run the service
dotnet run
```

The service will be available at `https://localhost:5001` (or the port configured in launchSettings.json).

### Access Swagger Documentation

Navigate to `https://localhost:5001/swagger` to explore the API endpoints.

## Usage Examples

### 1. Create an Audit Log

```bash
POST /api/v1/logs/audit
Content-Type: application/json

{
  "level": "Info",
  "userId": "11111111-1111-1111-1111-111111111111",
  "username": "admin@axdd.com",
  "userRole": "Administrator",
  "serviceName": "Enterprise.Api",
  "actionName": "CreateEnterprise",
  "entityType": "Enterprise",
  "entityId": "22222222-2222-2222-2222-222222222222",
  "httpMethod": "POST",
  "requestPath": "/api/v1/enterprises",
  "ipAddress": "192.168.1.100",
  "statusCode": 201,
  "durationMs": 245,
  "message": "Enterprise created successfully",
  "correlationId": "abc-123-def"
}
```

### 2. Query Audit Logs with Filters

```bash
GET /api/v1/logs/audit?serviceName=Enterprise.Api&level=Error&dateFrom=2024-01-01&pageNumber=1&pageSize=50
```

### 3. Trace Request Across Services

```bash
GET /api/v1/logs/audit/trace/abc-123-def
```

This returns all log entries with the same correlation ID, allowing you to trace a request as it flows through multiple microservices.

### 4. Log User Activity

```bash
POST /api/v1/logs/activities
Content-Type: application/json

{
  "userId": "11111111-1111-1111-1111-111111111111",
  "username": "admin@axdd.com",
  "activityType": "Create",
  "activityDescription": "Created new enterprise: ABC Manufacturing",
  "ipAddress": "192.168.1.100",
  "resourceType": "Enterprise",
  "resourceId": "22222222-2222-2222-2222-222222222222"
}
```

### 5. Log Performance Metrics

```bash
POST /api/v1/logs/performance
Content-Type: application/json

{
  "serviceName": "Enterprise.Api",
  "endpointName": "/api/v1/enterprises",
  "durationMs": 245,
  "memoryUsedMB": 128.5,
  "cpuUsagePercent": 35.2,
  "requestCount": 1,
  "successCount": 1,
  "errorCount": 0,
  "httpMethod": "POST",
  "statusCode": 201
}
```

### 6. Get Service Statistics

```bash
GET /api/v1/logs/performance/statistics?serviceName=Enterprise.Api&startDate=2024-01-01&endDate=2024-01-31
```

Response:
```json
{
  "serviceName": "Enterprise.Api",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-01-31T23:59:59Z",
  "totalRequests": 1250,
  "successCount": 1200,
  "errorCount": 50,
  "averageDurationMs": 234.5,
  "minDurationMs": 45,
  "maxDurationMs": 2150,
  "errorRate": 4.0
}
```

### 7. Get Dashboard Summary

```bash
GET /api/v1/logs/dashboard/summary
```

Response:
```json
{
  "totalLogsToday": 1523,
  "errorsToday": 42,
  "activeUsersToday": 87,
  "averageResponseTimeMs": 245.6,
  "criticalErrorsUnresolved": 3,
  "logsByService": [
    {
      "serviceName": "Enterprise.Api",
      "logCount": 523,
      "errorCount": 12
    }
  ],
  "logsByHour": [
    {
      "hour": 9,
      "logCount": 145
    }
  ],
  "topUsers": [
    {
      "userId": "11111111-1111-1111-1111-111111111111",
      "username": "admin@axdd.com",
      "activityCount": 234
    }
  ],
  "slowestEndpoints": [
    {
      "serviceName": "Search.Api",
      "endpointName": "/api/v1/search",
      "averageDurationMs": 1250.5,
      "requestCount": 89
    }
  ]
}
```

## Log Cleanup

The service includes a background job that automatically deletes audit logs older than the configured retention period.

**Configuration:**
```json
{
  "LoggingSettings": {
    "RetentionDays": 90
  }
}
```

The cleanup job runs once daily. You can also manually trigger cleanup:

```bash
DELETE /api/v1/logs/audit/cleanup?olderThanDays=90
```

## Database Schema

### Tables

#### AuditLogs
- Stores complete audit trail
- Indexed on: Timestamp, UserId, ServiceName, Level, CorrelationId
- Partitioned by date for large volumes (optional)

#### UserActivityLogs
- Tracks user activities
- Indexed on: UserId, Timestamp, ActivityType, ResourceType+ResourceId

#### ErrorLogs
- Stores error information
- Indexed on: Timestamp, ServiceName, Severity, IsResolved

#### PerformanceLogs
- Performance metrics
- Indexed on: Timestamp, ServiceName, DurationMs, ServiceName+EndpointName

## Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `RetentionDays` | Days to keep logs before cleanup | 90 |
| `PerformanceThresholdMs` | Threshold for slow requests | 1000 |
| `MaxLogSize` | Maximum log entry size | 10000 |
| `EnableRequestBodyLogging` | Log request bodies (caution: may contain sensitive data) | false |
| `EnableResponseBodyLogging` | Log response bodies (can be large) | false |

## Best Practices

### Security
- **Never log passwords, tokens, or sensitive data**
- Set `EnableRequestBodyLogging` and `EnableResponseBodyLogging` to `false` in production
- Implement proper authentication/authorization for the logging API
- Use HTTPS in production

### Performance
- This service handles high write volumes - ensure adequate database resources
- Consider asynchronous/background processing for log inserts
- Use appropriate retention periods to manage database size
- Enable database partitioning for very large log volumes

### Correlation IDs
- Always pass correlation IDs when making service-to-service calls
- This enables tracing requests across the entire microservices architecture
- Use the same correlation ID for all related operations

### Error Resolution
- Mark errors as resolved when fixed
- Document resolutions for future reference
- Monitor unresolved critical errors

## Integration with Other Services

### Client Libraries

Other services should integrate with the Logging Service using HTTP clients:

```csharp
// Example: Log an audit entry from another service
public async Task LogAuditAsync(CreateAuditLogRequest request)
{
    using var client = new HttpClient();
    client.BaseAddress = new Uri("https://logging-service:5001");
    
    var response = await client.PostAsJsonAsync("/api/v1/logs/audit", request);
    response.EnsureSuccessStatusCode();
}
```

### Middleware Integration

Consider creating middleware to automatically log requests:

```csharp
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _clientFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        // Log to Logging Service
        var auditLog = new CreateAuditLogRequest
        {
            // ... populate from context
            DurationMs = stopwatch.ElapsedMilliseconds
        };
        
        await LogToLoggingServiceAsync(auditLog);
    }
}
```

## Monitoring & Alerts

Monitor the following metrics:
- Critical errors (unresolved count)
- Slow requests (above threshold)
- Error rates by service
- Database size and growth rate
- Failed log writes

## Health Checks

The service exposes a health check endpoint:

```bash
GET /health
```

This checks:
- Database connectivity
- Service availability

## Development

### Running Tests

```bash
dotnet test
```

### Adding Migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Architecture

The service follows clean architecture principles:

```
├── Application/          # Business logic layer
│   ├── Services/        # Service implementations
│   ├── DTOs/           # Data transfer objects
│   └── Validators/     # Input validation
├── Domain/              # Domain entities and interfaces
│   ├── Entities/       # Domain entities
│   ├── Enums/         # Enumerations
│   └── Exceptions/    # Domain exceptions
├── Infrastructure/      # Data access layer
│   ├── Data/          # DbContext and migrations
│   ├── Repositories/  # Repository implementations (if needed)
│   └── HostedServices/ # Background services
└── Controllers/        # API controllers
```

## Troubleshooting

### Database Connection Issues
- Verify connection string in `appsettings.json`
- Ensure SQL Server is running
- Check firewall settings

### High Database Size
- Adjust `RetentionDays` to a lower value
- Run manual cleanup: `DELETE /api/v1/logs/audit/cleanup?olderThanDays=30`
- Consider database partitioning

### Slow Performance
- Add appropriate indexes (already configured)
- Increase database resources
- Consider read replicas for queries
- Implement caching for dashboard queries

## License

Copyright © 2024 AXDD Platform

## Support

For issues and questions, please contact the development team.
