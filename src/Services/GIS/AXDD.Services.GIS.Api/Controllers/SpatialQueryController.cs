using AXDD.Services.GIS.Api.DTOs;
using AXDD.Services.GIS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.GIS.Api.Controllers;

/// <summary>
/// Controller for spatial query operations
/// </summary>
[ApiController]
[Route("api/v1/gis/spatial-query")]
[Produces("application/json")]
public class SpatialQueryController : ControllerBase
{
    private readonly ISpatialQueryService _spatialQueryService;
    private readonly ILogger<SpatialQueryController> _logger;

    public SpatialQueryController(
        ISpatialQueryService spatialQueryService,
        ILogger<SpatialQueryController> logger)
    {
        _spatialQueryService = spatialQueryService;
        _logger = logger;
    }

    /// <summary>
    /// Execute a spatial query
    /// </summary>
    /// <param name="request">Spatial query request</param>
    /// <returns>Query result</returns>
    /// <response code="200">Query executed successfully</response>
    /// <response code="400">Invalid query parameters</response>
    [HttpPost]
    [ProducesResponseType(typeof(SpatialQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<SpatialQueryResponse> ExecuteSpatialQuery([FromBody] SpatialQueryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = new SpatialQueryResponse
        {
            QueryType = request.Type.ToString()
        };

        switch (request.Type)
        {
            case SpatialQueryType.DistanceBetweenPoints:
                if (request.Point1 == null || request.Point2 == null)
                {
                    return BadRequest("Both Point1 and Point2 are required for distance calculation");
                }

                var point1 = _spatialQueryService.CreatePoint(request.Point1.Latitude, request.Point1.Longitude);
                var point2 = _spatialQueryService.CreatePoint(request.Point2.Latitude, request.Point2.Longitude);
                var distance = _spatialQueryService.DistanceBetween(point1, point2);

                response.Value = Math.Round(distance, 2);
                response.Message = $"Distance: {response.Value} km";
                break;

            case SpatialQueryType.PointInPolygon:
                if (request.Point1 == null || request.Polygon1 == null)
                {
                    return BadRequest("Point1 and Polygon1 are required for point-in-polygon query");
                }

                var point = _spatialQueryService.CreatePoint(request.Point1.Latitude, request.Point1.Longitude);
                var polygon = ConvertPolygonDtoToGeometry(request.Polygon1);
                var isInside = _spatialQueryService.PointInPolygon(point, polygon);

                response.BooleanResult = isInside;
                response.Message = isInside ? "Point is inside polygon" : "Point is outside polygon";
                break;

            case SpatialQueryType.BufferAroundPoint:
                if (request.Point1 == null || request.RadiusKm == null)
                {
                    return BadRequest("Point1 and RadiusKm are required for buffer query");
                }

                var centerPoint = _spatialQueryService.CreatePoint(request.Point1.Latitude, request.Point1.Longitude);
                var bufferPolygon = _spatialQueryService.BufferAroundPoint(centerPoint, request.RadiusKm.Value);

                response.ResultPolygon = ConvertGeometryToPolygonDto(bufferPolygon);
                response.Message = $"Buffer created with radius {request.RadiusKm} km";
                break;

            case SpatialQueryType.PolygonIntersection:
                if (request.Polygon1 == null || request.Polygon2 == null)
                {
                    return BadRequest("Both Polygon1 and Polygon2 are required for intersection query");
                }

                var polygon1 = ConvertPolygonDtoToGeometry(request.Polygon1);
                var polygon2 = ConvertPolygonDtoToGeometry(request.Polygon2);
                var intersects = _spatialQueryService.Intersects(polygon1, polygon2);

                response.BooleanResult = intersects;
                response.Message = intersects ? "Polygons intersect" : "Polygons do not intersect";
                break;

            default:
                return BadRequest($"Unsupported query type: {request.Type}");
        }

        _logger.LogInformation("Executed spatial query: {QueryType}", request.Type);

        return Ok(response);
    }

    /// <summary>
    /// Validate coordinates
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <returns>Validation result</returns>
    /// <response code="200">Validation completed</response>
    [HttpGet("validate-coordinates")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult ValidateCoordinates(
        [FromQuery] double latitude,
        [FromQuery] double longitude)
    {
        var isValid = _spatialQueryService.ValidateCoordinates(latitude, longitude);
        var isInVietnam = _spatialQueryService.ValidateVietnamBounds(latitude, longitude);

        return Ok(new
        {
            IsValid = isValid,
            IsInVietnam = isInVietnam,
            Latitude = latitude,
            Longitude = longitude
        });
    }

    /// <summary>
    /// Convert PolygonDto to NetTopologySuite Polygon
    /// </summary>
    private NetTopologySuite.Geometries.Polygon ConvertPolygonDtoToGeometry(PolygonDto polygonDto)
    {
        if (polygonDto?.Coordinates == null || polygonDto.Coordinates.Count == 0)
        {
            throw new ArgumentException("Invalid polygon: no coordinates provided");
        }

        var exteriorRing = polygonDto.Coordinates[0];
        var coords = exteriorRing.Select(c => new double[] { c[0], c[1] }).ToArray();

        // Remove duplicate last coordinate if present
        if (coords.Length > 3 &&
            coords[0][0] == coords[^1][0] &&
            coords[0][1] == coords[^1][1])
        {
            coords = coords[..^1];
        }

        return _spatialQueryService.CreatePolygon(coords);
    }

    /// <summary>
    /// Convert NetTopologySuite Polygon to PolygonDto
    /// </summary>
    private static PolygonDto ConvertGeometryToPolygonDto(NetTopologySuite.Geometries.Polygon polygon)
    {
        var exteriorRing = polygon.ExteriorRing.Coordinates
            .Select(c => new List<double> { c.X, c.Y })
            .ToList();

        return new PolygonDto
        {
            Type = "Polygon",
            Coordinates = new List<List<List<double>>> { exteriorRing }
        };
    }
}
