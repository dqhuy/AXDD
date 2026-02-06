using AXDD.BuildingBlocks.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Enterprise.Api.Controllers;

[ApiController]
[Route("api/enterprise")]
public class EnterpriseController : ControllerBase
{
    private readonly ILogger<EnterpriseController> _logger;

    public EnterpriseController(ILogger<EnterpriseController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetEnterprises([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting enterprises - Page: {Page}, PageSize: {PageSize}", page, pageSize);
        
        var enterprises = new[]
        {
            new { Id = Guid.NewGuid(), Name = "Công ty TNHH ABC", TaxCode = "0123456789" },
            new { Id = Guid.NewGuid(), Name = "Công ty Cổ phần XYZ", TaxCode = "0987654321" }
        };

        var result = new PagedResult<object>
        {
            Items = enterprises,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = 2
        };

        return Ok(ApiResponse<PagedResult<object>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    public IActionResult GetEnterprise(Guid id)
    {
        _logger.LogInformation("Getting enterprise: {Id}", id);
        
        var enterprise = new
        {
            Id = id,
            Name = "Công ty TNHH ABC",
            TaxCode = "0123456789",
            Address = "KCN Biên Hòa 1, Đồng Nai"
        };

        return Ok(ApiResponse<object>.SuccessResponse(enterprise));
    }
}
