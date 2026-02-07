# AXDD Logging Service - Technical Documentation

## Architecture Overview

The AXDD Logging Service is built using Clean Architecture principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                      API Controllers                         │
│  (HTTP Request Handling, Validation, Response Formatting)   │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                   Application Layer                          │
│  - Service Interfaces (IAuditLogService, etc.)              │
│  - Service Implementations (Business Logic)                  │
│  - DTOs (Data Transfer Objects)                             │
│  - Validators (Input Validation)                            │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                     Domain Layer                             │
│  - Entities (AuditLog, ErrorLog, etc.)                      │
│  - Enums (LogLevel, ActivityType, ErrorSeverity)            │
│  - Domain Exceptions                                         │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                 Infrastructure Layer                         │
│  - DbContext (Entity Framework Core)                        │
│  - Database Configurations                                   │
│  - Migrations                                               │
│  - Hosted Services (Background Jobs)                        │
└─────────────────────────────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                   SQL Server Database                        │
│  - AuditLogs Table                                          │
│  - UserActivityLogs Table                                    │
│  - ErrorLogs Table                                          │
│  - PerformanceLogs Table                                    │
└─────────────────────────────────────────────────────────────┘
```

## Technology Stack

- **Framework**: .NET 9.0
- **Language**: C# 14
- **Web Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server 2019+
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Validation**: FluentValidation
- **Dependency Injection**: Built-in ASP.NET Core DI

## Database Schema

### AuditLogs Table

| Column | Type | Description | Indexed |
|--------|------|-------------|---------|
| Id | UNIQUEIDENTIFIER | Primary key | ✓ (PK) |
| Timestamp | DATETIME2 | When the log was created | ✓ |
| Level | INT | Log level (enum) | ✓ |
| UserId | UNIQUEIDENTIFIER | User who performed action | ✓ |
| Username | NVARCHAR(100) | Username | |
| UserRole | NVARCHAR(50) | User role | |
| ServiceName | NVARCHAR(100) | Service that generated log | ✓ |
| ActionName | NVARCHAR(100) | Action/method name | |
| EntityType | NVARCHAR(100) | Type of entity acted upon | |
| EntityId | UNIQUEIDENTIFIER | ID of entity acted upon | |
| HttpMethod | NVARCHAR(10) | HTTP method | |
| RequestPath | NVARCHAR(500) | Request path | |
| IpAddress | NVARCHAR(45) | IP address (IPv4/IPv6) | |
| UserAgent | NVARCHAR(500) | User agent string | |
| RequestBody | NVARCHAR(MAX) | Request body (optional) | |
| ResponseBody | NVARCHAR(MAX) | Response body (optional) | |
| StatusCode | INT | HTTP status code | |
| DurationMs | BIGINT | Duration in milliseconds | |
| Message | NVARCHAR(2000) | Log message | |
| ExceptionMessage | NVARCHAR(2000) | Exception message | |
| StackTrace | NVARCHAR(MAX) | Exception stack trace | |
| CorrelationId | NVARCHAR(100) | Correlation ID for tracing | ✓ |
| AdditionalData | NVARCHAR(MAX) | Additional JSON data | |

**Composite Index**: (Timestamp, ServiceName)

### UserActivityLogs Table

| Column | Type | Description | Indexed |
|--------|------|-------------|---------|
| Id | UNIQUEIDENTIFIER | Primary key | ✓ (PK) |
| UserId | UNIQUEIDENTIFIER | User ID | ✓ |
| Username | NVARCHAR(100) | Username | |
| ActivityType | INT | Activity type (enum) | ✓ |
| ActivityDescription | NVARCHAR(500) | Activity description | |
| Timestamp | DATETIME2 | When activity occurred | ✓ |
| IpAddress | NVARCHAR(45) | IP address | |
| DeviceInfo | NVARCHAR(500) | Device information | |
| ResourceType | NVARCHAR(100) | Type of resource | |
| ResourceId | UNIQUEIDENTIFIER | Resource ID | |
| OldValue | NVARCHAR(MAX) | Old value (for updates) | |
| NewValue | NVARCHAR(MAX) | New value (for updates) | |
| AdditionalData | NVARCHAR(MAX) | Additional JSON data | |

**Composite Index**: (ResourceType, ResourceId)

### ErrorLogs Table

| Column | Type | Description | Indexed |
|--------|------|-------------|---------|
| Id | UNIQUEIDENTIFIER | Primary key | ✓ (PK) |
| Timestamp | DATETIME2 | When error occurred | ✓ |
| ServiceName | NVARCHAR(100) | Service name | ✓ |
| ErrorMessage | NVARCHAR(2000) | Error message | |
| StackTrace | NVARCHAR(MAX) | Stack trace | |
| Severity | INT | Error severity (enum) | ✓ |
| UserId | UNIQUEIDENTIFIER | Associated user | |
| RequestPath | NVARCHAR(500) | Request path | |
| ExceptionType | NVARCHAR(200) | Exception type | |
| IsResolved | BIT | Resolution status | ✓ |
| ResolvedBy | UNIQUEIDENTIFIER | User who resolved | |
| ResolvedAt | DATETIME2 | When resolved | |
| Resolution | NVARCHAR(1000) | Resolution description | |
| CorrelationId | NVARCHAR(100) | Correlation ID | |
| AdditionalData | NVARCHAR(MAX) | Additional JSON data | |

**Composite Index**: (Severity, IsResolved)

### PerformanceLogs Table

| Column | Type | Description | Indexed |
|--------|------|-------------|---------|
| Id | UNIQUEIDENTIFIER | Primary key | ✓ (PK) |
| Timestamp | DATETIME2 | When logged | ✓ |
| ServiceName | NVARCHAR(100) | Service name | ✓ |
| EndpointName | NVARCHAR(200) | Endpoint name | |
| DurationMs | BIGINT | Duration in milliseconds | ✓ |
| MemoryUsedMB | FLOAT | Memory used (MB) | |
| CpuUsagePercent | FLOAT | CPU usage percentage | |
| RequestCount | INT | Request count | |
| SuccessCount | INT | Success count | |
| ErrorCount | INT | Error count | |
| HttpMethod | NVARCHAR(10) | HTTP method | |
| StatusCode | INT | HTTP status code | |
| AdditionalData | NVARCHAR(MAX) | Additional JSON data | |

**Composite Index**: (ServiceName, EndpointName)

## Service Layer

### IAuditLogService
Handles audit log operations including CRUD, filtering, and cleanup.

**Key Methods:**
- `CreateLogAsync`: Creates new audit log entry
- `GetLogsAsync`: Retrieves logs with advanced filtering
- `GetLogsByCorrelationIdAsync`: Traces requests across services
- `DeleteOldLogsAsync`: Cleanup old logs

### IUserActivityService
Manages user activity tracking.

**Key Methods:**
- `LogActivityAsync`: Records user activity
- `GetUserActivitiesAsync`: Retrieves user activities
- `GetRecentActivitiesAsync`: Gets recent activities
- `GetActivitiesByResourceAsync`: Activities by resource

### IErrorLogService
Handles error logging and resolution.

**Key Methods:**
- `LogErrorAsync`: Records error
- `GetErrorsAsync`: Retrieves errors with filtering
- `ResolveErrorAsync`: Marks error as resolved
- `GetUnresolvedErrorsAsync`: Gets unresolved errors

### IPerformanceLogService
Tracks performance metrics.

**Key Methods:**
- `LogPerformanceAsync`: Records performance metrics
- `GetPerformanceLogsAsync`: Retrieves performance logs
- `GetServiceStatisticsAsync`: Calculates service statistics
- `GetSlowRequestsAsync`: Identifies slow requests

### IDashboardService
Provides dashboard data and analytics.

**Key Methods:**
- `GetDashboardSummaryAsync`: Comprehensive dashboard data

## Background Services

### LogCleanupHostedService
Automatically deletes logs older than the configured retention period.

**Configuration:**
- Runs once per day
- Retention period: Configurable (default: 90 days)
- Startup delay: 1 minute

## Performance Considerations

### Write Optimization
- **High Volume**: This service handles many log writes
- **Async Operations**: All database operations are asynchronous
- **Bulk Inserts**: Consider implementing bulk insert for high-volume scenarios
- **Connection Pooling**: EF Core manages connection pooling

### Read Optimization
- **Indexes**: All frequently queried columns are indexed
- **Pagination**: All list endpoints support pagination
- **Filtering**: Server-side filtering reduces data transfer
- **Projections**: Use Select() to retrieve only needed columns

### Query Performance Tips
```csharp
// Good: Filter and paginate
var logs = await context.AuditLogs
    .Where(x => x.ServiceName == "Enterprise.Api")
    .OrderByDescending(x => x.Timestamp)
    .Skip(0).Take(50)
    .ToListAsync();

