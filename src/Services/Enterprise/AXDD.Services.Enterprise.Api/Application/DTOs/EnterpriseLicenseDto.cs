using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Application.DTOs;

/// <summary>
/// DTO for enterprise license details
/// </summary>
public class EnterpriseLicenseDto
{
    public Guid Id { get; set; }
    public Guid EnterpriseId { get; set; }
    public LicenseType LicenseType { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? FileId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Request DTO for creating a license
/// </summary>
public class CreateLicenseRequest
{
    public Guid EnterpriseId { get; set; }
    public LicenseType LicenseType { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? FileId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request DTO for updating a license
/// </summary>
public class UpdateLicenseRequest
{
    public LicenseType LicenseType { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? FileId { get; set; }
    public string? Notes { get; set; }
}
