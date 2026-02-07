using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Report template DTO
/// </summary>
public class ReportTemplateDto
{
    public Guid Id { get; set; }
    public ReportType ReportType { get; set; }
    public string ReportTypeName { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string FieldsJson { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
