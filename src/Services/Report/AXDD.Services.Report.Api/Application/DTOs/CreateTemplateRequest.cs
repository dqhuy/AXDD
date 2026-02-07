using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Application.DTOs;

/// <summary>
/// Request to create a new report template
/// </summary>
public class CreateTemplateRequest
{
    public ReportType ReportType { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string FieldsJson { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; } = 1;
}
