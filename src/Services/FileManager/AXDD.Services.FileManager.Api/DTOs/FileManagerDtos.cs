namespace AXDD.Services.FileManager.Api.DTOs;

/// <summary>
/// DTO for file metadata response
/// </summary>
public class FileMetadataDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string EnterpriseCode { get; set; } = string.Empty;
    public Guid? FolderId { get; set; }
    public string? FolderName { get; set; }
    public int Version { get; set; }
    public bool IsLatest { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for file upload request
/// </summary>
public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public string EnterpriseCode { get; set; } = string.Empty;
    public Guid? FolderId { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
}

/// <summary>
/// DTO for folder response
/// </summary>
public class FolderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public string? ParentFolderName { get; set; }
    public string EnterpriseCode { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int FileCount { get; set; }
    public int SubfolderCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for folder creation request
/// </summary>
public class CreateFolderRequest
{
    public string Name { get; set; } = string.Empty;
    public string EnterpriseCode { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for file version response
/// </summary>
public class FileVersionDto
{
    public Guid Id { get; set; }
    public Guid FileMetadataId { get; set; }
    public int Version { get; set; }
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for file share response
/// </summary>
public class FileShareDto
{
    public Guid Id { get; set; }
    public Guid FileMetadataId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string SharedWithUserId { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public string SharedBy { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
}

/// <summary>
/// DTO for file share request
/// </summary>
public class ShareFileRequest
{
    public Guid FileId { get; set; }
    public string SharedWithUserId { get; set; } = string.Empty;
    public string Permission { get; set; } = "Read";
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// DTO for storage quota response
/// </summary>
public class StorageQuotaDto
{
    public string EnterpriseCode { get; set; } = string.Empty;
    public long QuotaBytes { get; set; }
    public long UsedBytes { get; set; }
    public long AvailableBytes { get; set; }
    public double UsagePercentage { get; set; }
    public bool IsExceeded { get; set; }
    public bool IsWarningThresholdReached { get; set; }
}

/// <summary>
/// DTO for file list query
/// </summary>
public class FileListQuery
{
    public string? EnterpriseCode { get; set; }
    public Guid? FolderId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
