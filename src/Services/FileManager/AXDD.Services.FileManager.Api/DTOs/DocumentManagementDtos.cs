using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.DTOs;

#region Document Type DTOs

/// <summary>
/// DTO for document type response
/// </summary>
public record DocumentTypeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<DocumentTypeMetadataFieldDto> MetadataFields { get; init; } = [];
}

/// <summary>
/// DTO for creating/updating document type
/// </summary>
public record CreateDocumentTypeRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO for document type metadata field
/// </summary>
public record DocumentTypeMetadataFieldDto
{
    public Guid Id { get; init; }
    public Guid DocumentTypeId { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string DataType { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
    public string? ValidationRules { get; init; }
    public int DisplayOrder { get; init; }
    public string? ListOptions { get; init; }
}

/// <summary>
/// DTO for creating document type metadata field
/// </summary>
public record CreateDocumentTypeMetadataFieldRequest
{
    public string FieldName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string DataType { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
    public string? ValidationRules { get; init; }
    public int DisplayOrder { get; init; }
    public string? ListOptions { get; init; }
}

#endregion

#region Folder Type DTOs

/// <summary>
/// DTO for folder type response
/// </summary>
public record FolderTypeDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int RetentionPeriodMonths { get; init; }
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<FolderTypeMetadataFieldDto> MetadataFields { get; init; } = [];
}

/// <summary>
/// DTO for creating/updating folder type
/// </summary>
public record CreateFolderTypeRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public int RetentionPeriodMonths { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO for folder type metadata field
/// </summary>
public record FolderTypeMetadataFieldDto
{
    public Guid Id { get; init; }
    public Guid FolderTypeId { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string DataType { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
    public string? ValidationRules { get; init; }
    public int DisplayOrder { get; init; }
    public string? ListOptions { get; init; }
}

/// <summary>
/// DTO for creating folder type metadata field
/// </summary>
public record CreateFolderTypeMetadataFieldRequest
{
    public string FieldName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string DataType { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
    public string? ValidationRules { get; init; }
    public int DisplayOrder { get; init; }
    public string? ListOptions { get; init; }
}

#endregion

#region Digital Storage DTOs

/// <summary>
/// DTO for digital storage response
/// </summary>
public record DigitalStorageDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string BucketName { get; init; } = string.Empty;
    public long TotalCapacityBytes { get; init; }
    public long UsedCapacityBytes { get; init; }
    public long AvailableCapacityBytes { get; init; }
    public double UsagePercentage { get; init; }
    public bool IsActive { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for creating/updating digital storage
/// </summary>
public record CreateDigitalStorageRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string BucketName { get; init; } = string.Empty;
    public long TotalCapacityBytes { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}

#endregion

#region Physical Storage DTOs

/// <summary>
/// DTO for physical storage response
/// </summary>
public record PhysicalStorageDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
    public int LocationCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<PhysicalStorageLocationDto> Locations { get; init; } = [];
}

/// <summary>
/// DTO for creating/updating physical storage
/// </summary>
public record CreatePhysicalStorageRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Address { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}

/// <summary>
/// DTO for physical storage location response
/// </summary>
public record PhysicalStorageLocationDto
{
    public Guid Id { get; init; }
    public Guid PhysicalStorageId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsOccupied { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for creating physical storage location
/// </summary>
public record CreatePhysicalStorageLocationRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

#endregion

#region Document Approval DTOs

/// <summary>
/// DTO for document approval response
/// </summary>
public record DocumentApprovalDto
{
    public Guid Id { get; init; }
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public string RequestedBy { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
    public ApprovalStatus Status { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public string? RejectionReason { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO for submitting document for approval
/// </summary>
public record SubmitApprovalRequest
{
    public Guid DocumentId { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// DTO for approving/rejecting document
/// </summary>
public record ProcessApprovalRequest
{
    public string? RejectionReason { get; init; }
    public string? Notes { get; init; }
}

#endregion

#region Document Loan DTOs

/// <summary>
/// DTO for document loan response
/// </summary>
public record DocumentLoanDto
{
    public Guid Id { get; init; }
    public string LoanCode { get; init; } = string.Empty;
    public string BorrowerUserId { get; init; } = string.Empty;
    public string BorrowerName { get; init; } = string.Empty;
    public string? BorrowerDepartment { get; init; }
    public DateTime RequestedAt { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? ReturnedAt { get; init; }
    public LoanStatus Status { get; init; }
    public LoanType LoanType { get; init; }
    public string? Purpose { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public string? RejectionReason { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public List<DocumentLoanItemDto> Items { get; init; } = [];
}

/// <summary>
/// DTO for document loan item
/// </summary>
public record DocumentLoanItemDto
{
    public Guid Id { get; init; }
    public Guid DocumentLoanId { get; init; }
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public bool IsReturned { get; init; }
    public DateTime? ReturnedAt { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// DTO for creating document loan request
/// </summary>
public record CreateDocumentLoanRequest
{
    public string BorrowerName { get; init; } = string.Empty;
    public string? BorrowerDepartment { get; init; }
    public DateTime DueDate { get; init; }
    public LoanType LoanType { get; init; }
    public string? Purpose { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
    public List<Guid> DocumentIds { get; init; } = [];
}

/// <summary>
/// DTO for processing loan (approve/reject)
/// </summary>
public record ProcessLoanRequest
{
    public string? RejectionReason { get; init; }
}

/// <summary>
/// DTO for loan statistics
/// </summary>
public record LoanStatisticsDto
{
    public int TotalLoans { get; init; }
    public int PendingLoans { get; init; }
    public int ApprovedLoans { get; init; }
    public int RejectedLoans { get; init; }
    public int BorrowedLoans { get; init; }
    public int ReturnedLoans { get; init; }
    public int OverdueLoans { get; init; }
    public Dictionary<LoanType, int> LoansByType { get; init; } = new();
    public int TotalBorrowers { get; init; }
}

#endregion

#region Folder Permission DTOs

/// <summary>
/// DTO for folder permission response
/// </summary>
public record FolderPermissionDto
{
    public Guid Id { get; init; }
    public Guid FolderId { get; init; }
    public string? UserId { get; init; }
    public string? UserGroupId { get; init; }
    public PermissionLevel Permission { get; init; }
    public bool CanShare { get; init; }
    public bool CanDownload { get; init; }
    public bool CanPrint { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string GrantedBy { get; init; } = string.Empty;
    public DateTime GrantedAt { get; init; }
}

/// <summary>
/// DTO for granting folder permission
/// </summary>
public record GrantFolderPermissionRequest
{
    public string? UserId { get; init; }
    public string? UserGroupId { get; init; }
    public PermissionLevel Permission { get; init; }
    public bool CanShare { get; init; }
    public bool CanDownload { get; init; } = true;
    public bool CanPrint { get; init; } = true;
    public DateTime? ExpiresAt { get; init; }
}

#endregion

#region Security Level DTOs

/// <summary>
/// DTO for security level response
/// </summary>
public record SecurityLevelDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Level { get; init; }
    public bool RequiresApproval { get; init; }
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for creating/updating security level
/// </summary>
public record CreateSecurityLevelRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Level { get; init; }
    public bool RequiresApproval { get; init; }
    public bool IsActive { get; init; } = true;
    public int DisplayOrder { get; init; }
}

#endregion

#region Audit Log DTOs

/// <summary>
/// DTO for audit log response
/// </summary>
public record AuditLogDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public AuditAction Action { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string? EntityName { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public DateTime Timestamp { get; init; }
    public string EnterpriseCode { get; init; } = string.Empty;
}

/// <summary>
/// DTO for audit log query
/// </summary>
public record AuditLogQueryDto
{
    public string? EnterpriseCode { get; init; }
    public string? UserId { get; init; }
    public AuditAction? Action { get; init; }
    public string? EntityType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

#endregion

#region Statistics DTOs

/// <summary>
/// DTO for document statistics
/// </summary>
public record DocumentStatisticsDto
{
    public int TotalDocuments { get; init; }
    public int DigitizedDocuments { get; init; }
    public int PendingCataloging { get; init; }
    public int CatalogedDocuments { get; init; }
    public int ApprovedDocuments { get; init; }
    public int RejectedDocuments { get; init; }
    public Dictionary<string, int> DocumentsByType { get; init; } = new();
    public long TotalStorageBytes { get; init; }
    public long UsedStorageBytes { get; init; }
}

/// <summary>
/// DTO for storage statistics
/// </summary>
public record StorageStatisticsDto
{
    public int TotalFolders { get; init; }
    public Dictionary<string, int> FoldersByType { get; init; } = new();
    public Dictionary<string, long> StorageByEnterpriseBytes { get; init; } = new();
    public int ActiveUsers { get; init; }
}

#endregion
