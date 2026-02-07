using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.Enterprise.Api.Domain.Entities;

/// <summary>
/// Represents a contact person for an enterprise
/// </summary>
public class ContactPerson : BaseEntity
{
    /// <summary>
    /// Gets or sets the enterprise ID
    /// </summary>
    public Guid EnterpriseId { get; set; }

    /// <summary>
    /// Gets or sets the full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position/title
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Gets or sets the department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets whether this is the main contact
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Gets or sets the notes
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the enterprise
    /// </summary>
    public EnterpriseEntity? Enterprise { get; set; }
}
