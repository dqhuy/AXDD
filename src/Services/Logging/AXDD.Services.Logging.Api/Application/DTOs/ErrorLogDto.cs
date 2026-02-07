using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Application.DTOs;

/// <summary>
/// DTO for error log
/// </summary>
public class ErrorLogDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public ErrorSeverity Severity { get; set; }
    public Guid? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? ExceptionType { get; set; }
    public bool IsResolved { get; set; }
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Resolution { get; set; }
    public string? CorrelationId { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// Request DTO for creating an error log
/// </summary>
public class CreateErrorLogRequest
{
    public string ServiceName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Medium;
    public Guid? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? ExceptionType { get; set; }
    public string? CorrelationId { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// Request DTO for resolving an error
/// </summary>
public class ResolveErrorRequest
{
    public string Resolution { get; set; } = string.Empty;
    public Guid ResolvedBy { get; set; }
}
