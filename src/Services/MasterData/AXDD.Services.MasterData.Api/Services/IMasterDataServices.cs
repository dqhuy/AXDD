using AXDD.Services.MasterData.Api.DTOs;

namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Service for managing industrial zones
/// </summary>
public interface IIndustrialZoneService
{
    Task<List<IndustrialZoneDto>> GetAllAsync(Guid? provinceId = null, string? status = null, CancellationToken cancellationToken = default);
    Task<IndustrialZoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IndustrialZoneDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<IndustrialZoneDto>> GetByProvinceAsync(Guid provinceId, CancellationToken cancellationToken = default);
    Task<IndustrialZoneDto> CreateAsync(CreateIndustrialZoneRequest request, string userId, CancellationToken cancellationToken = default);
    Task<IndustrialZoneDto> UpdateAsync(Guid id, UpdateIndustrialZoneRequest request, string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing industry codes (VSIC)
/// </summary>
public interface IIndustryCodeService
{
    Task<List<IndustryCodeDto>> GetAllAsync(int? level = null, string? parentCode = null, CancellationToken cancellationToken = default);
    Task<IndustryCodeDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IndustryCodeHierarchyDto?> GetHierarchyAsync(string code, CancellationToken cancellationToken = default);
    Task<List<IndustryCodeDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing certificate types
/// </summary>
public interface ICertificateTypeService
{
    Task<List<CertificateTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CertificateTypeDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing document types
/// </summary>
public interface IDocumentTypeService
{
    Task<List<DocumentTypeDto>> GetAllAsync(string? category = null, CancellationToken cancellationToken = default);
    Task<DocumentTypeDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing status codes
/// </summary>
public interface IStatusCodeService
{
    Task<List<StatusCodeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<StatusCodeDto>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing configurations
/// </summary>
public interface IConfigurationService
{
    Task<List<ConfigurationDto>> GetAllAsync(string? category = null, CancellationToken cancellationToken = default);
    Task<ConfigurationDto?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);
    Task<ConfigurationDto> SetValueAsync(string key, string value, string userId, CancellationToken cancellationToken = default);
}
