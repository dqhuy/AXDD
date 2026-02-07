namespace AXDD.Services.Search.Api.Exceptions;

/// <summary>
/// Base exception for search service errors
/// </summary>
public class SearchException : Exception
{
    public SearchException(string message) : base(message)
    {
    }

    public SearchException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when an index is not found
/// </summary>
public class IndexNotFoundException : SearchException
{
    public string IndexName { get; }

    public IndexNotFoundException(string indexName) 
        : base($"Index '{indexName}' not found")
    {
        IndexName = indexName;
    }

    public IndexNotFoundException(string indexName, Exception innerException) 
        : base($"Index '{indexName}' not found", innerException)
    {
        IndexName = indexName;
    }
}

/// <summary>
/// Exception thrown when connection to Elasticsearch fails
/// </summary>
public class ElasticsearchConnectionException : SearchException
{
    public string Uri { get; }

    public ElasticsearchConnectionException(string uri, string message) 
        : base($"Failed to connect to Elasticsearch at {uri}: {message}")
    {
        Uri = uri;
    }

    public ElasticsearchConnectionException(string uri, string message, Exception innerException) 
        : base($"Failed to connect to Elasticsearch at {uri}: {message}", innerException)
    {
        Uri = uri;
    }
}

/// <summary>
/// Exception thrown when an indexing operation fails
/// </summary>
public class IndexingException : SearchException
{
    public string? DocumentId { get; }

    public IndexingException(string message) : base(message)
    {
    }

    public IndexingException(string message, string? documentId) : base(message)
    {
        DocumentId = documentId;
    }

    public IndexingException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a search query is invalid
/// </summary>
public class InvalidSearchQueryException : SearchException
{
    public string Query { get; }

    public InvalidSearchQueryException(string query, string message) 
        : base($"Invalid search query '{query}': {message}")
    {
        Query = query;
    }

    public InvalidSearchQueryException(string query, string message, Exception innerException) 
        : base($"Invalid search query '{query}': {message}", innerException)
    {
        Query = query;
    }
}
