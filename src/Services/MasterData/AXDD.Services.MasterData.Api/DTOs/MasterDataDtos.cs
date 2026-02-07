namespace AXDD.Services.MasterData.Api.DTOs;

/// <summary>
/// Industry code DTO
/// </summary>
public record IndustryCodeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ParentCode { get; init; }
    public int Level { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Industry code hierarchy DTO
/// </summary>
public record IndustryCodeHierarchyDto
{
    public IndustryCodeDto Code { get; init; } = null!;
    public List<IndustryCodeDto> Children { get; init; } = new();
    public List<IndustryCodeDto> Breadcrumb { get; init; } = new();
}

/// <summary>
/// Certificate type DTO
/// </summary>
public record CertificateTypeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int? ValidityPeriod { get; init; }
    public string? RequiringAuthority { get; init; }
    public bool IsRequired { get; init; }
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// Document type DTO
/// </summary>
public record DocumentTypeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsRequired { get; init; }
    public string? AllowedExtensions { get; init; }
    public int? MaxFileSizeMB { get; init; }
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// Status code DTO
/// </summary>
public record StatusCodeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Color { get; init; }
    public bool IsFinal { get; init; }
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// Configuration DTO
/// </summary>
public record ConfigurationDto
{
    public Guid Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string DataType { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsSystem { get; init; }
}

/// <summary>
/// Request to update configuration
/// </summary>
public record UpdateConfigurationRequest
{
    public string Value { get; init; } = string.Empty;
}
