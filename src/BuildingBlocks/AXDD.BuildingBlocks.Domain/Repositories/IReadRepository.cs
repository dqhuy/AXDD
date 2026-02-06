using System.Linq.Expressions;
using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.BuildingBlocks.Domain.Repositories;

/// <summary>
/// Read-only repository interface for query operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IReadRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its identifier including related entities
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="includes">Related entities to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities matching a predicate
    /// </summary>
    /// <param name="predicate">Filter predicate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching entities</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching a predicate
    /// </summary>
    /// <param name="predicate">Filter predicate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first matching entity if found, null otherwise</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches a predicate
    /// </summary>
    /// <param name="predicate">Filter predicate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of entities matching a predicate
    /// </summary>
    /// <param name="predicate">Filter predicate (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of matching entities</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queryable for advanced queries
    /// </summary>
    /// <returns>IQueryable for the entity type</returns>
    IQueryable<T> AsQueryable();
}
