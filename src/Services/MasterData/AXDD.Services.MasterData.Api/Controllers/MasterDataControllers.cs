using AXDD.Services.MasterData.Api.DTOs;
using AXDD.Services.MasterData.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.MasterData.Api.Controllers;

/// <summary>
/// Controller for managing certificate types
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class CertificateTypesController : ControllerBase
{
    private readonly ICertificateTypeService _service;

    public CertificateTypesController(ICertificateTypeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all certificate types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CertificateTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CertificateTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var types = await _service.GetAllAsync(cancellationToken);
        return Ok(types);
    }

    /// <summary>
    /// Gets a certificate type by code
    /// </summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CertificateTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateTypeDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var type = await _service.GetByCodeAsync(code, cancellationToken);
        if (type == null)
        {
            return NotFound();
        }

        return Ok(type);
    }
}

/// <summary>
/// Controller for managing document types
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class DocumentTypesController : ControllerBase
{
    private readonly IDocumentTypeService _service;

    public DocumentTypesController(IDocumentTypeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all document types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DocumentTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DocumentTypeDto>>> GetAll(
        [FromQuery] string? category = null,
        CancellationToken cancellationToken = default)
    {
        var types = await _service.GetAllAsync(category, cancellationToken);
        return Ok(types);
    }

    /// <summary>
    /// Gets a document type by code
    /// </summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(DocumentTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentTypeDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var type = await _service.GetByCodeAsync(code, cancellationToken);
        if (type == null)
        {
            return NotFound();
        }

        return Ok(type);
    }
}

/// <summary>
/// Controller for managing status codes
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class StatusCodesController : ControllerBase
{
    private readonly IStatusCodeService _service;

    public StatusCodesController(IStatusCodeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all status codes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<StatusCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StatusCodeDto>>> GetAll(
        [FromQuery] string? entityType = null,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(entityType))
        {
            var codes = await _service.GetByEntityTypeAsync(entityType, cancellationToken);
            return Ok(codes);
        }

        var allCodes = await _service.GetAllAsync(cancellationToken);
        return Ok(allCodes);
    }
}

/// <summary>
/// Controller for managing configurations
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class ConfigurationsController : ControllerBase
{
    private readonly IConfigurationService _service;

    public ConfigurationsController(IConfigurationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all configurations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ConfigurationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConfigurationDto>>> GetAll(
        [FromQuery] string? category = null,
        CancellationToken cancellationToken = default)
    {
        var configs = await _service.GetAllAsync(category, cancellationToken);
        return Ok(configs);
    }

    /// <summary>
    /// Gets a configuration by key
    /// </summary>
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(ConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConfigurationDto>> GetByKey(string key, CancellationToken cancellationToken)
    {
        var config = await _service.GetByKeyAsync(key, cancellationToken);
        if (config == null)
        {
            return NotFound();
        }

        return Ok(config);
    }

    /// <summary>
    /// Updates a configuration value (Admin only)
    /// </summary>
    [HttpPut("{key}")]
    [ProducesResponseType(typeof(ConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConfigurationDto>> UpdateValue(
        string key,
        [FromBody] UpdateConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var userId = "system"; // TODO: Get from authentication context
        var config = await _service.SetValueAsync(key, request.Value, userId, cancellationToken);
        return Ok(config);
    }
}
