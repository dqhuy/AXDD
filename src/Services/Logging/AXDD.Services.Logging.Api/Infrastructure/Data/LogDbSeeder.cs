using AXDD.Services.Logging.Api.Domain.Entities;
using AXDD.Services.Logging.Api.Domain.Enums;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Infrastructure.Data;

/// <summary>
/// Seeds sample data into the logging database
/// </summary>
public static class LogDbSeeder
{
    /// <summary>
    /// Seeds sample log data
    /// </summary>
    public static async Task SeedAsync(LogDbContext context)
    {
        // Check if data already exists
        if (await context.AuditLogs.AnyAsync())
        {
            return;
        }

        var sampleUserId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var sampleUserId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var correlationId = Guid.NewGuid().ToString();

        // Sample Audit Logs
        var auditLogs = new List<AuditLog>
        {
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-5),
                Level = AuditLogLevel.Info,
                UserId = sampleUserId1,
                Username = "admin@axdd.com",
                UserRole = "Administrator",
                ServiceName = "Enterprise.Api",
                ActionName = "CreateEnterprise",
                EntityType = "Enterprise",
                EntityId = Guid.NewGuid(),
                HttpMethod = "POST",
                RequestPath = "/api/v1/enterprises",
                IpAddress = "192.168.1.100",
                StatusCode = 201,
                DurationMs = 245,
                Message = "Enterprise created successfully",
                CorrelationId = correlationId
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-4),
                Level = AuditLogLevel.Info,
                UserId = sampleUserId2,
                Username = "user@axdd.com",
                UserRole = "User",
                ServiceName = "Search.Api",
                ActionName = "SearchEnterprises",
                HttpMethod = "GET",
                RequestPath = "/api/v1/search",
                IpAddress = "192.168.1.101",
                StatusCode = 200,
                DurationMs = 150,
                Message = "Search completed successfully"
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-3),
                Level = AuditLogLevel.Warning,
                UserId = sampleUserId1,
                Username = "admin@axdd.com",
                UserRole = "Administrator",
                ServiceName = "MasterData.Api",
                ActionName = "UpdateIndustry",
                EntityType = "Industry",
                EntityId = Guid.NewGuid(),
                HttpMethod = "PUT",
                RequestPath = "/api/v1/industries/abc-123",
                IpAddress = "192.168.1.100",
                StatusCode = 200,
                DurationMs = 320,
                Message = "Industry updated with validation warnings"
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-2),
                Level = AuditLogLevel.Error,
                UserId = sampleUserId2,
                Username = "user@axdd.com",
                UserRole = "User",
                ServiceName = "FileManager.Api",
                ActionName = "UploadFile",
                HttpMethod = "POST",
                RequestPath = "/api/v1/files",
                IpAddress = "192.168.1.101",
                StatusCode = 500,
                DurationMs = 1250,
                Message = "File upload failed",
                ExceptionMessage = "Storage service unavailable"
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-1),
                Level = AuditLogLevel.Info,
                UserId = sampleUserId1,
                Username = "admin@axdd.com",
                UserRole = "Administrator",
                ServiceName = "Auth.Api",
                ActionName = "Login",
                HttpMethod = "POST",
                RequestPath = "/api/v1/auth/login",
                IpAddress = "192.168.1.100",
                StatusCode = 200,
                DurationMs = 180,
                Message = "User logged in successfully"
            }
        };

        // Sample User Activity Logs
        var userActivityLogs = new List<UserActivityLog>
        {
            new()
            {
                UserId = sampleUserId1,
                Username = "admin@axdd.com",
                ActivityType = ActivityType.Login,
                ActivityDescription = "User logged in to the system",
                Timestamp = DateTime.UtcNow.AddHours(-6),
                IpAddress = "192.168.1.100",
                DeviceInfo = "Chrome 120 on Windows 10"
            },
            new()
            {
                UserId = sampleUserId1,
                Username = "admin@axdd.com",
                ActivityType = ActivityType.Create,
                ActivityDescription = "Created new enterprise: ABC Manufacturing",
                Timestamp = DateTime.UtcNow.AddHours(-5),
                IpAddress = "192.168.1.100",
                ResourceType = "Enterprise",
                ResourceId = Guid.NewGuid()
            },
            new()
            {
                UserId = sampleUserId2,
                Username = "user@axdd.com",
                ActivityType = ActivityType.Login,
                ActivityDescription = "User logged in to the system",
                Timestamp = DateTime.UtcNow.AddHours(-4),
                IpAddress = "192.168.1.101",
                DeviceInfo = "Firefox 119 on macOS"
            },
            new()
            {
                UserId = sampleUserId2,
                Username = "user@axdd.com",
                ActivityType = ActivityType.Search,
                ActivityDescription = "Searched for enterprises in Da Nang",
                Timestamp = DateTime.UtcNow.AddHours(-4),
                IpAddress = "192.168.1.101"
            },
            new()
            {
                UserId = sampleUserId1,
                Username = "admin@axdd.com",
                ActivityType = ActivityType.Update,
                ActivityDescription = "Updated industry classification",
                Timestamp = DateTime.UtcNow.AddHours(-3),
                IpAddress = "192.168.1.100",
                ResourceType = "Industry",
                ResourceId = Guid.NewGuid(),
                OldValue = "{\"code\":\"C101\",\"name\":\"Old Name\"}",
                NewValue = "{\"code\":\"C101\",\"name\":\"Updated Name\"}"
            },
            new()
            {
                UserId = sampleUserId2,
                Username = "user@axdd.com",
                ActivityType = ActivityType.Upload,
                ActivityDescription = "Attempted to upload file",
                Timestamp = DateTime.UtcNow.AddHours(-2),
                IpAddress = "192.168.1.101"
            },
            new()
            {
                UserId = sampleUserId2,
                Username = "user@axdd.com",
                ActivityType = ActivityType.View,
                ActivityDescription = "Viewed enterprise details",
                Timestamp = DateTime.UtcNow.AddMinutes(-30),
                IpAddress = "192.168.1.101",
                ResourceType = "Enterprise",
                ResourceId = Guid.NewGuid()
            }
        };

        // Sample Error Logs
        var errorLogs = new List<ErrorLog>
        {
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-2),
                ServiceName = "FileManager.Api",
                ErrorMessage = "Storage service unavailable",
                StackTrace = "at FileManager.Services.StorageService.UploadAsync()\nat FileManager.Controllers.FilesController.Upload()",
                Severity = ErrorSeverity.High,
                UserId = sampleUserId2,
                RequestPath = "/api/v1/files",
                ExceptionType = "StorageServiceException"
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-8),
                ServiceName = "Enterprise.Api",
                ErrorMessage = "Database connection timeout",
                StackTrace = "at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync()",
                Severity = ErrorSeverity.Critical,
                RequestPath = "/api/v1/enterprises",
                ExceptionType = "TimeoutException",
                IsResolved = true,
                ResolvedBy = sampleUserId1,
                ResolvedAt = DateTime.UtcNow.AddHours(-7),
                Resolution = "Database server was restarted. Connection pool refreshed."
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddDays(-1),
                ServiceName = "Search.Api",
                ErrorMessage = "Index not found",
                Severity = ErrorSeverity.Medium,
                RequestPath = "/api/v1/search",
                ExceptionType = "SearchIndexException",
                IsResolved = true,
                ResolvedBy = sampleUserId1,
                ResolvedAt = DateTime.UtcNow.AddDays(-1).AddHours(1),
                Resolution = "Search index was rebuilt successfully"
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-45),
                ServiceName = "MasterData.Api",
                ErrorMessage = "Invalid data format",
                Severity = ErrorSeverity.Low,
                UserId = sampleUserId1,
                RequestPath = "/api/v1/industries",
                ExceptionType = "ValidationException"
            }
        };

        // Sample Performance Logs
        var performanceLogs = new List<PerformanceLog>
        {
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-5),
                ServiceName = "Enterprise.Api",
                EndpointName = "/api/v1/enterprises",
                DurationMs = 245,
                MemoryUsedMB = 128.5,
                CpuUsagePercent = 35.2,
                RequestCount = 1,
                SuccessCount = 1,
                ErrorCount = 0,
                HttpMethod = "POST",
                StatusCode = 201
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-4),
                ServiceName = "Search.Api",
                EndpointName = "/api/v1/search",
                DurationMs = 150,
                MemoryUsedMB = 95.3,
                CpuUsagePercent = 25.8,
                RequestCount = 1,
                SuccessCount = 1,
                ErrorCount = 0,
                HttpMethod = "GET",
                StatusCode = 200
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-3),
                ServiceName = "MasterData.Api",
                EndpointName = "/api/v1/industries/{id}",
                DurationMs = 320,
                MemoryUsedMB = 110.7,
                CpuUsagePercent = 40.1,
                RequestCount = 1,
                SuccessCount = 1,
                ErrorCount = 0,
                HttpMethod = "PUT",
                StatusCode = 200
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-2),
                ServiceName = "FileManager.Api",
                EndpointName = "/api/v1/files",
                DurationMs = 1250,
                MemoryUsedMB = 256.8,
                CpuUsagePercent = 75.5,
                RequestCount = 1,
                SuccessCount = 0,
                ErrorCount = 1,
                HttpMethod = "POST",
                StatusCode = 500
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddHours(-1),
                ServiceName = "Auth.Api",
                EndpointName = "/api/v1/auth/login",
                DurationMs = 180,
                MemoryUsedMB = 75.2,
                CpuUsagePercent = 20.3,
                RequestCount = 1,
                SuccessCount = 1,
                ErrorCount = 0,
                HttpMethod = "POST",
                StatusCode = 200
            },
            new()
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-30),
                ServiceName = "Enterprise.Api",
                EndpointName = "/api/v1/enterprises",
                DurationMs = 2150,
                MemoryUsedMB = 185.4,
                CpuUsagePercent = 60.7,
                RequestCount = 1,
                SuccessCount = 1,
                ErrorCount = 0,
                HttpMethod = "GET",
                StatusCode = 200
            }
        };

        // Add all sample data
        await context.AuditLogs.AddRangeAsync(auditLogs);
        await context.UserActivityLogs.AddRangeAsync(userActivityLogs);
        await context.ErrorLogs.AddRangeAsync(errorLogs);
        await context.PerformanceLogs.AddRangeAsync(performanceLogs);

        await context.SaveChangesAsync();
    }
}
