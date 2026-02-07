namespace AXDD.Services.MasterData.Api.DTOs;

/// <summary>
/// Industrial zone DTO
/// </summary>
public record IndustrialZoneDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid ProvinceId { get; init; }
    public string? ProvinceName { get; init; }
    public Guid? DistrictId { get; init; }
    public string? DistrictName { get; init; }
    public decimal Area { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? EstablishedDate { get; init; }
    public string? ManagementUnit { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Request to create industrial zone
/// </summary>
public record CreateIndustrialZoneRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid ProvinceId { get; init; }
    public Guid? DistrictId { get; init; }
    public decimal Area { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? EstablishedDate { get; init; }
    public string? ManagementUnit { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Request to update industrial zone
/// </summary>
public record UpdateIndustrialZoneRequest
{
    public string Name { get; init; } = string.Empty;
    public Guid? DistrictId { get; init; }
    public decimal Area { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? EstablishedDate { get; init; }
    public string? ManagementUnit { get; init; }
    public string? Description { get; init; }
}
