using AXDD.Services.GIS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.GIS.Api.Controllers;

/// <summary>
/// Controller for map-related operations
/// </summary>
[ApiController]
[Route("api/v1/maps")]
[Produces("application/json")]
public class MapsController : ControllerBase
{
    private readonly IMapService _mapService;
    private readonly ILogger<MapsController> _logger;

    public MapsController(IMapService mapService, ILogger<MapsController> logger)
    {
        _mapService = mapService;
        _logger = logger;
    }

    /// <summary>
    /// Get OpenStreetMap tile URL
    /// </summary>
    /// <param name="zoom">Zoom level (0-19)</param>
    /// <param name="x">Tile X coordinate</param>
    /// <param name="y">Tile Y coordinate</param>
    /// <returns>Redirect to tile URL</returns>
    /// <response code="302">Redirect to tile</response>
    /// <response code="400">Invalid tile coordinates</response>
    [HttpGet("tiles/{zoom}/{x}/{y}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetMapTile(int zoom, int x, int y)
    {
        var tileUrl = _mapService.GetMapTileUrl(zoom, x, y);
        return Redirect(tileUrl);
    }

    /// <summary>
    /// Generate static map URL
    /// </summary>
    /// <param name="latitude">Center latitude</param>
    /// <param name="longitude">Center longitude</param>
    /// <param name="zoom">Zoom level (default 12)</param>
    /// <param name="width">Map width in pixels</param>
    /// <param name="height">Map height in pixels</param>
    /// <returns>Static map URL</returns>
    /// <response code="200">URL generated successfully</response>
    /// <response code="400">Invalid coordinates</response>
    [HttpGet("static")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult GetStaticMap(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int zoom = 12,
        [FromQuery] int? width = null,
        [FromQuery] int? height = null)
    {
        var mapUrl = _mapService.GenerateStaticMapUrl(
            latitude,
            longitude,
            zoom,
            markers: null,
            width,
            height);

        return Ok(new
        {
            MapUrl = mapUrl,
            Latitude = latitude,
            Longitude = longitude,
            Zoom = zoom
        });
    }

    /// <summary>
    /// Get map information for embedding
    /// </summary>
    /// <param name="latitude">Center latitude</param>
    /// <param name="longitude">Center longitude</param>
    /// <param name="zoom">Zoom level</param>
    /// <returns>Map embed information</returns>
    /// <response code="200">Information retrieved successfully</response>
    [HttpGet("embed-info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult GetEmbedInfo(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int zoom = 12)
    {
        var mapUrl = _mapService.GenerateStaticMapUrl(latitude, longitude, zoom);
        var tileUrl = _mapService.GetMapTileUrl(zoom, 0, 0);

        return Ok(new
        {
            EmbedUrl = mapUrl,
            TileServerUrl = tileUrl.Replace("/0/0", "/{z}/{x}/{y}"),
            Center = new { Latitude = latitude, Longitude = longitude },
            Zoom = zoom,
            Attribution = "© OpenStreetMap contributors"
        });
    }
}
