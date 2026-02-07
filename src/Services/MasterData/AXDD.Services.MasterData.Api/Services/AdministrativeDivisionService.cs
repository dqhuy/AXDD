using AXDD.Services.MasterData.Api.Data;
using AXDD.Services.MasterData.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Implementation of administrative division service
/// </summary>
public class AdministrativeDivisionService : IAdministrativeDivisionService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<AdministrativeDivisionService> _logger;
    private const string ProvincesCacheKey = "masterdata:provinces:all";
    private static readonly TimeSpan ProvinceCacheExpiry = TimeSpan.FromHours(24);

    public AdministrativeDivisionService(
        MasterDataDbContext context,
        ICacheService cache,
        ILogger<AdministrativeDivisionService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        // Try to get from cache
        var cached = await _cache.GetAsync<List<ProvinceDto>>(ProvincesCacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var provinces = await _context.Provinces
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .Select(p => new ProvinceDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Region = p.Region,
                DisplayOrder = p.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        // Cache the result
        await _cache.SetAsync(ProvincesCacheKey, provinces, ProvinceCacheExpiry, cancellationToken);

        return provinces;
    }

    public async Task<ProvinceDto?> GetProvinceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:province:{id}";
        
        // Try cache
        var cached = await _cache.GetAsync<ProvinceDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var province = await _context.Provinces
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new ProvinceDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Region = p.Region,
                DisplayOrder = p.DisplayOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (province != null)
        {
            await _cache.SetAsync(cacheKey, province, ProvinceCacheExpiry, cancellationToken);
        }

        return province;
    }

    public async Task<List<DistrictDto>> GetDistrictsAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:districts:province:{provinceId}";
        
        // Try cache
        var cached = await _cache.GetAsync<List<DistrictDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var districts = await _context.Districts
            .Include(d => d.Province)
            .Where(d => d.ProvinceId == provinceId && !d.IsDeleted)
            .OrderBy(d => d.DisplayOrder)
            .ThenBy(d => d.Name)
            .Select(d => new DistrictDto
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                ProvinceId = d.ProvinceId,
                ProvinceName = d.Province.Name,
                DisplayOrder = d.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        // Cache the result
        await _cache.SetAsync(cacheKey, districts, ProvinceCacheExpiry, cancellationToken);

        return districts;
    }

    public async Task<DistrictDto?> GetDistrictByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:district:{id}";
        
        // Try cache
        var cached = await _cache.GetAsync<DistrictDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var district = await _context.Districts
            .Include(d => d.Province)
            .Where(d => d.Id == id && !d.IsDeleted)
            .Select(d => new DistrictDto
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                ProvinceId = d.ProvinceId,
                ProvinceName = d.Province.Name,
                DisplayOrder = d.DisplayOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (district != null)
        {
            await _cache.SetAsync(cacheKey, district, ProvinceCacheExpiry, cancellationToken);
        }

        return district;
    }

    public async Task<List<WardDto>> GetWardsAsync(Guid districtId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:wards:district:{districtId}";
        
        // Try cache
        var cached = await _cache.GetAsync<List<WardDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var wards = await _context.Wards
            .Include(w => w.District)
            .Where(w => w.DistrictId == districtId && !w.IsDeleted)
            .OrderBy(w => w.DisplayOrder)
            .ThenBy(w => w.Name)
            .Select(w => new WardDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name,
                DistrictId = w.DistrictId,
                DistrictName = w.District.Name,
                DisplayOrder = w.DisplayOrder
            })
            .ToListAsync(cancellationToken);

        // Cache the result
        await _cache.SetAsync(cacheKey, wards, ProvinceCacheExpiry, cancellationToken);

        return wards;
    }

    public async Task<WardDto?> GetWardByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:ward:{id}";
        
        // Try cache
        var cached = await _cache.GetAsync<WardDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var ward = await _context.Wards
            .Include(w => w.District)
            .Where(w => w.Id == id && !w.IsDeleted)
            .Select(w => new WardDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name,
                DistrictId = w.DistrictId,
                DistrictName = w.District.Name,
                DisplayOrder = w.DisplayOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (ward != null)
        {
            await _cache.SetAsync(cacheKey, ward, ProvinceCacheExpiry, cancellationToken);
        }

        return ward;
    }

    public async Task<FullAddressDto?> GetFullAddressAsync(Guid wardId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:fulladdress:ward:{wardId}";
        
        // Try cache
        var cached = await _cache.GetAsync<FullAddressDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Get from database
        var ward = await _context.Wards
            .Include(w => w.District)
            .ThenInclude(d => d.Province)
            .Where(w => w.Id == wardId && !w.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (ward == null)
        {
            return null;
        }

        var result = new FullAddressDto
        {
            Ward = new WardDto
            {
                Id = ward.Id,
                Code = ward.Code,
                Name = ward.Name,
                DistrictId = ward.DistrictId,
                DistrictName = ward.District.Name,
                DisplayOrder = ward.DisplayOrder
            },
            District = new DistrictDto
            {
                Id = ward.District.Id,
                Code = ward.District.Code,
                Name = ward.District.Name,
                ProvinceId = ward.District.ProvinceId,
                ProvinceName = ward.District.Province.Name,
                DisplayOrder = ward.District.DisplayOrder
            },
            Province = new ProvinceDto
            {
                Id = ward.District.Province.Id,
                Code = ward.District.Province.Code,
                Name = ward.District.Province.Name,
                Region = ward.District.Province.Region,
                DisplayOrder = ward.District.Province.DisplayOrder
            },
            FormattedAddress = $"{ward.Name}, {ward.District.Name}, {ward.District.Province.Name}"
        };

        // Cache the result
        await _cache.SetAsync(cacheKey, result, ProvinceCacheExpiry, cancellationToken);

        return result;
    }
}
