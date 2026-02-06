using AXDD.BuildingBlocks.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

[ApiController]
[Route("api/filemanager")]
public class FileManagerController : ControllerBase
{
    private readonly ILogger<FileManagerController> _logger;

    public FileManagerController(ILogger<FileManagerController> logger)
    {
        _logger = logger;
    }

    [HttpGet("files")]
    public IActionResult GetFiles()
    {
        _logger.LogInformation("Getting files");
        
        var files = new[]
        {
            new { Id = Guid.NewGuid(), FileName = "document1.pdf", Size = 1024000, UploadDate = DateTime.UtcNow },
            new { Id = Guid.NewGuid(), FileName = "document2.docx", Size = 512000, UploadDate = DateTime.UtcNow }
        };

        return Ok(ApiResponse<object>.SuccessResponse(files));
    }

    [HttpPost("upload")]
    public IActionResult Upload([FromForm] IFormFile file)
    {
        _logger.LogInformation("Uploading file: {FileName}", file?.FileName);
        
        if (file == null)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded"));
        }

        var result = new
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            Size = file.Length,
            UploadDate = DateTime.UtcNow
        };

        return Ok(ApiResponse<object>.SuccessResponse(result, "File uploaded successfully"));
    }
}
