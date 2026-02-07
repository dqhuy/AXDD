namespace AXDD.Services.GIS.Api.Settings;

/// <summary>
/// GIS configuration settings
/// </summary>
public class GisSettings
{
    /// <summary>
    /// Default Spatial Reference System Identifier (SRID)
    /// WGS84 = 4326
    /// </summary>
    public int DefaultSRID { get; set; } = 4326;

    /// <summary>
    /// Vietnam geographic bounds
    /// </summary>
    public VietnamBounds VietnamBounds { get; set; } = new();
}

/// <summary>
/// Vietnam geographic boundaries
/// </summary>
public class VietnamBounds
{
    /// <summary>
    /// Minimum latitude (southern boundary)
    /// </summary>
    public double MinLatitude { get; set; } = 8.0;

    /// <summary>
    /// Maximum latitude (northern boundary)
    /// </summary>
    public double MaxLatitude { get; set; } = 24.0;

    /// <summary>
    /// Minimum longitude (western boundary)
    /// </summary>
    public double MinLongitude { get; set; } = 102.0;

    /// <summary>
    /// Maximum longitude (eastern boundary)
    /// </summary>
    public double MaxLongitude { get; set; } = 110.0;

    /// <summary>
    /// Checks if coordinates are within Vietnam bounds
    /// </summary>
    public bool IsWithinBounds(double latitude, double longitude)
    {
        return latitude >= MinLatitude && latitude <= MaxLatitude &&
               longitude >= MinLongitude && longitude <= MaxLongitude;
    }
}
