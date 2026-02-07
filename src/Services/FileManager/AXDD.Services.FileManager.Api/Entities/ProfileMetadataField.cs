using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a configurable metadata field for a document profile
/// Admins can define custom fields for each profile type
/// </summary>
public class ProfileMetadataField : BaseEntity
{
    public ProfileMetadataField()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the profile ID this field belongs to (null if it's a global field)
    /// </summary>
    public Guid? ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the profile this field belongs to
    /// </summary>
    public DocumentProfile? Profile { get; set; }

    /// <summary>
    /// Gets or sets the field name/key (e.g., "so_giay_phep", "ngay_cap")
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label for the field (e.g., "Số giấy phép", "Ngày cấp")
    /// </summary>
    public string DisplayLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field data type (String, Number, Date, Boolean, Select, MultiSelect)
    /// </summary>
    public string DataType { get; set; } = "String";

    /// <summary>
    /// Gets or sets whether this field is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the default value for the field
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text for input
    /// </summary>
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the options for Select/MultiSelect fields (JSON array)
    /// </summary>
    public string? SelectOptions { get; set; }

    /// <summary>
    /// Gets or sets the validation regex pattern
    /// </summary>
    public string? ValidationPattern { get; set; }

    /// <summary>
    /// Gets or sets the validation error message
    /// </summary>
    public string? ValidationMessage { get; set; }

    /// <summary>
    /// Gets or sets the minimum value (for Number fields)
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// Gets or sets the maximum value (for Number fields)
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// Gets or sets the maximum length (for String fields)
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the display order of this field
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets whether this field is visible in list views
    /// </summary>
    public bool IsVisibleInList { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this field is searchable
    /// </summary>
    public bool IsSearchable { get; set; }

    /// <summary>
    /// Gets or sets whether this field is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the field description/help text
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Gets or sets the collection of metadata values for this field
    /// </summary>
    public ICollection<DocumentMetadataValue> MetadataValues { get; set; } = new List<DocumentMetadataValue>();
}
