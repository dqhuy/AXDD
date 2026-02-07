using System.Linq.Expressions;
using AXDD.Services.Notification.Api.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Repositories;
using AXDD.Services.Notification.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Notification.Api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for notification templates
/// </summary>
public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly NotificationDbContext _context;
    private readonly DbSet<NotificationTemplate> _dbSet;

    public NotificationTemplateRepository(NotificationDbContext context)
    {
        _context = context;
        _dbSet = context.NotificationTemplates;
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<NotificationTemplate, object>>[] includes)
    {
        IQueryable<NotificationTemplate> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.TemplateKey)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> FindAsync(Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<NotificationTemplate?> FirstOrDefaultAsync(Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<NotificationTemplate, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync(cancellationToken);
    }

    public IQueryable<NotificationTemplate> AsQueryable()
    {
        return _dbSet.Where(t => !t.IsDeleted);
    }

    public async Task<NotificationTemplate?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.TemplateKey == key && !t.IsDeleted, cancellationToken);
    }

    public async Task<List<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.TemplateKey)
            .ToListAsync(cancellationToken);
    }

    public async Task<NotificationTemplate> AddAsync(NotificationTemplate entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<NotificationTemplate> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(NotificationTemplate entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<NotificationTemplate> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(NotificationTemplate entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
    }

    public void DeleteRange(IEnumerable<NotificationTemplate> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
    }

    public void HardDelete(NotificationTemplate entity)
    {
        _dbSet.Remove(entity);
    }
}
