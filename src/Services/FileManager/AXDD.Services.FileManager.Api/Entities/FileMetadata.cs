using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents file metadata stored in the database
/// </summary>
public class FileMetadata : BaseEntity
{
    public FileMetadata()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the file
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file extension (e.g., .pdf, .docx)
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MinIO bucket name
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MinIO object key (path in bucket)
    /// </summary>
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the enterprise code for organization
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the folder ID this file belongs to
    /// </summary>
    public Guid? FolderId { get; set; }

    /// <summary>
    /// Gets or sets the folder this file belongs to
    /// </summary>
    public Folder? Folder { get; set; }

    /// <summary>
    /// Gets or sets the file version number
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether this is the latest version
    /// </summary>
    public bool IsLatest { get; set; } = true;

    /// <summary>
    /// Gets or sets the user ID who uploaded the file
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the file was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Gets or sets the file checksum (MD5)
    /// </summary>
    public string Checksum { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the file tags for search
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Gets or sets the document type ID
    /// </summary>
    public Guid? DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the document type
    /// </summary>
    public DocumentType? DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the security level ID
    /// </summary>
    public Guid? SecurityLevelId { get; set; }

    /// <summary>
    /// Gets or sets the security level
    /// </summary>
    public SecurityLevel? SecurityLevel { get; set; }

    /// <summary>
    /// Gets or sets the password hash for protected files
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets whether the file is password protected
    /// </summary>
    public bool IsPasswordProtected { get; set; }

    /// <summary>
    /// Gets or sets the QR code data
    /// </summary>
    public string? QRCode { get; set; }

    /// <summary>
    /// Gets or sets the OCR extracted content
    /// </summary>
    public string? OcrContent { get; set; }

    /// <summary>
    /// Gets or sets whether OCR has been processed
    /// </summary>
    public bool IsOcrProcessed { get; set; }

    /// <summary>
    /// Gets or sets the document date (date on the document itself)
    /// </summary>
    public DateTime? DocumentDate { get; set; }

    /// <summary>
    /// Gets or sets the document number
    /// </summary>
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Gets or sets the physical storage location ID
    /// </summary>
    public Guid? PhysicalStorageLocationId { get; set; }

    /// <summary>
    /// Gets or sets the physical storage location
    /// </summary>
    public PhysicalStorageLocation? PhysicalStorageLocation { get; set; }

    /// <summary>
    /// Gets or sets the collection of file versions
    /// </summary>
    public ICollection<FileVersion> Versions { get; set; } = new List<FileVersion>();

    /// <summary>
    /// Gets or sets the collection of file shares
    /// </summary>
    public ICollection<FileShare> Shares { get; set; } = new List<FileShare>();

    /// <summary>
    /// Gets or sets the collection of document approvals
    /// </summary>
    public ICollection<DocumentApproval> Approvals { get; set; } = new List<DocumentApproval>();
}
