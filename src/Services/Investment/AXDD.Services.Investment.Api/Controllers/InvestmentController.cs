using AXDD.BuildingBlocks.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Investment.Api.Controllers;

[ApiController]
[Route("api/investment")]
public class InvestmentController : ControllerBase
{
    private readonly ILogger<InvestmentController> _logger;

    public InvestmentController(ILogger<InvestmentController> logger)
    {
        _logger = logger;
    }

    [HttpGet("projects")]
    public IActionResult GetProjects()
    {
        _logger.LogInformation("Getting investment projects");
        
        var projects = new[]
        {
            new { Id = Guid.NewGuid(), Name = "Dự án đầu tư A", Amount = 100000000000, Status = "Active" },
            new { Id = Guid.NewGuid(), Name = "Dự án đầu tư B", Amount = 50000000000, Status = "Pending" }
        };

        return Ok(ApiResponse<object>.SuccessResponse(projects));
    }
}
