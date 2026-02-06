using System.Text.Json.Serialization;

namespace AXDD.Services.GIS.Api.DTOs;

/// <summary>
/// GeoJSON Polygon structure
/// </summary>
public class PolygonDto
{
    /// <summary>
    /// GeoJSON type (always "Polygon")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Polygon";

    /// <summary>
    /// Array of coordinate rings
    /// First ring is exterior, subsequent rings are holes
    /// Format: [[[lon, lat], [lon, lat], ...]]
    /// </summary>
    [JsonPropertyName("coordinates")]
    public List<List<List<double>>> Coordinates { get; set; } = new();
}

/// <summary>
/// Industrial zone data transfer object
/// </summary>
public class IndustrialZoneDto
{
    /// <summary>
    /// Zone identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Zone name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Zone code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Zone boundary in GeoJSON format
    /// </summary>
    public PolygonDto? Boundary { get; set; }

    /// <summary>
    /// Area in hectares
    /// </summary>
    public double AreaHectares { get; set; }

    /// <summary>
    /// Centroid latitude
    /// </summary>
    public double CentroidLatitude { get; set; }

    /// <summary>
    /// Centroid longitude
    /// </summary>
    public double CentroidLongitude { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Province
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// District
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Year established
    /// </summary>
    public int? EstablishedYear { get; set; }

    /// <summary>
    /// Number of enterprises in the zone
    /// </summary>
    public int EnterpriseCount { get; set; }

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request to create an industrial zone
/// </summary>
public class CreateIndustrialZoneRequest
{
    /// <summary>
    /// Zone name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Zone code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Boundary polygon
    /// </summary>
    public PolygonDto Boundary { get; set; } = null!;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Province
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// District
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Year established
    /// </summary>
    public int? EstablishedYear { get; set; }
}

/// <summary>
/// Request to update an industrial zone boundary
/// </summary>
public class UpdateZoneBoundaryRequest
{
    /// <summary>
    /// New boundary polygon
    /// </summary>
    public PolygonDto Boundary { get; set; } = null!;
}

/// <summary>
/// Summary information about an industrial zone
/// </summary>
public class IndustrialZoneSummaryDto
{
    /// <summary>
    /// Zone identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Zone name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Zone code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Area in hectares
    /// </summary>
    public double AreaHectares { get; set; }

    /// <summary>
    /// Centroid latitude
    /// </summary>
    public double CentroidLatitude { get; set; }

    /// <summary>
    /// Centroid longitude
    /// </summary>
    public double CentroidLongitude { get; set; }

    /// <summary>
    /// Province
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of enterprises
    /// </summary>
    public int EnterpriseCount { get; set; }
}
