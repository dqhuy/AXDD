using System.Linq.Expressions;
using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Entities;
using AXDD.Services.Report.Api.Domain.Enums;
using AXDD.Services.Report.Api.Domain.Repositories;
using AXDD.Services.Report.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Report.Api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for enterprise reports
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly ReportDbContext _context;
    private readonly DbSet<EnterpriseReport> _dbSet;

    public ReportRepository(ReportDbContext context)
    {
        _context = context;
        _dbSet = context.Set<EnterpriseReport>();
    }

    public async Task<EnterpriseReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<EnterpriseReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<EnterpriseReport, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> FindAsync(Expression<Func<EnterpriseReport, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<EnterpriseReport?> FirstOrDefaultAsync(Expression<Func<EnterpriseReport, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<EnterpriseReport, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<EnterpriseReport, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public IQueryable<EnterpriseReport> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<EnterpriseReport> AddAsync(EnterpriseReport entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<EnterpriseReport> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(EnterpriseReport entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<EnterpriseReport> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(EnterpriseReport entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    public void DeleteRange(IEnumerable<EnterpriseReport> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
        }
        _dbSet.UpdateRange(entities);
    }

    public void HardDelete(EnterpriseReport entity)
    {
        _dbSet.Remove(entity);
    }

    // Custom methods
    public async Task<IReadOnlyList<EnterpriseReport>> GetByEnterpriseIdAsync(
        Guid enterpriseId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.EnterpriseId == enterpriseId)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> GetPendingReportsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> GetByStatusAsync(
        ReportStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> GetBySubmittedByAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.SubmittedBy == username)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> GetByReportTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.ReportType == reportType)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseReport>> GetByDateRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.SubmittedDate >= from && r.SubmittedDate <= to)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid enterpriseId,
        ReportType reportType,
        ReportPeriod reportPeriod,
        int year,
        int? month = null,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(r => r.EnterpriseId == enterpriseId
                && r.ReportType == reportType
                && r.ReportPeriod == reportPeriod
                && r.Year == year);

        if (month.HasValue)
        {
            query = query.Where(r => r.Month == month.Value);
        }

        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
