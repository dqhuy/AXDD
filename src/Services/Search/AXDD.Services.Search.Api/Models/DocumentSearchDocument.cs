namespace AXDD.Services.Search.Api.Models;

/// <summary>
/// Document search document for Elasticsearch indexing
/// </summary>
public class DocumentSearchDocument
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Extracted text content from the document
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// File type/extension (pdf, docx, xlsx, etc.)
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Associated enterprise code
    /// </summary>
    public string? EnterpriseCode { get; set; }

    /// <summary>
    /// Associated enterprise name
    /// </summary>
    public string? EnterpriseName { get; set; }

    /// <summary>
    /// User who uploaded the document
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Upload timestamp
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Document tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Document description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Document category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// File storage path
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Document status
    /// </summary>
    public string Status { get; set; } = "Active";
}
