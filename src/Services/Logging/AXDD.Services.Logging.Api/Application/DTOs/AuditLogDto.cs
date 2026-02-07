using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Application.DTOs;

/// <summary>
/// DTO for audit log
/// </summary>
public class AuditLogDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public AuditLogLevel Level { get; set; }
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? UserRole { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? ActionName { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public int? StatusCode { get; set; }
    public long? DurationMs { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? CorrelationId { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// Request DTO for creating an audit log
/// </summary>
public class CreateAuditLogRequest
{
    public AuditLogLevel Level { get; set; } = AuditLogLevel.Info;
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? UserRole { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? ActionName { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public int? StatusCode { get; set; }
    public long? DurationMs { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? CorrelationId { get; set; }
    public string? AdditionalData { get; set; }
}