// Bad: Load all then filter
var allLogs = await context.AuditLogs.ToListAsync();
var filtered = allLogs.Where(x => x.ServiceName == "Enterprise.Api").ToList();
```

## Security Considerations

### Sensitive Data
- **Never log**: Passwords, tokens, API keys, credit card numbers
- **Request/Response Bodies**: Disabled by default
- **PII**: Be cautious with personally identifiable information

### Access Control
- Implement authentication/authorization on endpoints
- Consider role-based access (Admin, User, ReadOnly)
- Audit access to logs themselves

### Data Retention
- Configure appropriate retention periods
- Comply with data protection regulations (GDPR, etc.)
- Implement secure deletion

## Integration Patterns

### 1. Direct HTTP Calls
```csharp
public class LoggingClient
{
    private readonly HttpClient _httpClient;

    public LoggingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LogAuditAsync(CreateAuditLogRequest request)
    {
        await _httpClient.PostAsJsonAsync("/api/v1/logs/audit", request);
    }
}
```

### 2. Middleware Approach
```csharp
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            await LogToLoggingServiceAsync(context, stopwatch.ElapsedMilliseconds);
        }
    }
}
```

### 3. Message Queue (Future Enhancement)
For very high volumes, consider asynchronous logging via message queues:
- Use RabbitMQ or Azure Service Bus
- Decouple log writing from application flow
- Better resilience and throughput

## Correlation ID Pattern

Correlation IDs enable tracing requests across microservices:

```
Client Request → API Gateway → Service A → Service B → Service C
                      ↓             ↓           ↓           ↓
                 [corr-id-123]  [corr-id-123] [corr-id-123] [corr-id-123]
                      ↓             ↓           ↓           ↓
                 Logging Service ← ← ← ← ← ← ← ← ← ← ← ← ← ←
