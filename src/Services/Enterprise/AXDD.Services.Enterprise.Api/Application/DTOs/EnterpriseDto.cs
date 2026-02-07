using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Application.DTOs;

/// <summary>
/// DTO for enterprise details
/// </summary>
public class EnterpriseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string? ShortName { get; set; }
    public string IndustryCode { get; set; } = string.Empty;
    public string IndustryName { get; set; } = string.Empty;
    public Guid? IndustrialZoneId { get; set; }
    public string? IndustrialZoneName { get; set; }
    public EnterpriseStatus Status { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? Position { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateTime? RegisteredDate { get; set; }
    public decimal? RegisteredCapital { get; set; }
    public decimal? CharterCapital { get; set; }
    public int? TotalEmployees { get; set; }
    public int? VietnamEmployees { get; set; }
    public int? ForeignEmployees { get; set; }
    public string? ProductionCapacity { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public List<ContactPersonDto> Contacts { get; set; } = [];
    public List<EnterpriseLicenseDto> Licenses { get; set; } = [];
}

/// <summary>
/// DTO for enterprise summary in lists
/// </summary>
public class EnterpriseListDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string IndustryCode { get; set; } = string.Empty;
    public string IndustryName { get; set; } = string.Empty;
    public Guid? IndustrialZoneId { get; set; }
    public string? IndustrialZoneName { get; set; }
    public EnterpriseStatus Status { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? TotalEmployees { get; set; }
    public decimal? RegisteredCapital { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request DTO for creating an enterprise
/// </summary>
public class CreateEnterpriseRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string? ShortName { get; set; }
    public string IndustryCode { get; set; } = string.Empty;
    public string IndustryName { get; set; } = string.Empty;
    public Guid? IndustrialZoneId { get; set; }
    public string? IndustrialZoneName { get; set; }
    public EnterpriseStatus Status { get; set; } = EnterpriseStatus.Active;
    public string? LegalRepresentative { get; set; }
    public string? Position { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateTime? RegisteredDate { get; set; }
    public decimal? RegisteredCapital { get; set; }
    public decimal? CharterCapital { get; set; }
    public int? TotalEmployees { get; set; }
    public int? VietnamEmployees { get; set; }
    public int? ForeignEmployees { get; set; }
    public string? ProductionCapacity { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request DTO for updating an enterprise
/// </summary>
public class UpdateEnterpriseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string? ShortName { get; set; }
    public string IndustryCode { get; set; } = string.Empty;
    public string IndustryName { get; set; } = string.Empty;
    public Guid? IndustrialZoneId { get; set; }
    public string? IndustrialZoneName { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? Position { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateTime? RegisteredDate { get; set; }
    public decimal? RegisteredCapital { get; set; }
    public decimal? CharterCapital { get; set; }
    public int? TotalEmployees { get; set; }
    public int? VietnamEmployees { get; set; }
    public int? ForeignEmployees { get; set; }
    public string? ProductionCapacity { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request DTO for changing enterprise status
/// </summary>
public class ChangeStatusRequest
{
    public EnterpriseStatus NewStatus { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// DTO for enterprise statistics
/// </summary>
public class EnterpriseStatisticsDto
{
    public int TotalCount { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = [];
    public Dictionary<string, int> ByZone { get; set; } = [];
    public Dictionary<string, int> ByIndustry { get; set; } = [];
}
