using AXDD.Services.MasterData.Api.Data;
using AXDD.Services.MasterData.Api.DTOs;
using AXDD.Services.MasterData.Api.Entities;
using AXDD.Services.MasterData.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Implementation of industrial zone service
/// </summary>
public class IndustrialZoneService : IIndustrialZoneService
{
    private readonly MasterDataDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<IndustrialZoneService> _logger;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(1);

    public IndustrialZoneService(
        MasterDataDbContext context,
        ICacheService cache,
        ILogger<IndustrialZoneService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<IndustrialZoneDto>> GetAllAsync(Guid? provinceId = null, string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.IndustrialZones
            .Include(iz => iz.Province)
            .Include(iz => iz.District)
            .Where(iz => !iz.IsDeleted);

        if (provinceId.HasValue)
        {
            query = query.Where(iz => iz.ProvinceId == provinceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(iz => iz.Status == status);
        }

        return await query
            .OrderBy(iz => iz.Name)
            .Select(iz => new IndustrialZoneDto
            {
                Id = iz.Id,
                Code = iz.Code,
                Name = iz.Name,
                ProvinceId = iz.ProvinceId,
                ProvinceName = iz.Province.Name,
                DistrictId = iz.DistrictId,
                DistrictName = iz.District != null ? iz.District.Name : null,
                Area = iz.Area,
                Status = iz.Status,
                EstablishedDate = iz.EstablishedDate,
                ManagementUnit = iz.ManagementUnit,
                Description = iz.Description
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IndustrialZoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:industrialzone:{id}";
        var cached = await _cache.GetAsync<IndustrialZoneDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var zone = await _context.IndustrialZones
            .Include(iz => iz.Province)
            .Include(iz => iz.District)
            .Where(iz => iz.Id == id && !iz.IsDeleted)
            .Select(iz => new IndustrialZoneDto
            {
                Id = iz.Id,
                Code = iz.Code,
                Name = iz.Name,
                ProvinceId = iz.ProvinceId,
                ProvinceName = iz.Province.Name,
                DistrictId = iz.DistrictId,
                DistrictName = iz.District != null ? iz.District.Name : null,
                Area = iz.Area,
                Status = iz.Status,
                EstablishedDate = iz.EstablishedDate,
                ManagementUnit = iz.ManagementUnit,
                Description = iz.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (zone != null)
        {
            await _cache.SetAsync(cacheKey, zone, CacheExpiry, cancellationToken);
        }

        return zone;
    }

    public async Task<IndustrialZoneDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"masterdata:industrialzone:code:{code}";
        var cached = await _cache.GetAsync<IndustrialZoneDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var zone = await _context.IndustrialZones
            .Include(iz => iz.Province)
            .Include(iz => iz.District)
            .Where(iz => iz.Code == code && !iz.IsDeleted)
            .Select(iz => new IndustrialZoneDto
            {
                Id = iz.Id,
                Code = iz.Code,
                Name = iz.Name,
                ProvinceId = iz.ProvinceId,
                ProvinceName = iz.Province.Name,
                DistrictId = iz.DistrictId,
                DistrictName = iz.District != null ? iz.District.Name : null,
                Area = iz.Area,
                Status = iz.Status,
                EstablishedDate = iz.EstablishedDate,
                ManagementUnit = iz.ManagementUnit,
                Description = iz.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (zone != null)
        {
            await _cache.SetAsync(cacheKey, zone, CacheExpiry, cancellationToken);
        }

        return zone;
    }

    public async Task<List<IndustrialZoneDto>> GetByProvinceAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        return await GetAllAsync(provinceId, null, cancellationToken);
    }

    public async Task<IndustrialZoneDto> CreateAsync(CreateIndustrialZoneRequest request, string userId, CancellationToken cancellationToken = default)
    {
        // Check for duplicate code
        var exists = await _context.IndustrialZones.AnyAsync(iz => iz.Code == request.Code && !iz.IsDeleted, cancellationToken);
        if (exists)
        {
            throw new DuplicateCodeException("IndustrialZone", request.Code);
        }

        var zone = new IndustrialZone
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            ProvinceId = request.ProvinceId,
            DistrictId = request.DistrictId,
            Area = request.Area,
            Status = request.Status,
            EstablishedDate = request.EstablishedDate,
            ManagementUnit = request.ManagementUnit,
            Description = request.Description,
            CreatedBy = userId
        };

        _context.IndustrialZones.Add(zone);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(zone.Id, cancellationToken))!;
    }

    public async Task<IndustrialZoneDto> UpdateAsync(Guid id, UpdateIndustrialZoneRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var zone = await _context.IndustrialZones.FirstOrDefaultAsync(iz => iz.Id == id && !iz.IsDeleted, cancellationToken);
        if (zone == null)
        {
            throw new MasterDataNotFoundException("IndustrialZone", id);
        }

        zone.Name = request.Name;
        zone.DistrictId = request.DistrictId;
        zone.Area = request.Area;
        zone.Status = request.Status;
        zone.EstablishedDate = request.EstablishedDate;
        zone.ManagementUnit = request.ManagementUnit;
        zone.Description = request.Description;
        zone.UpdatedBy = userId;

        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await _cache.RemoveAsync($"masterdata:industrialzone:{id}", cancellationToken);
        await _cache.RemoveAsync($"masterdata:industrialzone:code:{zone.Code}", cancellationToken);

        return (await GetByIdAsync(zone.Id, cancellationToken))!;
    }
}
