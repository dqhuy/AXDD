namespace AXDD.Services.Enterprise.Api.Application.DTOs;

/// <summary>
/// DTO for contact person details
/// </summary>
public class ContactPersonDto
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsMain { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Request DTO for creating a contact person
/// </summary>
public class CreateContactRequest
{
    public Guid EnterpriseId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsMain { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request DTO for updating a contact person
/// </summary>
public class UpdateContactRequest
{
    public string FullName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsMain { get; set; }
    public string? Notes { get; set; }
}
