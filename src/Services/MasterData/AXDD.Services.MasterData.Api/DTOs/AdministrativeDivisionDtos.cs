namespace AXDD.Services.MasterData.Api.DTOs;

/// <summary>
/// Province DTO
/// </summary>
public record ProvinceDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

/// <summary>
/// District DTO
/// </summary>
public record DistrictDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid ProvinceId { get; init; }
    public string? ProvinceName { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// Ward DTO
/// </summary>
public record WardDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid DistrictId { get; init; }
    public string? DistrictName { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// Full address DTO
/// </summary>
public record FullAddressDto
{
    public WardDto Ward { get; init; } = null!;
    public DistrictDto District { get; init; } = null!;
    public ProvinceDto Province { get; init; } = null!;
    public string FormattedAddress { get; init; } = string.Empty;
}
