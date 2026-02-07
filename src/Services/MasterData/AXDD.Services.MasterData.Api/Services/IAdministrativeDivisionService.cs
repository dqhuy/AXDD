using AXDD.Services.MasterData.Api.DTOs;

namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Service for managing administrative divisions (Province, District, Ward)
/// </summary>
public interface IAdministrativeDivisionService
{
    /// <summary>
    /// Gets all provinces
    /// </summary>
    Task<List<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a province by ID
    /// </summary>
    Task<ProvinceDto?> GetProvinceByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets districts by province ID
    /// </summary>
    Task<List<DistrictDto>> GetDistrictsAsync(Guid provinceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a district by ID
    /// </summary>
    Task<DistrictDto?> GetDistrictByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets wards by district ID
    /// </summary>
    Task<List<WardDto>> GetWardsAsync(Guid districtId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a ward by ID
    /// </summary>
    Task<WardDto?> GetWardByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets full address information for a ward
    /// </summary>
    Task<FullAddressDto?> GetFullAddressAsync(Guid wardId, CancellationToken cancellationToken = default);
}
