using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AXDD.Services.MasterData.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValidityPeriod = table.Column<int>(type: "int", nullable: true),
                    RequiringAuthority = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    AllowedExtensions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaxFileSizeMB = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndustryCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProvinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndustrialZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ProvinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Area = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstablishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManagementUnit = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrialZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustrialZones_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndustrialZones_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CertificateTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DisplayOrder", "IsActive", "IsDeleted", "IsRequired", "Name", "RequiringAuthority", "UpdatedAt", "UpdatedBy", "ValidityPeriod" },
                values: new object[,]
                {
                    { new Guid("0a622a1e-34ec-4783-836d-383ec3692dc2"), "ENV", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(9173), null, null, null, "Giấy phép môi trường hoặc văn bản xác nhận đăng ký kế hoạch bảo vệ môi trường", 3, true, false, true, "Giấy phép môi trường", "Sở Tài nguyên và Môi trường", null, null, 60 },
                    { new Guid("0ccbee44-ac87-4751-a171-77bde21e5efd"), "FIRE", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(9192), null, null, null, "Giấy chứng nhận đủ điều kiện về phòng cháy chữa cháy", 5, true, false, true, "Giấy chứng nhận đủ điều kiện về PCCC", "Công an tỉnh/thành phố", null, null, null },
                    { new Guid("24deca26-29bb-4c5f-bb82-045db628caf9"), "BUSINESS", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(9161), null, null, null, "Giấy chứng nhận đăng ký doanh nghiệp theo Luật Doanh nghiệp", 2, true, false, true, "Giấy chứng nhận đăng ký doanh nghiệp", "Sở Kế hoạch và Đầu tư", null, null, null },
                    { new Guid("5330f7ea-3bdc-47cf-a343-f29358e18309"), "CONSTRUCT", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(9182), null, null, null, "Giấy phép xây dựng công trình", 4, true, false, false, "Giấy phép xây dựng", "Sở Xây dựng", null, null, 24 },
                    { new Guid("8ece070a-1c02-4b51-b3ea-696832c94c17"), "INVEST", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(9111), null, null, null, "Giấy chứng nhận đăng ký đầu tư theo quy định của Luật Đầu tư", 1, true, false, true, "Giấy chứng nhận đầu tư", "Sở Kế hoạch và Đầu tư", null, null, 60 },
                    { new Guid("c5153112-5cde-4e4f-a61c-2084f688a7df"), "IMPORT", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(9206), null, null, null, "Giấy phép nhập khẩu hàng hoá, thiết bị", 6, true, false, false, "Giấy phép nhập khẩu", "Bộ Công Thương", null, null, 12 }
                });

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "DataType", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "IsSystem", "Key", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { new Guid("3d2eaeed-68f9-43e1-a291-a4270a1ee53b"), "System", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(9365), null, "Boolean", null, null, "Bật/tắt chế độ bảo trì hệ thống", true, false, true, "System.MaintenanceMode", null, null, "false" },
                    { new Guid("80c0dbb1-c19e-4964-9621-899ada66a9e9"), "Email", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(9449), null, "String", null, null, "Địa chỉ email người gửi", true, false, false, "Email.SenderAddress", null, null, "noreply@axdd.gov.vn" },
                    { new Guid("8345ffe6-4b8f-47ad-99b5-4ca1ccaf26b9"), "Report", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(9426), null, "Int", null, null, "Số ngày hạn nộp báo cáo thường niên (sau khi kết thúc năm tài chính)", true, false, false, "Report.AnnualDeadlineDays", null, null, "30" },
                    { new Guid("9922b1ae-c887-4b0c-b556-5a9e7f108262"), "Email", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(9435), null, "Boolean", null, null, "Bật/tắt gửi email thông báo", true, false, false, "Email.NotificationEnabled", null, null, "true" },
                    { new Guid("caf0f4f2-cb51-49ff-9311-95c394a715aa"), "Report", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(9417), null, "Int", null, null, "Số ngày hạn nộp báo cáo định kỳ (sau khi kết thúc kỳ báo cáo)", true, false, false, "Report.QuarterlyDeadlineDays", null, null, "15" },
                    { new Guid("e4f4863f-beea-4a86-8b1d-624c4eaf1162"), "System", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(9407), null, "String", null, null, "Ngôn ngữ mặc định của hệ thống", true, false, true, "System.DefaultLanguage", null, null, "vi-VN" }
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "Id", "AllowedExtensions", "Category", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DisplayOrder", "IsActive", "IsDeleted", "IsRequired", "MaxFileSizeMB", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("057ce934-7952-4dfa-a181-d1da90814381"), ".pdf", "Legal", "DOC_LAND_LEASE", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5702), null, null, null, null, 4, true, false, true, 5, "Hợp đồng thuê đất", null, null },
                    { new Guid("29310468-cc05-4544-9ef8-d51fb4868090"), ".pdf", "Financial", "DOC_TAX_CODE", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5740), null, null, null, null, 6, true, false, true, 5, "Giấy chứng nhận mã số thuế", null, null },
                    { new Guid("2ea95acf-f95f-478f-824a-2c1419cc5559"), ".pdf,.doc,.docx", "Investment", "DOC_FEASIBILITY", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5682), null, null, null, null, 2, true, false, true, 20, "Báo cáo khả thi", null, null },
                    { new Guid("49d2c385-98a6-480c-9459-41e022108a77"), ".pdf,.xls,.xlsx", "Financial", "DOC_FINANCIAL_STMT", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5749), null, null, null, null, 7, true, false, false, 10, "Báo cáo tài chính", null, null },
                    { new Guid("878492d3-9229-46ee-8713-49c2345a577f"), ".pdf,.doc,.docx", "Investment", "DOC_INVEST_PROPOSAL", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5634), null, null, null, null, 1, true, false, true, 10, "Đề xuất dự án đầu tư", null, null },
                    { new Guid("bdf99f04-1cca-414c-9ba9-9c614482c227"), ".pdf,.jpg,.png", "Legal", "DOC_PASSPORT", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5758), null, null, null, null, 8, true, false, false, 5, "Hộ chiếu (đối với nhà đầu tư nước ngoài)", null, null },
                    { new Guid("cc14dd9b-0a65-4f06-8110-ec0e00057c20"), ".pdf", "Legal", "DOC_BUSINESS_REG", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5711), null, null, null, null, 5, true, false, true, 5, "Giấy chứng nhận đăng ký kinh doanh", null, null },
                    { new Guid("cd094cb4-a7a6-418c-b93d-4d46193549de"), ".pdf,.doc,.docx", "Environment", "DOC_ENV_IMPACT", new DateTime(2026, 2, 7, 1, 31, 15, 433, DateTimeKind.Utc).AddTicks(5693), null, null, null, null, 3, true, false, true, 20, "Báo cáo đánh giá tác động môi trường", null, null }
                });

            migrationBuilder.InsertData(
                table: "IndustryCodes",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DisplayOrder", "IsActive", "IsDeleted", "Level", "Name", "ParentCode", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("0bea4340-43cd-4399-ae5d-84e3517b2dc8"), "C15", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2390), null, null, null, null, 5, true, false, 2, "Sản xuất da và các sản phẩm có liên quan", "C", null, null },
                    { new Guid("0f1c2d5a-174e-4037-bb16-4ec5114d23e8"), "C24", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2474), null, null, null, null, 13, true, false, 2, "Sản xuất kim loại", "C", null, null },
                    { new Guid("1f8c212d-3b99-4092-9c94-7abf1eb91154"), "C27", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2500), null, null, null, null, 16, true, false, 2, "Sản xuất thiết bị điện", "C", null, null },
                    { new Guid("2969f223-7e88-4b1b-8e14-cb8377090336"), "C101", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2549), null, null, null, null, 1, true, false, 3, "Chế biến và bảo quản thịt và các sản phẩm từ thịt", "C10", null, null },
                    { new Guid("29ae8896-e5d0-46a1-bcea-6123b430effc"), "C108", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2608), null, null, null, null, 8, true, false, 3, "Sản xuất thức ăn gia súc, gia cầm và thuỷ sản", "C10", null, null },
                    { new Guid("2a5a2195-a86e-4c5f-b654-a9eda0c28add"), "J", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1326), null, null, null, null, 10, true, false, 1, "Thông tin và truyền thông", null, null, null },
                    { new Guid("2b5c7e53-ab4c-412b-833f-0b0850f41be0"), "C11", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2364), null, null, null, null, 2, true, false, 2, "Sản xuất đồ uống", "C", null, null },
                    { new Guid("2dc26122-05ba-44ce-a57f-59254ed2c78e"), "C16", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2399), null, null, null, null, 6, true, false, 2, "Chế biến gỗ và sản xuất sản phẩm từ gỗ, tre, nứa", "C", null, null },
                    { new Guid("2e1d6e89-f461-481c-a3da-fc4326ac0f31"), "C18", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2415), null, null, null, null, 8, true, false, 2, "In, sao chép bản ghi các loại", "C", null, null },
                    { new Guid("4f9c0644-7330-4936-b88e-6db923892c4b"), "C29", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2517), null, null, null, null, 18, true, false, 2, "Sản xuất xe có động cơ", "C", null, null },
                    { new Guid("5739d61d-5abf-4f5f-9493-3b5e45dd2361"), "C105", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2583), null, null, null, null, 5, true, false, 3, "Sản xuất sản phẩm từ sữa", "C10", null, null },
                    { new Guid("5970732d-4758-45a2-81e2-9c924438eeac"), "C", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1265), null, null, null, null, 3, true, false, 1, "Công nghiệp chế biến, chế tạo", null, null, null },
                    { new Guid("59d90e61-d1b4-4c84-9e7b-82286d262e64"), "G", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1300), null, null, null, null, 7, true, false, 1, "Bán buôn và bán lẻ; sửa chữa ô tô, mô tô, xe máy và xe có động cơ khác", null, null, null },
                    { new Guid("5eef4bb8-a501-40b0-952f-6a5e5a75995f"), "C102", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2558), null, null, null, null, 2, true, false, 3, "Chế biến và bảo quản thuỷ sản và các sản phẩm từ thuỷ sản", "C10", null, null },
                    { new Guid("681e3c10-02f3-4465-a834-d7c5ab5208f1"), "H", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1309), null, null, null, null, 8, true, false, 1, "Vận tải, kho bãi", null, null, null },
                    { new Guid("72708ff7-20c6-4242-9678-391079af7786"), "C21", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2432), null, null, null, null, 10, true, false, 2, "Sản xuất thuốc, hoá dược và dược liệu", "C", null, null },
                    { new Guid("77c8e099-83a5-419b-afaa-e14d8cf71386"), "C14", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2382), null, null, null, null, 4, true, false, 2, "Sản xuất trang phục", "C", null, null },
                    { new Guid("8e029743-27bc-4ceb-b625-d1b4e91c7213"), "C106", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2591), null, null, null, null, 6, true, false, 3, "Sản xuất sản phẩm từ xay xát, bột và tinh bột", "C10", null, null },
                    { new Guid("8f53e2f4-11df-4555-865a-b92d778714ac"), "C17", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2407), null, null, null, null, 7, true, false, 2, "Sản xuất giấy và sản phẩm từ giấy", "C", null, null },
                    { new Guid("915b7639-c4ac-4db1-b674-03bc06b3d1fb"), "C25", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2483), null, null, null, null, 14, true, false, 2, "Sản xuất sản phẩm từ kim loại đúc sẵn (trừ máy móc, thiết bị)", "C", null, null },
                    { new Guid("946a8f30-26bc-499e-b160-02234e74bcd0"), "C26", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2492), null, null, null, null, 15, true, false, 2, "Sản xuất sản phẩm điện tử, máy vi tính và sản phẩm quang học", "C", null, null },
                    { new Guid("95405d57-3623-4347-83e9-587e14e314ac"), "C104", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2574), null, null, null, null, 4, true, false, 3, "Sản xuất dầu và mỡ động, thực vật", "C10", null, null },
                    { new Guid("9f3ec9ac-cf1c-4ce1-b31e-6064a00cf5d3"), "D", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1274), null, null, null, null, 4, true, false, 1, "Sản xuất và phân phối điện, khí đốt, nước nóng, hơi nước và điều hòa không khí", null, null, null },
                    { new Guid("a4228681-f140-450f-a3b1-7f17aca8214d"), "C22", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2441), null, null, null, null, 11, true, false, 2, "Sản xuất sản phẩm từ cao su và plastic", "C", null, null },
                    { new Guid("aaf35522-43cb-4aab-81ce-2de0c86e6a61"), "E", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1283), null, null, null, null, 5, true, false, 1, "Cung cấp nước; hoạt động quản lý và xử lý rác thải, nước thải", null, null, null },
                    { new Guid("ae23afd2-11bd-4fcf-a446-2038cc5f6b0d"), "C103", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2566), null, null, null, null, 3, true, false, 3, "Chế biến và bảo quản rau quả", "C10", null, null },
                    { new Guid("b6481a00-3a12-4abe-98ad-ce576a97f2ed"), "C10", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2346), null, null, null, null, 1, true, false, 2, "Sản xuất thực phẩm", "C", null, null },
                    { new Guid("b9954533-d8ff-4def-b863-1476b610776b"), "C30", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2526), null, null, null, null, 19, true, false, 2, "Sản xuất phương tiện vận tải khác", "C", null, null },
                    { new Guid("bdae7fce-36fc-4a0b-ba02-d69a3b2c8d19"), "F", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1291), null, null, null, null, 6, true, false, 1, "Xây dựng", null, null, null },
                    { new Guid("c57afb95-7901-414b-b56c-d21db095ca3a"), "C20", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2424), null, null, null, null, 9, true, false, 2, "Sản xuất hoá chất và sản phẩm hoá chất", "C", null, null },
                    { new Guid("cd297974-9eda-45ac-9840-9b99a07f4e8c"), "C1012", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2905), null, null, null, "Sản xuất các sản phẩm chế biến từ thịt như xúc xích, giăm bông, thịt xông khói", 2, true, false, 4, "Sản xuất sản phẩm từ thịt", "C101", null, null },
                    { new Guid("d93ac620-d33f-4de4-93c8-9622eccca3be"), "A", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1231), null, null, null, null, 1, true, false, 1, "Nông nghiệp, lâm nghiệp và thuỷ sản", null, null, null },
                    { new Guid("e06e137a-59b1-4dd5-89ea-73585518e6af"), "C23", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2449), null, null, null, null, 12, true, false, 2, "Sản xuất sản phẩm từ khoáng phi kim loại khác", "C", null, null },
                    { new Guid("e76c5a6e-8554-4170-b946-8fda39cfae47"), "C28", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2509), null, null, null, null, 17, true, false, 2, "Sản xuất máy móc, thiết bị chưa được phân vào đâu", "C", null, null },
                    { new Guid("e81d1068-53a4-4860-9662-640bedfd1552"), "B", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1255), null, null, null, null, 2, true, false, 1, "Khai khoáng", null, null, null },
                    { new Guid("ece99b09-d4d0-44ed-9c6b-6835f6545084"), "I", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(1317), null, null, null, null, 9, true, false, 1, "Dịch vụ lưu trú và ăn uống", null, null, null },
                    { new Guid("edd59da3-ea20-49c2-b0eb-bd3225bc6595"), "C13", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2373), null, null, null, null, 3, true, false, 2, "Dệt", "C", null, null },
                    { new Guid("f025f809-aa2e-4cd0-aedd-7687967862ec"), "C1011", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2889), null, null, null, "Chế biến, bảo quản thịt gia súc, gia cầm và các loại thịt khác", 1, true, false, 4, "Chế biến và bảo quản thịt", "C101", null, null },
                    { new Guid("f0bd8f8d-2546-4406-90de-d84d3a5d06a1"), "C107", new DateTime(2026, 2, 7, 1, 31, 15, 432, DateTimeKind.Utc).AddTicks(2600), null, null, null, null, 7, true, false, 3, "Sản xuất các sản phẩm khác từ thực phẩm", "C10", null, null }
                });

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DisplayOrder", "IsDeleted", "Name", "Region", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("01000000-0000-0000-0000-000000000001"), "01", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9149), null, null, null, 1, false, "Hà Nội", "North", null, null },
                    { new Guid("02000000-0000-0000-0000-000000000002"), "02", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9380), null, null, null, 2, false, "Hà Giang", "North", null, null },
                    { new Guid("04000000-0000-0000-0000-000000000004"), "04", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9384), null, null, null, 3, false, "Cao Bằng", "North", null, null },
                    { new Guid("06000000-0000-0000-0000-000000000006"), "06", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9387), null, null, null, 4, false, "Bắc Kạn", "North", null, null },
                    { new Guid("08000000-0000-0000-0000-000000000008"), "08", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9390), null, null, null, 5, false, "Tuyên Quang", "North", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "10", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9397), null, null, null, 6, false, "Lào Cai", "North", null, null },
                    { new Guid("11000000-0000-0000-0000-000000000011"), "11", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9399), null, null, null, 7, false, "Điện Biên", "North", null, null },
                    { new Guid("12000000-0000-0000-0000-000000000012"), "12", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9401), null, null, null, 8, false, "Lai Châu", "North", null, null },
                    { new Guid("14000000-0000-0000-0000-000000000014"), "14", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9404), null, null, null, 9, false, "Sơn La", "North", null, null },
                    { new Guid("15000000-0000-0000-0000-000000000015"), "15", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9407), null, null, null, 10, false, "Yên Bái", "North", null, null },
                    { new Guid("17000000-0000-0000-0000-000000000017"), "17", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9410), null, null, null, 11, false, "Hòa Bình", "North", null, null },
                    { new Guid("19000000-0000-0000-0000-000000000019"), "19", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9412), null, null, null, 12, false, "Thái Nguyên", "North", null, null },
                    { new Guid("20000000-0000-0000-0000-000000000020"), "20", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9414), null, null, null, 13, false, "Lạng Sơn", "North", null, null },
                    { new Guid("22000000-0000-0000-0000-000000000022"), "22", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9416), null, null, null, 14, false, "Quảng Ninh", "North", null, null },
                    { new Guid("24000000-0000-0000-0000-000000000024"), "24", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9418), null, null, null, 15, false, "Bắc Giang", "North", null, null },
                    { new Guid("25000000-0000-0000-0000-000000000025"), "25", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9421), null, null, null, 16, false, "Phú Thọ", "North", null, null },
                    { new Guid("26000000-0000-0000-0000-000000000026"), "26", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9437), null, null, null, 17, false, "Vĩnh Phúc", "North", null, null },
                    { new Guid("27000000-0000-0000-0000-000000000027"), "27", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9440), null, null, null, 18, false, "Bắc Ninh", "North", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000030"), "30", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9442), null, null, null, 19, false, "Hải Dương", "North", null, null },
                    { new Guid("31000000-0000-0000-0000-000000000031"), "31", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9445), null, null, null, 20, false, "Hải Phòng", "North", null, null },
                    { new Guid("33000000-0000-0000-0000-000000000033"), "33", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9447), null, null, null, 21, false, "Hưng Yên", "North", null, null },
                    { new Guid("34000000-0000-0000-0000-000000000034"), "34", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9449), null, null, null, 22, false, "Thái Bình", "North", null, null },
                    { new Guid("35000000-0000-0000-0000-000000000035"), "35", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9451), null, null, null, 23, false, "Hà Nam", "North", null, null },
                    { new Guid("36000000-0000-0000-0000-000000000036"), "36", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9454), null, null, null, 24, false, "Nam Định", "North", null, null },
                    { new Guid("37000000-0000-0000-0000-000000000037"), "37", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9456), null, null, null, 25, false, "Ninh Bình", "North", null, null },
                    { new Guid("38000000-0000-0000-0000-000000000038"), "38", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9458), null, null, null, 26, false, "Thanh Hóa", "Central", null, null },
                    { new Guid("40000000-0000-0000-0000-000000000040"), "40", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9460), null, null, null, 27, false, "Nghệ An", "Central", null, null },
                    { new Guid("42000000-0000-0000-0000-000000000042"), "42", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9462), null, null, null, 28, false, "Hà Tĩnh", "Central", null, null },
                    { new Guid("44000000-0000-0000-0000-000000000044"), "44", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9464), null, null, null, 29, false, "Quảng Bình", "Central", null, null },
                    { new Guid("45000000-0000-0000-0000-000000000045"), "45", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9467), null, null, null, 30, false, "Quảng Trị", "Central", null, null },
                    { new Guid("46000000-0000-0000-0000-000000000046"), "46", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9469), null, null, null, 31, false, "Thừa Thiên Huế", "Central", null, null },
                    { new Guid("48000000-0000-0000-0000-000000000048"), "48", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9471), null, null, null, 32, false, "Đà Nẵng", "Central", null, null },
                    { new Guid("49000000-0000-0000-0000-000000000049"), "49", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9473), null, null, null, 33, false, "Quảng Nam", "Central", null, null },
                    { new Guid("51000000-0000-0000-0000-000000000051"), "51", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9476), null, null, null, 34, false, "Quảng Ngãi", "Central", null, null },
                    { new Guid("52000000-0000-0000-0000-000000000052"), "52", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9479), null, null, null, 35, false, "Bình Định", "Central", null, null },
                    { new Guid("54000000-0000-0000-0000-000000000054"), "54", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9481), null, null, null, 36, false, "Phú Yên", "Central", null, null },
                    { new Guid("56000000-0000-0000-0000-000000000056"), "56", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9483), null, null, null, 37, false, "Khánh Hòa", "Central", null, null },
                    { new Guid("58000000-0000-0000-0000-000000000058"), "58", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9485), null, null, null, 38, false, "Ninh Thuận", "Central", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000060"), "60", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9487), null, null, null, 39, false, "Bình Thuận", "Central", null, null },
                    { new Guid("62000000-0000-0000-0000-000000000062"), "62", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9489), null, null, null, 40, false, "Kon Tum", "Central", null, null },
                    { new Guid("64000000-0000-0000-0000-000000000064"), "64", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9492), null, null, null, 41, false, "Gia Lai", "Central", null, null },
                    { new Guid("66000000-0000-0000-0000-000000000066"), "66", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9494), null, null, null, 42, false, "Đắk Lắk", "Central", null, null },
                    { new Guid("67000000-0000-0000-0000-000000000067"), "67", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9496), null, null, null, 43, false, "Đắk Nông", "Central", null, null },
                    { new Guid("68000000-0000-0000-0000-000000000068"), "68", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9498), null, null, null, 44, false, "Lâm Đồng", "Central", null, null },
                    { new Guid("70000000-0000-0000-0000-000000000070"), "70", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9500), null, null, null, 45, false, "Bình Phước", "South", null, null },
                    { new Guid("72000000-0000-0000-0000-000000000072"), "72", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9508), null, null, null, 46, false, "Tây Ninh", "South", null, null },
                    { new Guid("74000000-0000-0000-0000-000000000074"), "74", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9510), null, null, null, 47, false, "Bình Dương", "South", null, null },
                    { new Guid("75000000-0000-0000-0000-000000000075"), "75", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9512), null, null, null, 48, false, "Đồng Nai", "South", null, null },
                    { new Guid("77000000-0000-0000-0000-000000000077"), "77", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9515), null, null, null, 49, false, "Bà Rịa - Vũng Tàu", "South", null, null },
                    { new Guid("79000000-0000-0000-0000-000000000079"), "79", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9517), null, null, null, 50, false, "Hồ Chí Minh", "South", null, null },
                    { new Guid("80000000-0000-0000-0000-000000000080"), "80", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9519), null, null, null, 51, false, "Long An", "South", null, null },
                    { new Guid("82000000-0000-0000-0000-000000000082"), "82", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9521), null, null, null, 52, false, "Tiền Giang", "South", null, null },
                    { new Guid("83000000-0000-0000-0000-000000000083"), "83", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9523), null, null, null, 53, false, "Bến Tre", "South", null, null },
                    { new Guid("84000000-0000-0000-0000-000000000084"), "84", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9526), null, null, null, 54, false, "Trà Vinh", "South", null, null },
                    { new Guid("86000000-0000-0000-0000-000000000086"), "86", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9528), null, null, null, 55, false, "Vĩnh Long", "South", null, null },
                    { new Guid("87000000-0000-0000-0000-000000000087"), "87", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9530), null, null, null, 56, false, "Đồng Tháp", "South", null, null },
                    { new Guid("89000000-0000-0000-0000-000000000089"), "89", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9532), null, null, null, 57, false, "An Giang", "South", null, null },
                    { new Guid("91000000-0000-0000-0000-000000000091"), "91", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9534), null, null, null, 58, false, "Kiên Giang", "South", null, null },
                    { new Guid("92000000-0000-0000-0000-000000000092"), "92", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9536), null, null, null, 59, false, "Cần Thơ", "South", null, null },
                    { new Guid("93000000-0000-0000-0000-000000000093"), "93", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9539), null, null, null, 60, false, "Hậu Giang", "South", null, null },
                    { new Guid("94000000-0000-0000-0000-000000000094"), "94", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9541), null, null, null, 61, false, "Sóc Trăng", "South", null, null },
                    { new Guid("95000000-0000-0000-0000-000000000095"), "95", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9543), null, null, null, 62, false, "Bạc Liêu", "South", null, null },
                    { new Guid("96000000-0000-0000-0000-000000000096"), "96", new DateTime(2026, 2, 7, 1, 31, 15, 428, DateTimeKind.Utc).AddTicks(9545), null, null, null, 63, false, "Cà Mau", "South", null, null }
                });

            migrationBuilder.InsertData(
                table: "StatusCodes",
                columns: new[] { "Id", "Code", "Color", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DisplayOrder", "EntityType", "IsActive", "IsDeleted", "IsFinal", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("441440a3-17e8-4dda-8824-58c58e57f1d4"), "CLOSED", "#dc3545", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(3452), null, null, null, null, 3, "Enterprise", true, false, true, "Đã đóng", null, null },
                    { new Guid("4b825bc1-f364-4189-b3e6-8ba8d89dd595"), "PENDING", "#6c757d", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4365), null, null, null, null, 1, "Report", true, false, false, "Chờ nộp", null, null },
                    { new Guid("4df34023-373e-45c8-b9f7-a83244527e95"), "SUBMITTED", "#17a2b8", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4280), null, null, null, null, 2, "Project", true, false, false, "Đã nộp", null, null },
                    { new Guid("542723ee-1bc3-448f-b804-9d59b9968966"), "COMPLETED", "#28a745", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4337), null, null, null, null, 7, "Project", true, false, true, "Đã hoàn thành", null, null },
                    { new Guid("60e65b2c-109a-420c-bc2e-51ec23e74648"), "IN_PROGRESS", "#007bff", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4328), null, null, null, null, 6, "Project", true, false, false, "Đang thực hiện", null, null },
                    { new Guid("63d2fe34-13c5-48a9-a377-9372fb0a78bb"), "OVERDUE", "#dc3545", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4383), null, null, null, null, 3, "Report", true, false, false, "Quá hạn", null, null },
                    { new Guid("702d2bfb-42b7-474b-9669-bb3de8f6f69d"), "REJECTED", "#dc3545", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4316), null, null, null, null, 5, "Project", true, false, true, "Từ chối", null, null },
                    { new Guid("77a3e4dc-b294-49af-9636-112f3ccd7829"), "CANCELLED", "#dc3545", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4345), null, null, null, null, 8, "Project", true, false, true, "Đã hủy", null, null },
                    { new Guid("9106c9ea-9d0d-4742-94c2-1010b4f27dc4"), "DRAFT", "#6c757d", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4265), null, null, null, null, 1, "Project", true, false, false, "Nháp", null, null },
                    { new Guid("973e5c33-a3b0-45b2-bec4-f4999e976a75"), "ACTIVE", "#28a745", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(3417), null, null, null, null, 1, "Enterprise", true, false, false, "Hoạt động", null, null },
                    { new Guid("b96ad0cd-c619-4d28-9c2e-a8537c1eaa74"), "UNDER_REVIEW", "#ffc107", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4293), null, null, null, null, 3, "Project", true, false, false, "Đang thẩm định", null, null },
                    { new Guid("ba5f7a38-397b-4480-b47c-6acfe7c9d5e4"), "APPROVED", "#28a745", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4305), null, null, null, null, 4, "Project", true, false, false, "Đã phê duyệt", null, null },
                    { new Guid("bd7a7885-e9dc-4108-ba8f-1f29eaf02dde"), "SUSPENDED", "#ffc107", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(3441), null, null, null, null, 2, "Enterprise", true, false, false, "Tạm ngừng", null, null },
                    { new Guid("c4e2d77d-be37-4f7b-bb54-6a0224cec68d"), "SUBMITTED", "#28a745", new DateTime(2026, 2, 7, 1, 31, 15, 434, DateTimeKind.Utc).AddTicks(4374), null, null, null, null, 2, "Report", true, false, true, "Đã nộp", null, null }
                });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DisplayOrder", "IsDeleted", "Name", "ProvinceId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("21bbbfe7-d0b2-4196-93ae-61389562a2ae"), "7502", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1133), null, null, null, 2, false, "Thành phố Long Khánh", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("44e127bd-9909-49ad-a597-5667e2bafa9f"), "7504", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1154), null, null, null, 4, false, "Huyện Vĩnh Cửu", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("5096649a-af9e-473f-b704-95e8b6a87a73"), "7507", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1185), null, null, null, 7, false, "Huyện Thống Nhất", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("62667c1f-73dd-4d59-a683-b8f803f233a7"), "7510", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1227), null, null, null, 10, false, "Huyện Xuân Lộc", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("793068f1-bcb4-42cd-ae11-c45e88823a6c"), "7506", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1177), null, null, null, 6, false, "Huyện Trảng Bom", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("851acf74-dd60-43cf-a55a-93143759806d"), "7508", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1194), null, null, null, 8, false, "Huyện Cẩm Mỹ", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("90cc53a2-2d02-4c5a-851d-4af4647c400c"), "7511", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1235), null, null, null, 11, false, "Huyện Nhơn Trạch", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("9132141d-3a32-4f44-b243-941d5709a98f"), "7501", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1087), null, null, null, 1, false, "Thành phố Biên Hòa", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("ac196ddb-1409-4462-8fd9-9cf7d06b73e3"), "7505", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1163), null, null, null, 5, false, "Huyện Định Quán", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("bdb9942b-76bb-43a7-9a96-4a97672a6b9d"), "7503", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1145), null, null, null, 3, false, "Huyện Tân Phú", new Guid("75000000-0000-0000-0000-000000000075"), null, null },
                    { new Guid("f4cb19f7-87fb-446d-9bbc-f2dfa89a2ed6"), "7509", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(1216), null, null, null, 9, false, "Huyện Long Thành", new Guid("75000000-0000-0000-0000-000000000075"), null, null }
                });

            migrationBuilder.InsertData(
                table: "IndustrialZones",
                columns: new[] { "Id", "Area", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DistrictId", "EstablishedDate", "IsDeleted", "ManagementUnit", "Name", "ProvinceId", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("6ff9e92f-8505-48da-81e0-8a359806a427"), 700m, "DN-AMATA", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(7793), null, null, null, "Khu công nghiệp lớn, thu hút nhiều nhà đầu tư nước ngoài", null, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Amata Corporation", "Khu công nghiệp Amata", new Guid("75000000-0000-0000-0000-000000000075"), "Active", null, null },
                    { new Guid("79b3f536-0b09-4e15-b77e-3a64e8d9f817"), 1000m, "DN-NHONTRACH", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(7877), null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ban quản lý các KCN Đồng Nai", "Khu công nghiệp Nhơn Trạch", new Guid("75000000-0000-0000-0000-000000000075"), "Active", null, null },
                    { new Guid("9ac1e4a3-a0c5-46b1-af13-2af087186ff5"), 315m, "DN-LOTE-PHUHUU", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(7868), null, null, null, null, null, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ban quản lý các KCN Đồng Nai", "Khu công nghiệp Lộ Tẻ - Phú Hữu", new Guid("75000000-0000-0000-0000-000000000075"), "Active", null, null },
                    { new Guid("9c1232b3-5773-44c3-9b91-126204e512a4"), 500m, "DN-LONGTHANH", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(7858), null, null, null, null, null, new DateTime(1997, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ban quản lý các KCN Đồng Nai", "Khu công nghiệp Long Thành", new Guid("75000000-0000-0000-0000-000000000075"), "Active", null, null },
                    { new Guid("d2f19822-3630-4e90-8778-8d1ae8c5e197"), 426m, "DN-BIENHOA1", new DateTime(2026, 2, 7, 1, 31, 15, 430, DateTimeKind.Utc).AddTicks(7847), null, null, null, null, null, new DateTime(1996, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ban quản lý các KCN Đồng Nai", "Khu công nghiệp Biên Hòa 1", new Guid("75000000-0000-0000-0000-000000000075"), "Active", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTypes_Code",
                table: "CertificateTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTypes_IsActive",
                table: "CertificateTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTypes_Name",
                table: "CertificateTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_Category",
                table: "Configurations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_IsActive",
                table: "Configurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_Key",
                table: "Configurations",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_Code",
                table: "Districts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_Name",
                table: "Districts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Category",
                table: "DocumentTypes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Code",
                table: "DocumentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_IsActive",
                table: "DocumentTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Name",
                table: "DocumentTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Code",
                table: "IndustrialZones",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_DistrictId",
                table: "IndustrialZones",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Name",
                table: "IndustrialZones",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_ProvinceId",
                table: "IndustrialZones",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialZones_Status",
                table: "IndustrialZones",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryCodes_Code",
                table: "IndustryCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryCodes_IsActive",
                table: "IndustryCodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryCodes_Level",
                table: "IndustryCodes",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryCodes_Name",
                table: "IndustryCodes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryCodes_ParentCode",
                table: "IndustryCodes",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Code",
                table: "Provinces",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Name",
                table: "Provinces",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Region",
                table: "Provinces",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_StatusCodes_Code",
                table: "StatusCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_StatusCodes_Code_EntityType",
                table: "StatusCodes",
                columns: new[] { "Code", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusCodes_EntityType",
                table: "StatusCodes",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_StatusCodes_IsActive",
                table: "StatusCodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_Code",
                table: "Wards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictId",
                table: "Wards",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_Name",
                table: "Wards",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateTypes");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "IndustrialZones");

            migrationBuilder.DropTable(
                name: "IndustryCodes");

            migrationBuilder.DropTable(
                name: "StatusCodes");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
