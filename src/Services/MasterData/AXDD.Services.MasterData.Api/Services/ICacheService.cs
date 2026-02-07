namespace AXDD.Services.MasterData.Api.Services;

/// <summary>
/// Cache service interface for master data
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets a cached value
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes a cached value
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple cached values by pattern
    /// </summary>
    /// <remarks>
    /// Note: Pattern-based removal requires Redis SCAN command or Lua scripts.
    /// Current implementation logs a warning. For production use, consider implementing
    /// Redis-specific pattern matching or maintaining a cache key registry.
    /// </remarks>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cached values (use with caution)
    /// </summary>
    /// <remarks>
    /// Note: Clearing all cache requires Redis FLUSHDB command with admin privileges.
    /// Current implementation logs a warning. For production use, consider implementing
    /// Redis-specific clear operations or namespace-based cache partitioning.
    /// </remarks>
    Task ClearAllAsync(CancellationToken cancellationToken = default);
}
