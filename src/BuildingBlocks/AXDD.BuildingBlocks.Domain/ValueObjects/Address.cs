namespace AXDD.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Value object representing an address
/// </summary>
public record Address
{
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
}
