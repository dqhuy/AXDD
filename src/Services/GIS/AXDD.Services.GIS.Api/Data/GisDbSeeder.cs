using AXDD.Services.GIS.Api.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Data;

/// <summary>
/// Seeds initial data for GIS database
/// </summary>
public static class GisDbSeeder
{
    /// <summary>
    /// Seed industrial zones and sample enterprise locations
    /// </summary>
    public static async Task SeedDataAsync(GisDbContext context)
    {
        // Check if data already exists
        if (await context.IndustrialZones.AnyAsync())
        {
            return; // Data already seeded
        }

        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

        // Create Industrial Zone 1: KCN Biên Hòa 1
        var bienHoa1Coords = new[]
        {
            new Coordinate(106.8150, 10.9550), // lon, lat
            new Coordinate(106.8250, 10.9550),
            new Coordinate(106.8250, 10.9450),
            new Coordinate(106.8150, 10.9450),
            new Coordinate(106.8150, 10.9550)  // Close the ring
        };
        var bienHoa1Shell = geometryFactory.CreateLinearRing(bienHoa1Coords);
        var bienHoa1Boundary = geometryFactory.CreatePolygon(bienHoa1Shell);
        var bienHoa1Centroid = bienHoa1Boundary.Centroid;

        var bienHoa1 = new IndustrialZone
        {
            Name = "Khu Công Nghiệp Biên Hòa 1",
            Code = "KCN-BH1",
            Boundary = bienHoa1Boundary,
            AreaHectares = CalculateAreaHectares(bienHoa1Boundary),
            CentroidLatitude = bienHoa1Centroid.Y,
            CentroidLongitude = bienHoa1Centroid.X,
            Description = "Khu công nghiệp lớn nhất tại Biên Hòa, Đồng Nai. Chuyên về sản xuất điện tử và cơ khí.",
            Province = "Đồng Nai",
            District = "Biên Hòa",
            Status = IndustrialZoneStatus.Active,
            EstablishedYear = 1993
        };

        context.IndustrialZones.Add(bienHoa1);

        // Create Industrial Zone 2: KCN Long Thành
        var longThanhCoords = new[]
        {
            new Coordinate(106.9450, 10.7550),
            new Coordinate(106.9550, 10.7550),
            new Coordinate(106.9550, 10.7450),
            new Coordinate(106.9450, 10.7450),
            new Coordinate(106.9450, 10.7550)
        };
        var longThanhShell = geometryFactory.CreateLinearRing(longThanhCoords);
        var longThanhBoundary = geometryFactory.CreatePolygon(longThanhShell);
        var longThanhCentroid = longThanhBoundary.Centroid;

        var longThanh = new IndustrialZone
        {
            Name = "Khu Công Nghiệp Long Thành",
            Code = "KCN-LT",
            Boundary = longThanhBoundary,
            AreaHectares = CalculateAreaHectares(longThanhBoundary),
            CentroidLatitude = longThanhCentroid.Y,
            CentroidLongitude = longThanhCentroid.X,
            Description = "Khu công nghiệp gần sân bay Long Thành. Chuyên về logistics và sản xuất công nghệ cao.",
            Province = "Đồng Nai",
            District = "Long Thành",
            Status = IndustrialZoneStatus.Active,
            EstablishedYear = 2005
        };

        context.IndustrialZones.Add(longThanh);

        // Create Industrial Zone 3: KCN Nhơn Trạch
        var nhonTrachCoords = new[]
        {
            new Coordinate(106.9100, 10.7200),
            new Coordinate(106.9200, 10.7200),
            new Coordinate(106.9200, 10.7100),
            new Coordinate(106.9100, 10.7100),
            new Coordinate(106.9100, 10.7200)
        };
        var nhonTrachShell = geometryFactory.CreateLinearRing(nhonTrachCoords);
        var nhonTrachBoundary = geometryFactory.CreatePolygon(nhonTrachShell);
        var nhonTrachCentroid = nhonTrachBoundary.Centroid;

        var nhonTrach = new IndustrialZone
        {
            Name = "Khu Công Nghiệp Nhơn Trạch",
            Code = "KCN-NT",
            Boundary = nhonTrachBoundary,
            AreaHectares = CalculateAreaHectares(nhonTrachBoundary),
            CentroidLatitude = nhonTrachCentroid.Y,
            CentroidLongitude = nhonTrachCentroid.X,
            Description = "Khu công nghiệp mới đang phát triển tại Nhơn Trạch. Tập trung vào công nghiệp nặng và năng lượng.",
            Province = "Đồng Nai",
            District = "Nhơn Trạch",
            Status = IndustrialZoneStatus.UnderConstruction,
            EstablishedYear = 2018
        };

        context.IndustrialZones.Add(nhonTrach);

        // Add some sample enterprise locations within Biên Hòa 1
        var sampleEnterprises = new[]
        {
            new EnterpriseLocation
            {
                EnterpriseId = Guid.NewGuid(),
                EnterpriseCode = "DN001",
                EnterpriseName = "Công ty TNHH Điện tử ABC",
                Location = geometryFactory.CreatePoint(new Coordinate(106.8180, 10.9520)),
                Latitude = 10.9520,
                Longitude = 106.8180,
                Address = "Lô A1, Khu Công Nghiệp Biên Hòa 1, Đồng Nai",
                IsPrimary = true,
                IndustrialZoneId = bienHoa1.Id
            },
            new EnterpriseLocation
            {
                EnterpriseId = Guid.NewGuid(),
                EnterpriseCode = "DN002",
                EnterpriseName = "Công ty Cổ phần Cơ khí XYZ",
                Location = geometryFactory.CreatePoint(new Coordinate(106.8220, 10.9480)),
                Latitude = 10.9480,
                Longitude = 106.8220,
                Address = "Lô B5, Khu Công Nghiệp Biên Hòa 1, Đồng Nai",
                IsPrimary = true,
                IndustrialZoneId = bienHoa1.Id
            }
        };

        context.EnterpriseLocations.AddRange(sampleEnterprises);

        // Save all changes
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Calculate area in hectares (approximate)
    /// </summary>
    private static double CalculateAreaHectares(Polygon polygon)
    {
        // Rough calculation for demonstration
        // In production, use proper projection
        var areaDegrees = polygon.Area;
        var lat = polygon.Centroid.Y;
        var metersPerDegreeLat = 111320.0;
        var metersPerDegreeLon = 111320.0 * Math.Cos(lat * Math.PI / 180.0);
        var areaSquareMeters = areaDegrees * metersPerDegreeLat * metersPerDegreeLon;
        return areaSquareMeters / 10000.0; // Convert to hectares
    }
}
