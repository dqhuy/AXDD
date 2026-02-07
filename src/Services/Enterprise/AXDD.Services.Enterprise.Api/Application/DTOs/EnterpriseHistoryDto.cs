using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Application.DTOs;

/// <summary>
/// DTO for enterprise history details
/// </summary>
public class EnterpriseHistoryDto
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public ChangeType ChangeType { get; set; }
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }
    public string? Details { get; set; }
}
