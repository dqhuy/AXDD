namespace AXDD.Services.Search.Api.DTOs;

/// <summary>
/// Generic search response wrapper
/// </summary>
/// <typeparam name="T">Type of search result document</typeparam>
public class SearchResponse<T>
{
    /// <summary>
    /// Search results
    /// </summary>
    public List<SearchResult<T>> Results { get; set; } = new();

    /// <summary>
    /// Total number of matching documents
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Time taken for search in milliseconds
    /// </summary>
    public long Took { get; set; }

    /// <summary>
    /// Maximum relevance score
    /// </summary>
    public double? MaxScore { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Faceted aggregations
    /// </summary>
    public Dictionary<string, List<FacetItem>>? Facets { get; set; }
}

/// <summary>
/// Individual search result with scoring and highlighting
/// </summary>
/// <typeparam name="T">Type of search result document</typeparam>
public class SearchResult<T>
{
    /// <summary>
    /// The source document
    /// </summary>
    public T Document { get; set; } = default!;

    /// <summary>
    /// Relevance score
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Highlighted field snippets
    /// </summary>
    public Dictionary<string, List<string>>? Highlights { get; set; }
}

/// <summary>
/// Facet item for aggregation results
/// </summary>
public class FacetItem
{
    /// <summary>
    /// Facet value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Number of documents matching this facet
    /// </summary>
    public long Count { get; set; }
}

/// <summary>
/// Suggestion response for autocomplete
/// </summary>
public class SuggestionResponse
{
    /// <summary>
    /// List of suggestions
    /// </summary>
    public List<string> Suggestions { get; set; } = new();

    /// <summary>
    /// Original query text
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Time taken in milliseconds
    /// </summary>
    public long Took { get; set; }
}

/// <summary>
/// Index statistics response
/// </summary>
public class IndexStatsResponse
{
    /// <summary>
    /// Index name
    /// </summary>
    public string IndexName { get; set; } = string.Empty;

    /// <summary>
    /// Number of documents in the index
    /// </summary>
    public long DocumentCount { get; set; }

    /// <summary>
    /// Size of the index in bytes
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Index health status
    /// </summary>
    public string Health { get; set; } = string.Empty;

    /// <summary>
    /// Number of shards
    /// </summary>
    public int Shards { get; set; }

    /// <summary>
    /// Number of replicas
    /// </summary>
    public int Replicas { get; set; }
}

/// <summary>
/// Multi-index search response
/// </summary>
public class MultiSearchResponse
{
    /// <summary>
    /// Enterprise search results
    /// </summary>
    public List<SearchResult<object>> Enterprises { get; set; } = new();

    /// <summary>
    /// Document search results
    /// </summary>
    public List<SearchResult<object>> Documents { get; set; } = new();

    /// <summary>
    /// Project search results
    /// </summary>
    public List<SearchResult<object>> Projects { get; set; } = new();

    /// <summary>
    /// Total results across all indexes
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Time taken in milliseconds
    /// </summary>
    public long Took { get; set; }
}
