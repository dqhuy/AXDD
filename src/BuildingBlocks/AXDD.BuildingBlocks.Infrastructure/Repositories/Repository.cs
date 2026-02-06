using System.Linq.Expressions;
using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.BuildingBlocks.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AXDD.BuildingBlocks.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation using Entity Framework Core
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
internal class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync(cancellationToken);
    }

    public IQueryable<T> AsQueryable()
    {
        return _dbSet.Where(e => !e.IsDeleted).AsQueryable();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.CreatedAt = now;
        }
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.UpdatedAt = now;
        }
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        // Soft delete
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = now;
        }
        _dbSet.UpdateRange(entities);
    }

    public void HardDelete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
