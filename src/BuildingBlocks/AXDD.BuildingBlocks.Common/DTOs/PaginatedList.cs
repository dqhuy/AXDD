using Microsoft.EntityFrameworkCore;

namespace AXDD.BuildingBlocks.Common.DTOs;

/// <summary>
/// Represents a paginated list of items
/// </summary>
/// <typeparam name="T">Type of items in the list</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// Gets the items in the current page
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based)
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the page size
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items across all pages
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets the index of the first item in the current page (1-based)
    /// </summary>
    public int FirstItemIndex => (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Gets the index of the last item in the current page (1-based)
    /// </summary>
    public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

    private PaginatedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Creates a paginated list from a queryable source
    /// </summary>
    /// <param name="source">The queryable source</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list</returns>
    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than or equal to 1", nameof(pageNumber));
        }

        if (pageSize < 1)
        {
            throw new ArgumentException("Page size must be greater than or equal to 1", nameof(pageSize));
        }

        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates a paginated list from a list source
    /// </summary>
    /// <param name="source">The list source</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>A paginated list</returns>
    public static PaginatedList<T> Create(
        IReadOnlyList<T> source,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than or equal to 1", nameof(pageNumber));
        }

        if (pageSize < 1)
        {
            throw new ArgumentException("Page size must be greater than or equal to 1", nameof(pageSize));
        }

        var count = source.Count;
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates an empty paginated list
    /// </summary>
    public static PaginatedList<T> Empty(int pageSize = 10)
    {
        return new PaginatedList<T>(Array.Empty<T>(), 0, 1, pageSize);
    }
}
