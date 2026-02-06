using AXDD.BuildingBlocks.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Report.Api.Controllers;

[ApiController]
[Route("api/report")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ReportController> _logger;

    public ReportController(ILogger<ReportController> logger)
    {
        _logger = logger;
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        _logger.LogInformation("Getting report summary");
        
        var summary = new
        {
            TotalEnterprises = 150,
            TotalInvestment = 5000000000000,
            ActiveProjects = 45,
            GeneratedDate = DateTime.UtcNow
        };

        return Ok(ApiResponse<object>.SuccessResponse(summary));
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] string format = "pdf")
    {
        _logger.LogInformation("Exporting report in format: {Format}", format);
        
        var result = new
        {
            FileName = $"report_{DateTime.UtcNow:yyyyMMdd}.{format}",
            DownloadUrl = $"/downloads/report_{DateTime.UtcNow:yyyyMMdd}.{format}"
        };

        return Ok(ApiResponse<object>.SuccessResponse(result, "Report exported successfully"));
    }
}
