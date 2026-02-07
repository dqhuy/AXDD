using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.BuildingBlocks.Domain.Repositories;

/// <summary>
/// Generic repository interface for write operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> : IReadRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added entity</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    /// <param name="entities">The entities to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    void Update(T entity);

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    /// <param name="entities">The entities to update</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Deletes an entity (soft delete if supported)
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    void Delete(T entity);

    /// <summary>
    /// Deletes multiple entities (soft delete if supported)
    /// </summary>
    /// <param name="entities">The entities to delete</param>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Permanently removes an entity from the database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    void HardDelete(T entity);
}
