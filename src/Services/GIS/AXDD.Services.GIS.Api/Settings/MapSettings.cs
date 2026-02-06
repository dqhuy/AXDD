namespace AXDD.Services.GIS.Api.Settings;

/// <summary>
/// Map service configuration settings
/// </summary>
public class MapSettings
{
    /// <summary>
    /// OpenStreetMap tile server URL template
    /// {z} = zoom level, {x} = tile X coordinate, {y} = tile Y coordinate
    /// </summary>
    public string TileServerUrl { get; set; } = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

    /// <summary>
    /// Static map embed URL template
    /// </summary>
    public string StaticMapUrl { get; set; } = "https://www.openstreetmap.org/export/embed.html";

    /// <summary>
    /// Default zoom level for maps
    /// </summary>
    public int DefaultZoom { get; set; } = 12;

    /// <summary>
    /// Default map width in pixels
    /// </summary>
    public int DefaultWidth { get; set; } = 600;

    /// <summary>
    /// Default map height in pixels
    /// </summary>
    public int DefaultHeight { get; set; } = 450;

    /// <summary>
    /// User agent string for tile requests
    /// </summary>
    public string UserAgent { get; set; } = "AXDD-GIS-Service/1.0";
}
