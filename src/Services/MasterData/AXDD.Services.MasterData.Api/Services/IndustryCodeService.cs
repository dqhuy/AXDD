using AXDD.Services.MasterData.Api.Data;
using AXDD.Services.MasterData.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Implementation of industry code service
/// </summary>
public class IndustryCodeService : IIndustryCodeService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<IndustryCodeService> _logger;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);

    public IndustryCodeService(
        MasterDataDbContext context,
        ICacheService cache,
        ILogger<IndustryCodeService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<IndustryCodeDto>> GetAllAsync(int? level = null, string? parentCode = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:industrycodes:level:{level}:parent:{parentCode}";
        var cached = await _cache.GetAsync<List<IndustryCodeDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var query = _context.IndustryCodes.Where(ic => ic.IsActive && !ic.IsDeleted);

        if (level.HasValue)
        {
            query = query.Where(ic => ic.Level == level.Value);
        }

        if (!string.IsNullOrWhiteSpace(parentCode))
        {
            query = query.Where(ic => ic.ParentCode == parentCode);
        }

        var codes = await query
            .OrderBy(ic => ic.DisplayOrder)
            .ThenBy(ic => ic.Code)
            .Select(ic => new IndustryCodeDto
            {
                Id = ic.Id,
                Code = ic.Code,
                Name = ic.Name,
                Description = ic.Description,
                ParentCode = ic.ParentCode,
                Level = ic.Level,
                DisplayOrder = ic.DisplayOrder,
                IsActive = ic.IsActive
            })
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(cacheKey, codes, CacheExpiry, cancellationToken);

        return codes;
    }

    public async Task<IndustryCodeDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:industrycode:{code}";
        var cached = await _cache.GetAsync<IndustryCodeDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var industryCode = await _context.IndustryCodes
            .Where(ic => ic.Code == code && ic.IsActive && !ic.IsDeleted)
            .Select(ic => new IndustryCodeDto
            {
                Id = ic.Id,
                Code = ic.Code,
                Name = ic.Name,
                Description = ic.Description,
                ParentCode = ic.ParentCode,
                Level = ic.Level,
                DisplayOrder = ic.DisplayOrder,
                IsActive = ic.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (industryCode != null)
        {
            await _cache.SetAsync(cacheKey, industryCode, CacheExpiry, cancellationToken);
        }

        return industryCode;
    }

    public async Task<IndustryCodeHierarchyDto?> GetHierarchyAsync(string code, CancellationToken cancellationToken = default)
    {
        var industryCode = await GetByCodeAsync(code, cancellationToken);
        if (industryCode == null)
        {
            return null;
        }

        // Get children
        var children = await GetAllAsync(null, code, cancellationToken);

        // Build breadcrumb
        var breadcrumb = new List<IndustryCodeDto> { industryCode };
        var currentParent = industryCode.ParentCode;
        
        while (!string.IsNullOrWhiteSpace(currentParent))
        {
            var parent = await GetByCodeAsync(currentParent, cancellationToken);
            if (parent == null) break;
            
            breadcrumb.Insert(0, parent);
            currentParent = parent.ParentCode;
        }

        return new IndustryCodeHierarchyDto
        {
            Code = industryCode,
            Children = children,
            Breadcrumb = breadcrumb
        };
    }

    public async Task<List<IndustryCodeDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<IndustryCodeDto>();
        }

        var searchTerm = query.ToLower();

        var codes = await _context.IndustryCodes
            .Where(ic => ic.IsActive && !ic.IsDeleted &&
                (ic.Code.ToLower().Contains(searchTerm) ||
                 ic.Name.ToLower().Contains(searchTerm) ||
                 (ic.Description != null && ic.Description.ToLower().Contains(searchTerm))))
            .OrderBy(ic => ic.Level)
            .ThenBy(ic => ic.Code)
            .Take(50)
            .Select(ic => new IndustryCodeDto
            {
                Id = ic.Id,
                Code = ic.Code,
                Name = ic.Name,
                Description = ic.Description,
                ParentCode = ic.ParentCode,
                Level = ic.Level,
                DisplayOrder = ic.DisplayOrder,
                IsActive = ic.IsActive
            })
            .ToListAsync(cancellationToken);

        return codes;
    }
}
