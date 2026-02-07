using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;

namespace AXDD.Services.Report.Api.Domain.Entities;

/// <summary>
/// Represents a template for report data structure
/// </summary>
public class ReportTemplate : AuditableEntity
{
    /// <summary>
    /// Gets or sets the type of report this template is for
    /// </summary>
    public ReportType ReportType { get; set; }

    /// <summary>
    /// Gets or sets the template name
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field definitions as JSON schema
    /// </summary>
    public string FieldsJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this template is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the template description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the template version number (hides base Version with new keyword)
    /// </summary>
    public new int Version { get; set; }
}
