using AXDD.Services.MasterData.Api.Data;
using AXDD.Services.MasterData.Api.DTOs;
using AXDD.Services.MasterData.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Implementation of certificate type service
/// </summary>
public class CertificateTypeService : ICertificateTypeService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);
    private const string AllCacheKey = "masterdata:certificatetypes:all";

    public CertificateTypeService(MasterDataDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<CertificateTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync<List<CertificateTypeDto>>(AllCacheKey, cancellationToken);
        if (cached != null) return cached;

        var types = await _context.CertificateTypes
            .Where(ct => ct.IsActive && !ct.IsDeleted)
            .OrderBy(ct => ct.DisplayOrder)
            .ThenBy(ct => ct.Name)
            .Select(ct => new CertificateTypeDto
            {
                Id = ct.Id,
                Code = ct.Code,
                Name = ct.Name,
                Description = ct.Description,
                ValidityPeriod = ct.ValidityPeriod,
                RequiringAuthority = ct.RequiringAuthority,
                IsRequired = ct.IsRequired,
                IsActive = ct.IsActive,
                DisplayOrder = ct.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(AllCacheKey, types, CacheExpiry, cancellationToken);
        return types;
    }

    public async Task<CertificateTypeDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:certificatetype:{code}";
        var cached = await _cache.GetAsync<CertificateTypeDto>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var type = await _context.CertificateTypes
            .Where(ct => ct.Code == code && ct.IsActive && !ct.IsDeleted)
            .Select(ct => new CertificateTypeDto
            {
                Id = ct.Id,
                Code = ct.Code,
                Name = ct.Name,
                Description = ct.Description,
                ValidityPeriod = ct.ValidityPeriod,
                RequiringAuthority = ct.RequiringAuthority,
                IsRequired = ct.IsRequired,
                IsActive = ct.IsActive,
                DisplayOrder = ct.DisplayOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (type != null)
        {
            await _cache.SetAsync(cacheKey, type, CacheExpiry, cancellationToken);
        }

        return type;
    }
}

/// <summary>
/// Implementation of document type service
/// </summary>
public class DocumentTypeService : IDocumentTypeService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);

    public DocumentTypeService(MasterDataDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<DocumentTypeDto>> GetAllAsync(string? category = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:documenttypes:category:{category ?? "all"}";
        var cached = await _cache.GetAsync<List<DocumentTypeDto>>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var query = _context.DocumentTypes.Where(dt => dt.IsActive && !dt.IsDeleted);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(dt => dt.Category == category);
        }

        var types = await query
            .OrderBy(dt => dt.DisplayOrder)
            .ThenBy(dt => dt.Name)
            .Select(dt => new DocumentTypeDto
            {
                Id = dt.Id,
                Code = dt.Code,
                Name = dt.Name,
                Category = dt.Category,
                Description = dt.Description,
                IsRequired = dt.IsRequired,
                AllowedExtensions = dt.AllowedExtensions,
                MaxFileSizeMB = dt.MaxFileSizeMB,
                IsActive = dt.IsActive,
                DisplayOrder = dt.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(cacheKey, types, CacheExpiry, cancellationToken);
        return types;
    }

    public async Task<DocumentTypeDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:documenttype:{code}";
        var cached = await _cache.GetAsync<DocumentTypeDto>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var type = await _context.DocumentTypes
            .Where(dt => dt.Code == code && dt.IsActive && !dt.IsDeleted)
            .Select(dt => new DocumentTypeDto
            {
                Id = dt.Id,
                Code = dt.Code,
                Name = dt.Name,
                Category = dt.Category,
                Description = dt.Description,
                IsRequired = dt.IsRequired,
                AllowedExtensions = dt.AllowedExtensions,
                MaxFileSizeMB = dt.MaxFileSizeMB,
                IsActive = dt.IsActive,
                DisplayOrder = dt.DisplayOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (type != null)
        {
            await _cache.SetAsync(cacheKey, type, CacheExpiry, cancellationToken);
        }

        return type;
    }
}

/// <summary>
/// Implementation of status code service
/// </summary>
public class StatusCodeService : IStatusCodeService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);
    private const string AllCacheKey = "masterdata:statuscodes:all";

    public StatusCodeService(MasterDataDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<StatusCodeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync<List<StatusCodeDto>>(AllCacheKey, cancellationToken);
        if (cached != null) return cached;

        var codes = await _context.StatusCodes
            .Where(sc => sc.IsActive && !sc.IsDeleted)
            .OrderBy(sc => sc.EntityType)
            .ThenBy(sc => sc.DisplayOrder)
            .Select(sc => new StatusCodeDto
            {
                Id = sc.Id,
                Code = sc.Code,
                Name = sc.Name,
                EntityType = sc.EntityType,
                Description = sc.Description,
                Color = sc.Color,
                IsFinal = sc.IsFinal,
                IsActive = sc.IsActive,
                DisplayOrder = sc.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(AllCacheKey, codes, CacheExpiry, cancellationToken);
        return codes;
    }

    public async Task<List<StatusCodeDto>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:statuscodes:entity:{entityType}";
        var cached = await _cache.GetAsync<List<StatusCodeDto>>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var codes = await _context.StatusCodes
            .Where(sc => sc.EntityType == entityType && sc.IsActive && !sc.IsDeleted)
            .OrderBy(sc => sc.DisplayOrder)
            .Select(sc => new StatusCodeDto
            {
                Id = sc.Id,
                Code = sc.Code,
                Name = sc.Name,
                EntityType = sc.EntityType,
                Description = sc.Description,
                Color = sc.Color,
                IsFinal = sc.IsFinal,
                IsActive = sc.IsActive,
                DisplayOrder = sc.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(cacheKey, codes, CacheExpiry, cancellationToken);
        return codes;
    }
}

/// <summary>
/// Implementation of configuration service
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(1);

    public ConfigurationService(MasterDataDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<ConfigurationDto>> GetAllAsync(string? category = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Configurations.Where(c => c.IsActive && !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(c => c.Category == category);
        }

        return await query
            .OrderBy(c => c.Category)
            .ThenBy(c => c.Key)
            .Select(c => new ConfigurationDto
            {
                Id = c.Id,
                Key = c.Key,
                Value = c.Value,
                Category = c.Category,
                Description = c.Description,
                DataType = c.DataType,
                IsActive = c.IsActive,
                IsSystem = c.IsSystem
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ConfigurationDto?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:configuration:{key}";
        var cached = await _cache.GetAsync<ConfigurationDto>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var config = await _context.Configurations
            .Where(c => c.Key == key && c.IsActive && !c.IsDeleted)
            .Select(c => new ConfigurationDto
            {
                Id = c.Id,
                Key = c.Key,
                Value = c.Value,
                Category = c.Category,
                Description = c.Description,
                DataType = c.DataType,
                IsActive = c.IsActive,
                IsSystem = c.IsSystem
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (config != null)
        {
            await _cache.SetAsync(cacheKey, config, CacheExpiry, cancellationToken);
        }

        return config;
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var config = await GetByKeyAsync(key, cancellationToken);
        return config?.Value;
    }

    public async Task<ConfigurationDto> SetValueAsync(string key, string value, string userId, CancellationToken cancellationToken = default)
    {
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key && !c.IsDeleted, cancellationToken);

        if (config == null)
        {
            throw new MasterDataNotFoundException("Configuration", key);
        }

        config.Value = value;
        config.UpdatedBy = userId;

        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await _cache.RemoveAsync($"masterdata:configuration:{key}", cancellationToken);

        return (await GetByKeyAsync(key, cancellationToken))!;
    }
}
