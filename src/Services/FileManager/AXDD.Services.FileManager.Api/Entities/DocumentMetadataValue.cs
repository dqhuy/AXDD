using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a custom metadata value for a document in a profile
/// Stores the actual values for configurable metadata fields
/// </summary>
public class DocumentMetadataValue : BaseEntity
{
    public DocumentMetadataValue()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the document ID this value belongs to
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the document this value belongs to
    /// </summary>
    public DocumentProfileDocument Document { get; set; } = null!;

    /// <summary>
    /// Gets or sets the metadata field ID
    /// </summary>
    public Guid MetadataFieldId { get; set; }

    /// <summary>
    /// Gets or sets the metadata field definition
    /// </summary>
    public ProfileMetadataField MetadataField { get; set; } = null!;

    /// <summary>
    /// Gets or sets the string value (for String, Select types)
    /// </summary>
    public string? StringValue { get; set; }

    /// <summary>
    /// Gets or sets the numeric value (for Number types)
    /// </summary>
    public decimal? NumberValue { get; set; }

    /// <summary>
    /// Gets or sets the date value (for Date types)
    /// </summary>
    public DateTime? DateValue { get; set; }

    /// <summary>
    /// Gets or sets the boolean value (for Boolean types)
    /// </summary>
    public bool? BooleanValue { get; set; }

    /// <summary>
    /// Gets or sets the JSON value (for complex/MultiSelect types)
    /// </summary>
    public string? JsonValue { get; set; }
}
