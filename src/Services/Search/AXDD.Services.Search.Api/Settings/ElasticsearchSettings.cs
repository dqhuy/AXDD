namespace AXDD.Services.Search.Api.Settings;

/// <summary>
/// Configuration settings for Elasticsearch connection and behavior
/// </summary>
public class ElasticsearchSettings
{
    /// <summary>
    /// Elasticsearch server URI
    /// </summary>
    public string Uri { get; set; } = "http://localhost:9200";

    /// <summary>
    /// Username for authentication
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for authentication
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Default index prefix
    /// </summary>
    public string DefaultIndex { get; set; } = "axdd";

    /// <summary>
    /// Index names configuration
    /// </summary>
    public IndexConfiguration Indexes { get; set; } = new();

    /// <summary>
    /// Maximum number of retries for failed requests
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int RequestTimeout { get; set; } = 30;

    /// <summary>
    /// Enable/disable certificate validation
    /// </summary>
    public bool ValidateCertificate { get; set; } = true;
}

/// <summary>
/// Index names configuration
/// </summary>
public class IndexConfiguration
{
    public string Enterprises { get; set; } = "enterprises_idx";
    public string Documents { get; set; } = "documents_idx";
    public string Projects { get; set; } = "projects_idx";
}
