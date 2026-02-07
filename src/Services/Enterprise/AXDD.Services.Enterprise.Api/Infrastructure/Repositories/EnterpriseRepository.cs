using System.Linq.Expressions;
using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;
using AXDD.Services.Enterprise.Api.Domain.Repositories;
using AXDD.Services.Enterprise.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Enterprise.Api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for enterprises
/// </summary>
public class EnterpriseRepository : IEnterpriseRepository
{
    private readonly EnterpriseDbContext _context;
    private readonly DbSet<EnterpriseEntity> _dbSet;

    public EnterpriseRepository(EnterpriseDbContext context)
    {
        _context = context;
        _dbSet = context.Set<EnterpriseEntity>();
    }

    public async Task<EnterpriseEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<EnterpriseEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<EnterpriseEntity, object>>[] includes)
    {
        IQueryable<EnterpriseEntity> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseEntity>> FindAsync(Expression<Func<EnterpriseEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<EnterpriseEntity?> FirstOrDefaultAsync(Expression<Func<EnterpriseEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<EnterpriseEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<EnterpriseEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync(cancellationToken);
    }

    public IQueryable<EnterpriseEntity> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<EnterpriseEntity> AddAsync(EnterpriseEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<EnterpriseEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(EnterpriseEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<EnterpriseEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(EnterpriseEntity entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    public void DeleteRange(IEnumerable<EnterpriseEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
        }
        _dbSet.UpdateRange(entities);
    }

    public void HardDelete(EnterpriseEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<EnterpriseEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Code == code, cancellationToken);
    }

    public async Task<EnterpriseEntity?> GetByTaxCodeAsync(string taxCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.TaxCode == taxCode, cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseEntity>> GetByIndustrialZoneAsync(Guid zoneId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.IndustrialZoneId == zoneId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseEntity>> GetByIndustryCodeAsync(string industryCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.IndustryCode == industryCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseEntity>> GetByStatusAsync(EnterpriseStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.Code == code);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> TaxCodeExistsAsync(string taxCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.TaxCode == taxCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
