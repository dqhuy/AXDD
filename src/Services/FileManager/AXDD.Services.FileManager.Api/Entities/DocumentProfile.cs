using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a document profile (folder-like structure) for storing and organizing documents
/// Similar to Google Drive/Dropbox folder concept with configurable metadata schema
/// </summary>
public class DocumentProfile : BaseEntity
{
    public DocumentProfile()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the profile name (e.g., "Hồ sơ đầu tư", "Hồ sơ môi trường")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the profile code (unique identifier within enterprise)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the enterprise code this profile belongs to
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the profile type (e.g., "InvestmentCertificate", "EnvironmentalPermit")
    /// </summary>
    public string ProfileType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the profile description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the parent profile ID for hierarchy (null for root profiles)
    /// </summary>
    public Guid? ParentProfileId { get; set; }

    /// <summary>
    /// Gets or sets the parent profile
    /// </summary>
    public DocumentProfile? ParentProfile { get; set; }

    /// <summary>
    /// Gets or sets the full path of the profile (for hierarchy navigation)
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this profile is a template (can be used to create new profiles)
    /// </summary>
    public bool IsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the retention period in months (0 = permanent)
    /// </summary>
    public int RetentionPeriodMonths { get; set; }

    /// <summary>
    /// Gets or sets the profile status (Draft, Active, Archived, Closed)
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets when the profile was opened/started
    /// </summary>
    public DateTime? OpenedAt { get; set; }

    /// <summary>
    /// Gets or sets when the profile was closed
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of child profiles
    /// </summary>
    public ICollection<DocumentProfile> ChildProfiles { get; set; } = new List<DocumentProfile>();

    /// <summary>
    /// Gets or sets the collection of metadata fields defined for this profile
    /// </summary>
    public ICollection<ProfileMetadataField> MetadataFields { get; set; } = new List<ProfileMetadataField>();

    /// <summary>
    /// Gets or sets the collection of documents in this profile
    /// </summary>
    public ICollection<DocumentProfileDocument> Documents { get; set; } = new List<DocumentProfileDocument>();
}