```

**Implementation:**
1. Generate correlation ID at API Gateway
2. Pass in HTTP headers: `X-Correlation-ID`
3. Include in all log entries
4. Query logs by correlation ID to see full request flow

## Error Handling

### Exception Types
- `LogNotFoundException`: When log entry not found (404)
- Standard ASP.NET Core exceptions are handled by middleware

### HTTP Status Codes
- `200 OK`: Successful retrieval
- `201 Created`: Successful creation
- `400 Bad Request`: Invalid input
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Monitoring & Observability

### Metrics to Monitor
1. **Log Volume**: Logs per second/minute
2. **Error Rate**: Percentage of error logs
3. **Critical Errors**: Count of unresolved critical errors
4. **Database Size**: Monitor growth rate
5. **Query Performance**: Slow query detection
6. **Service Health**: Uptime and availability

### Health Checks
- Database connectivity
- Disk space availability
- Memory usage

### Alerts
- Critical errors (unresolved > 5)
- Database size (> 80% capacity)
- Failed log writes
- Service unavailability

## Testing Strategy

### Unit Tests
- Service layer business logic
- DTO validation
- Domain entity behavior

### Integration Tests
- Database operations
- Controller endpoints
- Full request/response cycles

### Performance Tests
- High-volume log writing
- Concurrent read operations
- Query performance under load

## Deployment

### Prerequisites
- SQL Server database
- .NET 9.0 runtime
- Network connectivity between services

### Configuration
1. Set connection string
2. Configure retention policy
3. Adjust performance thresholds
4. Set log levels

### Migration
```bash
dotnet ef database update
```

### Running
```bash
dotnet run --urls "https://0.0.0.0:5001"
```

## Future Enhancements

1. **Real-time Notifications**: SignalR for live log streaming
2. **Message Queue Integration**: RabbitMQ for async logging
3. **Read Replicas**: Separate read/write databases
4. **Archival**: Move old logs to cold storage
5. **Advanced Analytics**: ML-based anomaly detection
6. **Elasticsearch Integration**: Full-text search capabilities
7. **GraphQL API**: Alternative query interface
8. **Multi-tenancy**: Tenant isolation

## Troubleshooting

### High Database Growth
- Reduce retention period
- Enable log sampling (log 1 in N requests)
- Archive old data

### Slow Queries
- Check index usage: `SET STATISTICS IO ON`
- Analyze execution plans
- Consider database partitioning

### Memory Issues
- Increase page size limits if needed
- Implement result streaming for large datasets
- Monitor memory usage

## References

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Serilog](https://serilog.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Changelog

### Version 1.0.0 (2024-02-07)
- Initial release
- Audit logging
- User activity tracking
- Error logging with resolution
- Performance monitoring
- Dashboard with statistics
- Automatic cleanup
- Sample data seeding
