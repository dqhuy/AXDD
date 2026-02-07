using System.Linq.Expressions;
using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;
using AXDD.Services.Report.Api.Domain.Repositories;
using AXDD.Services.Report.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Report.Api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for report templates
/// </summary>
public class ReportTemplateRepository : IReportTemplateRepository
{
    private readonly ReportDbContext _context;
    private readonly DbSet<ReportTemplate> _dbSet;

    public ReportTemplateRepository(ReportDbContext context)
    {
        _context = context;
        _dbSet = context.Set<ReportTemplate>();
    }

    public async Task<ReportTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<ReportTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<ReportTemplate, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ReportTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportTemplate>> FindAsync(Expression<Func<ReportTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<ReportTemplate?> FirstOrDefaultAsync(Expression<Func<ReportTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<ReportTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<ReportTemplate, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public IQueryable<ReportTemplate> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<ReportTemplate> AddAsync(ReportTemplate entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<ReportTemplate> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(ReportTemplate entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<ReportTemplate> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(ReportTemplate entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    public void DeleteRange(IEnumerable<ReportTemplate> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
        }
        _dbSet.UpdateRange(entities);
    }

    public void HardDelete(ReportTemplate entity)
    {
        _dbSet.Remove(entity);
    }

    // Custom methods
    public async Task<IReadOnlyList<ReportTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .OrderBy(t => t.ReportType)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReportTemplate?> GetByReportTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ReportType == reportType && t.IsActive)
            .OrderByDescending(t => t.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReportTemplate>> GetAllByReportTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ReportType == reportType)
            .OrderByDescending(t => t.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TemplateNameExistsAsync(
        string templateName,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.TemplateName == templateName);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
