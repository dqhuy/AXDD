using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AXDD.Services.GIS.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "IndustrialZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Boundary = table.Column<Polygon>(type: "geometry(Polygon, 4326)", nullable: false),
                    AreaHectares = table.Column<double>(type: "double precision", nullable: false),
                    CentroidLatitude = table.Column<double>(type: "double precision", nullable: false),
                    CentroidLongitude = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EstablishedYear = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrialZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnterpriseId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnterpriseCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EnterpriseName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Location = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Accuracy = table.Column<double>(type: "double precision", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IndustrialZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnterpriseLocations_IndustrialZones_IndustrialZoneId",
                        column: x => x.IndustrialZoneId,
                        principalTable: "IndustrialZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LandPlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlotNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IndustrialZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    Geometry = table.Column<Polygon>(type: "geometry(Polygon, 4326)", nullable: false),
                    AreaSquareMeters = table.Column<double>(type: "double precision", nullable: false),
                    OwnerEnterpriseId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerEnterpriseCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LeaseStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeaseEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PricePerSquareMeter = table.Column<decimal>(type: "numeric", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandPlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandPlots_IndustrialZones_IndustrialZoneId",
                        column: x => x.IndustrialZoneId,
                        principalTable: "IndustrialZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseLocations_EnterpriseCode",
                table: "EnterpriseLocations",
                column: "EnterpriseCode");

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseLocations_EnterpriseId",
                table: "EnterpriseLocations",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseLocations_IndustrialZoneId",
                table: "EnterpriseLocations",
                column: "IndustrialZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseLocations_Location",
                table: "EnterpriseLocations",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Boundary",
                table: "IndustrialZones",
                column: "Boundary")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Code",
                table: "IndustrialZones",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Name",
                table: "IndustrialZones",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Province",
                table: "IndustrialZones",
                column: "Province");

            migrationBuilder.CreateIndex(
                name: "IX_LandPlots_Geometry",
                table: "LandPlots",
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_LandPlots_IndustrialZoneId_PlotNumber",
                table: "LandPlots",
                columns: new[] { "IndustrialZoneId", "PlotNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandPlots_OwnerEnterpriseId",
                table: "LandPlots",
                column: "OwnerEnterpriseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnterpriseLocations");

            migrationBuilder.DropTable(
                name: "LandPlots");

            migrationBuilder.DropTable(
                name: "IndustrialZones");
        }
    }
}
