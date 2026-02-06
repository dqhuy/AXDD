namespace AXDD.Services.GIS.Api.DTOs;

/// <summary>
/// Represents a geographic point with latitude and longitude
/// </summary>
public class PointDto
{
    /// <summary>
    /// Latitude in decimal degrees (WGS84)
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees (WGS84)
    /// </summary>
    public double Longitude { get; set; }
}

/// <summary>
/// Location data transfer object
/// </summary>
public class LocationDto
{
    /// <summary>
    /// Latitude in decimal degrees (WGS84)
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees (WGS84)
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// GPS accuracy in meters
    /// </summary>
    public double? Accuracy { get; set; }
}

/// <summary>
/// Request to save or update an enterprise location
/// </summary>
public class SaveEnterpriseLocationRequest
{
    /// <summary>
    /// Latitude in decimal degrees (WGS84)
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees (WGS84)
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// GPS accuracy in meters
    /// </summary>
    public double? Accuracy { get; set; }

    /// <summary>
    /// Notes or additional information
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this is the primary location
    /// </summary>
    public bool IsPrimary { get; set; } = true;
}

/// <summary>
/// Enterprise location data transfer object
/// </summary>
public class EnterpriseLocationDto
{
    /// <summary>
    /// Location identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Enterprise code
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Enterprise name
    /// </summary>
    public string EnterpriseName { get; set; } = string.Empty;

    /// <summary>
    /// Latitude in decimal degrees
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Distance in kilometers (populated for proximity queries)
    /// </summary>
    public double? DistanceKm { get; set; }

    /// <summary>
    /// Industrial zone name (if located in a zone)
    /// </summary>
    public string? IndustrialZoneName { get; set; }

    /// <summary>
    /// Industrial zone ID (if located in a zone)
    /// </summary>
    public Guid? IndustrialZoneId { get; set; }

    /// <summary>
    /// Whether this is the primary location
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// When the location was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the location was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
