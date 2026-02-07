using System.Linq.Expressions;
using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Notification.Api.Domain.Entities;
using AXDD.Services.Notification.Api.Domain.Repositories;
using AXDD.Services.Notification.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Notification.Api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for notifications
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;
    private readonly DbSet<NotificationEntity> _dbSet;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
        _dbSet = context.Notifications;
    }

    public async Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted, cancellationToken);
    }

    public async Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, params Expression<Func<NotificationEntity, object>>[] includes)
    {
        IQueryable<NotificationEntity> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationEntity>> FindAsync(Expression<Func<NotificationEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<NotificationEntity?> FirstOrDefaultAsync(Expression<Func<NotificationEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<NotificationEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<NotificationEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<PagedResult<NotificationEntity>> GetByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<NotificationEntity>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<NotificationEntity>> GetUnreadByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _dbSet.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
            notification.UpdatedAt = now;
        }
    }

    public IQueryable<NotificationEntity> AsQueryable()
    {
        return _dbSet.Where(n => !n.IsDeleted);
    }

    public async Task<NotificationEntity> AddAsync(NotificationEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<NotificationEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(NotificationEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<NotificationEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(NotificationEntity entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
    }

    public void DeleteRange(IEnumerable<NotificationEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
    }

    public void HardDelete(NotificationEntity entity)
    {
        _dbSet.Remove(entity);
    }
}
