using AXDD.Services.MasterData.Api.DTOs;
using AXDD.Services.MasterData.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.MasterData.Api.Controllers;

/// <summary>
/// Controller for managing administrative divisions (Provinces, Districts, Wards)
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class AdministrativeDivisionsController : ControllerBase
{
    private readonly IAdministrativeDivisionService _service;
    private readonly ILogger<AdministrativeDivisionsController> _logger;

    public AdministrativeDivisionsController(
        IAdministrativeDivisionService service,
        ILogger<AdministrativeDivisionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all provinces
    /// </summary>
    [HttpGet("provinces")]
    [ProducesResponseType(typeof(List<ProvinceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProvinceDto>>> GetProvinces(CancellationToken cancellationToken)
    {
        var provinces = await _service.GetProvincesAsync(cancellationToken);
        return Ok(provinces);
    }

    /// <summary>
    /// Gets a province by ID
    /// </summary>
    [HttpGet("provinces/{id:guid}")]
    [ProducesResponseType(typeof(ProvinceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProvinceDto>> GetProvince(Guid id, CancellationToken cancellationToken)
    {
        var province = await _service.GetProvinceByIdAsync(id, cancellationToken);
        if (province == null)
        {
            return NotFound();
        }

        return Ok(province);
    }

    /// <summary>
    /// Gets districts by province ID
    /// </summary>
    [HttpGet("provinces/{provinceId:guid}/districts")]
    [ProducesResponseType(typeof(List<DistrictDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DistrictDto>>> GetDistricts(Guid provinceId, CancellationToken cancellationToken)
    {
        var districts = await _service.GetDistrictsAsync(provinceId, cancellationToken);
        return Ok(districts);
    }

    /// <summary>
    /// Gets a district by ID
    /// </summary>
    [HttpGet("districts/{id:guid}")]
    [ProducesResponseType(typeof(DistrictDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DistrictDto>> GetDistrict(Guid id, CancellationToken cancellationToken)
    {
        var district = await _service.GetDistrictByIdAsync(id, cancellationToken);
        if (district == null)
        {
            return NotFound();
        }

        return Ok(district);
    }

    /// <summary>
    /// Gets wards by district ID
    /// </summary>
    [HttpGet("districts/{districtId:guid}/wards")]
    [ProducesResponseType(typeof(List<WardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WardDto>>> GetWards(Guid districtId, CancellationToken cancellationToken)
    {
        var wards = await _service.GetWardsAsync(districtId, cancellationToken);
        return Ok(wards);
    }

    /// <summary>
    /// Gets a ward by ID
    /// </summary>
    [HttpGet("wards/{id:guid}")]
    [ProducesResponseType(typeof(WardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WardDto>> GetWard(Guid id, CancellationToken cancellationToken)
    {
        var ward = await _service.GetWardByIdAsync(id, cancellationToken);
        if (ward == null)
        {
            return NotFound();
        }

        return Ok(ward);
    }

    /// <summary>
    /// Gets full address information for a ward
    /// </summary>
    [HttpGet("wards/{wardId:guid}/full-address")]
    [ProducesResponseType(typeof(FullAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FullAddressDto>> GetFullAddress(Guid wardId, CancellationToken cancellationToken)
    {
        var fullAddress = await _service.GetFullAddressAsync(wardId, cancellationToken);
        if (fullAddress == null)
        {
            return NotFound();
        }

        return Ok(fullAddress);
    }
}
