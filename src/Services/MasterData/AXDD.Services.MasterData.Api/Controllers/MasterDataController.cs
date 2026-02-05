using AXDD.BuildingBlocks.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.MasterData.Api.Controllers;

[ApiController]
[Route("api/masterdata")]
public class MasterDataController : ControllerBase
{
    private readonly ILogger<MasterDataController> _logger;

    public MasterDataController(ILogger<MasterDataController> logger)
    {
        _logger = logger;
    }

    [HttpGet("provinces")]
    public IActionResult GetProvinces()
    {
        _logger.LogInformation("Getting provinces");
        
        var provinces = new[]
        {
            new { Id = Guid.NewGuid(), Name = "Đồng Nai", Code = "DN" },
            new { Id = Guid.NewGuid(), Name = "Hồ Chí Minh", Code = "HCM" }
        };

        return Ok(ApiResponse<object>.SuccessResponse(provinces));
    }

    [HttpGet("industries")]
    public IActionResult GetIndustries()
    {
        _logger.LogInformation("Getting industries");
        
        var industries = new[]
        {
            new { Id = Guid.NewGuid(), Name = "Sản xuất", Code = "MFG" },
            new { Id = Guid.NewGuid(), Name = "Dịch vụ", Code = "SVC" }
        };

        return Ok(ApiResponse<object>.SuccessResponse(industries));
    }
}
