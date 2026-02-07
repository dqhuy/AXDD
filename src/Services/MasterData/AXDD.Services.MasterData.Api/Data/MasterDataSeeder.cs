using AXDD.Services.MasterData.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.MasterData.Api.Data;

/// <summary>
/// Seeds initial master data
/// </summary>
public static class MasterDataSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        SeedProvinces(modelBuilder);
        SeedDistricts(modelBuilder);
        // SeedWards(modelBuilder); // Commented out for now - can be added later with proper district ID references
        SeedIndustrialZones(modelBuilder);
        SeedIndustryCodes(modelBuilder);
        SeedCertificateTypes(modelBuilder);
        SeedDocumentTypes(modelBuilder);
        SeedStatusCodes(modelBuilder);
        SeedConfigurations(modelBuilder);
    }

    private static void SeedProvinces(ModelBuilder modelBuilder)
    {
        var provinces = new List<Province>
        {
            // North Region
            new() { Id = Guid.Parse("01000000-0000-0000-0000-000000000001"), Code = "01", Name = "Hà Nội", Region = "North", DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("02000000-0000-0000-0000-000000000002"), Code = "02", Name = "Hà Giang", Region = "North", DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("04000000-0000-0000-0000-000000000004"), Code = "04", Name = "Cao Bằng", Region = "North", DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("06000000-0000-0000-0000-000000000006"), Code = "06", Name = "Bắc Kạn", Region = "North", DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("08000000-0000-0000-0000-000000000008"), Code = "08", Name = "Tuyên Quang", Region = "North", DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), Code = "10", Name = "Lào Cai", Region = "North", DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("11000000-0000-0000-0000-000000000011"), Code = "11", Name = "Điện Biên", Region = "North", DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("12000000-0000-0000-0000-000000000012"), Code = "12", Name = "Lai Châu", Region = "North", DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("14000000-0000-0000-0000-000000000014"), Code = "14", Name = "Sơn La", Region = "North", DisplayOrder = 9, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("15000000-0000-0000-0000-000000000015"), Code = "15", Name = "Yên Bái", Region = "North", DisplayOrder = 10, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("17000000-0000-0000-0000-000000000017"), Code = "17", Name = "Hòa Bình", Region = "North", DisplayOrder = 11, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("19000000-0000-0000-0000-000000000019"), Code = "19", Name = "Thái Nguyên", Region = "North", DisplayOrder = 12, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000020"), Code = "20", Name = "Lạng Sơn", Region = "North", DisplayOrder = 13, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("22000000-0000-0000-0000-000000000022"), Code = "22", Name = "Quảng Ninh", Region = "North", DisplayOrder = 14, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("24000000-0000-0000-0000-000000000024"), Code = "24", Name = "Bắc Giang", Region = "North", DisplayOrder = 15, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("25000000-0000-0000-0000-000000000025"), Code = "25", Name = "Phú Thọ", Region = "North", DisplayOrder = 16, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("26000000-0000-0000-0000-000000000026"), Code = "26", Name = "Vĩnh Phúc", Region = "North", DisplayOrder = 17, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("27000000-0000-0000-0000-000000000027"), Code = "27", Name = "Bắc Ninh", Region = "North", DisplayOrder = 18, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000030"), Code = "30", Name = "Hải Dương", Region = "North", DisplayOrder = 19, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("31000000-0000-0000-0000-000000000031"), Code = "31", Name = "Hải Phòng", Region = "North", DisplayOrder = 20, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("33000000-0000-0000-0000-000000000033"), Code = "33", Name = "Hưng Yên", Region = "North", DisplayOrder = 21, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("34000000-0000-0000-0000-000000000034"), Code = "34", Name = "Thái Bình", Region = "North", DisplayOrder = 22, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("35000000-0000-0000-0000-000000000035"), Code = "35", Name = "Hà Nam", Region = "North", DisplayOrder = 23, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("36000000-0000-0000-0000-000000000036"), Code = "36", Name = "Nam Định", Region = "North", DisplayOrder = 24, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("37000000-0000-0000-0000-000000000037"), Code = "37", Name = "Ninh Bình", Region = "North", DisplayOrder = 25, CreatedAt = DateTime.UtcNow },

            // Central Region
            new() { Id = Guid.Parse("38000000-0000-0000-0000-000000000038"), Code = "38", Name = "Thanh Hóa", Region = "Central", DisplayOrder = 26, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000040"), Code = "40", Name = "Nghệ An", Region = "Central", DisplayOrder = 27, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("42000000-0000-0000-0000-000000000042"), Code = "42", Name = "Hà Tĩnh", Region = "Central", DisplayOrder = 28, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("44000000-0000-0000-0000-000000000044"), Code = "44", Name = "Quảng Bình", Region = "Central", DisplayOrder = 29, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("45000000-0000-0000-0000-000000000045"), Code = "45", Name = "Quảng Trị", Region = "Central", DisplayOrder = 30, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("46000000-0000-0000-0000-000000000046"), Code = "46", Name = "Thừa Thiên Huế", Region = "Central", DisplayOrder = 31, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("48000000-0000-0000-0000-000000000048"), Code = "48", Name = "Đà Nẵng", Region = "Central", DisplayOrder = 32, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("49000000-0000-0000-0000-000000000049"), Code = "49", Name = "Quảng Nam", Region = "Central", DisplayOrder = 33, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("51000000-0000-0000-0000-000000000051"), Code = "51", Name = "Quảng Ngãi", Region = "Central", DisplayOrder = 34, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000052"), Code = "52", Name = "Bình Định", Region = "Central", DisplayOrder = 35, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("54000000-0000-0000-0000-000000000054"), Code = "54", Name = "Phú Yên", Region = "Central", DisplayOrder = 36, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000056"), Code = "56", Name = "Khánh Hòa", Region = "Central", DisplayOrder = 37, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000058"), Code = "58", Name = "Ninh Thuận", Region = "Central", DisplayOrder = 38, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000060"), Code = "60", Name = "Bình Thuận", Region = "Central", DisplayOrder = 39, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000062"), Code = "62", Name = "Kon Tum", Region = "Central", DisplayOrder = 40, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("64000000-0000-0000-0000-000000000064"), Code = "64", Name = "Gia Lai", Region = "Central", DisplayOrder = 41, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("66000000-0000-0000-0000-000000000066"), Code = "66", Name = "Đắk Lắk", Region = "Central", DisplayOrder = 42, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("67000000-0000-0000-0000-000000000067"), Code = "67", Name = "Đắk Nông", Region = "Central", DisplayOrder = 43, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("68000000-0000-0000-0000-000000000068"), Code = "68", Name = "Lâm Đồng", Region = "Central", DisplayOrder = 44, CreatedAt = DateTime.UtcNow },

            // South Region
            new() { Id = Guid.Parse("70000000-0000-0000-0000-000000000070"), Code = "70", Name = "Bình Phước", Region = "South", DisplayOrder = 45, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("72000000-0000-0000-0000-000000000072"), Code = "72", Name = "Tây Ninh", Region = "South", DisplayOrder = 46, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("74000000-0000-0000-0000-000000000074"), Code = "74", Name = "Bình Dương", Region = "South", DisplayOrder = 47, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("75000000-0000-0000-0000-000000000075"), Code = "75", Name = "Đồng Nai", Region = "South", DisplayOrder = 48, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("77000000-0000-0000-0000-000000000077"), Code = "77", Name = "Bà Rịa - Vũng Tàu", Region = "South", DisplayOrder = 49, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("79000000-0000-0000-0000-000000000079"), Code = "79", Name = "Hồ Chí Minh", Region = "South", DisplayOrder = 50, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("80000000-0000-0000-0000-000000000080"), Code = "80", Name = "Long An", Region = "South", DisplayOrder = 51, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("82000000-0000-0000-0000-000000000082"), Code = "82", Name = "Tiền Giang", Region = "South", DisplayOrder = 52, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("83000000-0000-0000-0000-000000000083"), Code = "83", Name = "Bến Tre", Region = "South", DisplayOrder = 53, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("84000000-0000-0000-0000-000000000084"), Code = "84", Name = "Trà Vinh", Region = "South", DisplayOrder = 54, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("86000000-0000-0000-0000-000000000086"), Code = "86", Name = "Vĩnh Long", Region = "South", DisplayOrder = 55, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("87000000-0000-0000-0000-000000000087"), Code = "87", Name = "Đồng Tháp", Region = "South", DisplayOrder = 56, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("89000000-0000-0000-0000-000000000089"), Code = "89", Name = "An Giang", Region = "South", DisplayOrder = 57, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("91000000-0000-0000-0000-000000000091"), Code = "91", Name = "Kiên Giang", Region = "South", DisplayOrder = 58, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("92000000-0000-0000-0000-000000000092"), Code = "92", Name = "Cần Thơ", Region = "South", DisplayOrder = 59, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("93000000-0000-0000-0000-000000000093"), Code = "93", Name = "Hậu Giang", Region = "South", DisplayOrder = 60, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("94000000-0000-0000-0000-000000000094"), Code = "94", Name = "Sóc Trăng", Region = "South", DisplayOrder = 61, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("95000000-0000-0000-0000-000000000095"), Code = "95", Name = "Bạc Liêu", Region = "South", DisplayOrder = 62, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.Parse("96000000-0000-0000-0000-000000000096"), Code = "96", Name = "Cà Mau", Region = "South", DisplayOrder = 63, CreatedAt = DateTime.UtcNow }
        };

        modelBuilder.Entity<Province>().HasData(provinces);
    }

    private static void SeedDistricts(ModelBuilder modelBuilder)
    {
        // Dong Nai Province districts
        var dongNaiId = Guid.Parse("75000000-0000-0000-0000-000000000075");
        
        var districts = new List<District>
        {
            new() { Id = Guid.NewGuid(), Code = "7501", Name = "Thành phố Biên Hòa", ProvinceId = dongNaiId, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7502", Name = "Thành phố Long Khánh", ProvinceId = dongNaiId, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7503", Name = "Huyện Tân Phú", ProvinceId = dongNaiId, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7504", Name = "Huyện Vĩnh Cửu", ProvinceId = dongNaiId, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7505", Name = "Huyện Định Quán", ProvinceId = dongNaiId, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7506", Name = "Huyện Trảng Bom", ProvinceId = dongNaiId, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7507", Name = "Huyện Thống Nhất", ProvinceId = dongNaiId, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7508", Name = "Huyện Cẩm Mỹ", ProvinceId = dongNaiId, DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7509", Name = "Huyện Long Thành", ProvinceId = dongNaiId, DisplayOrder = 9, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7510", Name = "Huyện Xuân Lộc", ProvinceId = dongNaiId, DisplayOrder = 10, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "7511", Name = "Huyện Nhơn Trạch", ProvinceId = dongNaiId, DisplayOrder = 11, CreatedAt = DateTime.UtcNow }
        };

        modelBuilder.Entity<District>().HasData(districts);
    }

    private static void SeedWards(ModelBuilder modelBuilder)
    {
        // We'll add a few wards for Bien Hoa city as examples
        // In a real implementation, you would add all wards
        var wards = new List<Ward>
        {
            new() { Id = Guid.NewGuid(), Code = "750101", Name = "Phường Tân Phong", DistrictId = Guid.Parse("7501"), DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "750102", Name = "Phường Tân Biên", DistrictId = Guid.Parse("7501"), DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "750103", Name = "Phường Hố Nai", DistrictId = Guid.Parse("7501"), DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "750104", Name = "Phường Tân Hòa", DistrictId = Guid.Parse("7501"), DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "750105", Name = "Phường Tân Hiệp", DistrictId = Guid.Parse("7501"), DisplayOrder = 5, CreatedAt = DateTime.UtcNow }
        };

        // Note: For production, you would need to resolve the actual district IDs from the seeded districts
        // This is simplified for demonstration
    }

    private static void SeedIndustrialZones(ModelBuilder modelBuilder)
    {
        var dongNaiId = Guid.Parse("75000000-0000-0000-0000-000000000075");
        
        var zones = new List<IndustrialZone>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "DN-AMATA", 
                Name = "Khu công nghiệp Amata", 
                ProvinceId = dongNaiId, 
                Area = 700,
                Status = "Active",
                EstablishedDate = new DateTime(1994, 1, 1),
                ManagementUnit = "Amata Corporation",
                Description = "Khu công nghiệp lớn, thu hút nhiều nhà đầu tư nước ngoài",
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "DN-BIENHOA1", 
                Name = "Khu công nghiệp Biên Hòa 1", 
                ProvinceId = dongNaiId, 
                Area = 426,
                Status = "Active",
                EstablishedDate = new DateTime(1996, 1, 1),
                ManagementUnit = "Ban quản lý các KCN Đồng Nai",
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "DN-LONGTHANH", 
                Name = "Khu công nghiệp Long Thành", 
                ProvinceId = dongNaiId, 
                Area = 500,
                Status = "Active",
                EstablishedDate = new DateTime(1997, 1, 1),
                ManagementUnit = "Ban quản lý các KCN Đồng Nai",
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "DN-LOTE-PHUHUU", 
                Name = "Khu công nghiệp Lộ Tẻ - Phú Hữu", 
                ProvinceId = dongNaiId, 
                Area = 315,
                Status = "Active",
                EstablishedDate = new DateTime(1999, 1, 1),
                ManagementUnit = "Ban quản lý các KCN Đồng Nai",
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "DN-NHONTRACH", 
                Name = "Khu công nghiệp Nhơn Trạch", 
                ProvinceId = dongNaiId, 
                Area = 1000,
                Status = "Active",
                EstablishedDate = new DateTime(2000, 1, 1),
                ManagementUnit = "Ban quản lý các KCN Đồng Nai",
                CreatedAt = DateTime.UtcNow 
            }
        };

        modelBuilder.Entity<IndustrialZone>().HasData(zones);
    }

    private static void SeedIndustryCodes(ModelBuilder modelBuilder)
    {
        // This will be continued in the next part...
        var codes = new List<IndustryCode>();
        
        // Level 1 - Sections
        codes.AddRange(new[]
        {
            new IndustryCode { Id = Guid.NewGuid(), Code = "A", Name = "Nông nghiệp, lâm nghiệp và thuỷ sản", Level = 1, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "B", Name = "Khai khoáng", Level = 1, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C", Name = "Công nghiệp chế biến, chế tạo", Level = 1, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "D", Name = "Sản xuất và phân phối điện, khí đốt, nước nóng, hơi nước và điều hòa không khí", Level = 1, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "E", Name = "Cung cấp nước; hoạt động quản lý và xử lý rác thải, nước thải", Level = 1, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "F", Name = "Xây dựng", Level = 1, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "G", Name = "Bán buôn và bán lẻ; sửa chữa ô tô, mô tô, xe máy và xe có động cơ khác", Level = 1, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "H", Name = "Vận tải, kho bãi", Level = 1, DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "I", Name = "Dịch vụ lưu trú và ăn uống", Level = 1, DisplayOrder = 9, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "J", Name = "Thông tin và truyền thông", Level = 1, DisplayOrder = 10, CreatedAt = DateTime.UtcNow }
        });

        // Level 2 - Divisions (examples under C - Manufacturing)
        codes.AddRange(new[]
        {
            new IndustryCode { Id = Guid.NewGuid(), Code = "C10", Name = "Sản xuất thực phẩm", ParentCode = "C", Level = 2, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C11", Name = "Sản xuất đồ uống", ParentCode = "C", Level = 2, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C13", Name = "Dệt", ParentCode = "C", Level = 2, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C14", Name = "Sản xuất trang phục", ParentCode = "C", Level = 2, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C15", Name = "Sản xuất da và các sản phẩm có liên quan", ParentCode = "C", Level = 2, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C16", Name = "Chế biến gỗ và sản xuất sản phẩm từ gỗ, tre, nứa", ParentCode = "C", Level = 2, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C17", Name = "Sản xuất giấy và sản phẩm từ giấy", ParentCode = "C", Level = 2, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C18", Name = "In, sao chép bản ghi các loại", ParentCode = "C", Level = 2, DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C20", Name = "Sản xuất hoá chất và sản phẩm hoá chất", ParentCode = "C", Level = 2, DisplayOrder = 9, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C21", Name = "Sản xuất thuốc, hoá dược và dược liệu", ParentCode = "C", Level = 2, DisplayOrder = 10, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C22", Name = "Sản xuất sản phẩm từ cao su và plastic", ParentCode = "C", Level = 2, DisplayOrder = 11, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C23", Name = "Sản xuất sản phẩm từ khoáng phi kim loại khác", ParentCode = "C", Level = 2, DisplayOrder = 12, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C24", Name = "Sản xuất kim loại", ParentCode = "C", Level = 2, DisplayOrder = 13, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C25", Name = "Sản xuất sản phẩm từ kim loại đúc sẵn (trừ máy móc, thiết bị)", ParentCode = "C", Level = 2, DisplayOrder = 14, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C26", Name = "Sản xuất sản phẩm điện tử, máy vi tính và sản phẩm quang học", ParentCode = "C", Level = 2, DisplayOrder = 15, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C27", Name = "Sản xuất thiết bị điện", ParentCode = "C", Level = 2, DisplayOrder = 16, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C28", Name = "Sản xuất máy móc, thiết bị chưa được phân vào đâu", ParentCode = "C", Level = 2, DisplayOrder = 17, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C29", Name = "Sản xuất xe có động cơ", ParentCode = "C", Level = 2, DisplayOrder = 18, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C30", Name = "Sản xuất phương tiện vận tải khác", ParentCode = "C", Level = 2, DisplayOrder = 19, CreatedAt = DateTime.UtcNow }
        });

        // Level 3 - Groups (examples under C10 - Food production)
        codes.AddRange(new[]
        {
            new IndustryCode { Id = Guid.NewGuid(), Code = "C101", Name = "Chế biến và bảo quản thịt và các sản phẩm từ thịt", ParentCode = "C10", Level = 3, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C102", Name = "Chế biến và bảo quản thuỷ sản và các sản phẩm từ thuỷ sản", ParentCode = "C10", Level = 3, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C103", Name = "Chế biến và bảo quản rau quả", ParentCode = "C10", Level = 3, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C104", Name = "Sản xuất dầu và mỡ động, thực vật", ParentCode = "C10", Level = 3, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C105", Name = "Sản xuất sản phẩm từ sữa", ParentCode = "C10", Level = 3, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C106", Name = "Sản xuất sản phẩm từ xay xát, bột và tinh bột", ParentCode = "C10", Level = 3, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C107", Name = "Sản xuất các sản phẩm khác từ thực phẩm", ParentCode = "C10", Level = 3, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C108", Name = "Sản xuất thức ăn gia súc, gia cầm và thuỷ sản", ParentCode = "C10", Level = 3, DisplayOrder = 8, CreatedAt = DateTime.UtcNow }
        });

        // Level 4 - Classes (examples under C101)
        codes.AddRange(new[]
        {
            new IndustryCode { Id = Guid.NewGuid(), Code = "C1011", Name = "Chế biến và bảo quản thịt", ParentCode = "C101", Level = 4, DisplayOrder = 1, Description = "Chế biến, bảo quản thịt gia súc, gia cầm và các loại thịt khác", CreatedAt = DateTime.UtcNow },
            new IndustryCode { Id = Guid.NewGuid(), Code = "C1012", Name = "Sản xuất sản phẩm từ thịt", ParentCode = "C101", Level = 4, DisplayOrder = 2, Description = "Sản xuất các sản phẩm chế biến từ thịt như xúc xích, giăm bông, thịt xông khói", CreatedAt = DateTime.UtcNow }
        });

        modelBuilder.Entity<IndustryCode>().HasData(codes);
    }

    private static void SeedCertificateTypes(ModelBuilder modelBuilder)
    {
        var types = new List<CertificateType>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "INVEST", 
                Name = "Giấy chứng nhận đầu tư", 
                Description = "Giấy chứng nhận đăng ký đầu tư theo quy định của Luật Đầu tư",
                ValidityPeriod = 60, 
                RequiringAuthority = "Sở Kế hoạch và Đầu tư",
                IsRequired = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "BUSINESS", 
                Name = "Giấy chứng nhận đăng ký doanh nghiệp", 
                Description = "Giấy chứng nhận đăng ký doanh nghiệp theo Luật Doanh nghiệp",
                ValidityPeriod = null,
                RequiringAuthority = "Sở Kế hoạch và Đầu tư",
                IsRequired = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "ENV", 
                Name = "Giấy phép môi trường", 
                Description = "Giấy phép môi trường hoặc văn bản xác nhận đăng ký kế hoạch bảo vệ môi trường",
                ValidityPeriod = 60,
                RequiringAuthority = "Sở Tài nguyên và Môi trường",
                IsRequired = true,
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "CONSTRUCT", 
                Name = "Giấy phép xây dựng", 
                Description = "Giấy phép xây dựng công trình",
                ValidityPeriod = 24,
                RequiringAuthority = "Sở Xây dựng",
                IsRequired = false,
                DisplayOrder = 4,
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "FIRE", 
                Name = "Giấy chứng nhận đủ điều kiện về PCCC", 
                Description = "Giấy chứng nhận đủ điều kiện về phòng cháy chữa cháy",
                ValidityPeriod = null,
                RequiringAuthority = "Công an tỉnh/thành phố",
                IsRequired = true,
                DisplayOrder = 5,
                CreatedAt = DateTime.UtcNow 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Code = "IMPORT", 
                Name = "Giấy phép nhập khẩu", 
                Description = "Giấy phép nhập khẩu hàng hoá, thiết bị",
                ValidityPeriod = 12,
                RequiringAuthority = "Bộ Công Thương",
                IsRequired = false,
                DisplayOrder = 6,
                CreatedAt = DateTime.UtcNow 
            }
        };

        modelBuilder.Entity<CertificateType>().HasData(types);
    }

    private static void SeedDocumentTypes(ModelBuilder modelBuilder)
    {
        var types = new List<DocumentType>
        {
            new() { Id = Guid.NewGuid(), Code = "DOC_INVEST_PROPOSAL", Name = "Đề xuất dự án đầu tư", Category = "Investment", IsRequired = true, AllowedExtensions = ".pdf,.doc,.docx", MaxFileSizeMB = 10, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_FEASIBILITY", Name = "Báo cáo khả thi", Category = "Investment", IsRequired = true, AllowedExtensions = ".pdf,.doc,.docx", MaxFileSizeMB = 20, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_ENV_IMPACT", Name = "Báo cáo đánh giá tác động môi trường", Category = "Environment", IsRequired = true, AllowedExtensions = ".pdf,.doc,.docx", MaxFileSizeMB = 20, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_LAND_LEASE", Name = "Hợp đồng thuê đất", Category = "Legal", IsRequired = true, AllowedExtensions = ".pdf", MaxFileSizeMB = 5, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_BUSINESS_REG", Name = "Giấy chứng nhận đăng ký kinh doanh", Category = "Legal", IsRequired = true, AllowedExtensions = ".pdf", MaxFileSizeMB = 5, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_TAX_CODE", Name = "Giấy chứng nhận mã số thuế", Category = "Financial", IsRequired = true, AllowedExtensions = ".pdf", MaxFileSizeMB = 5, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_FINANCIAL_STMT", Name = "Báo cáo tài chính", Category = "Financial", IsRequired = false, AllowedExtensions = ".pdf,.xls,.xlsx", MaxFileSizeMB = 10, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "DOC_PASSPORT", Name = "Hộ chiếu (đối với nhà đầu tư nước ngoài)", Category = "Legal", IsRequired = false, AllowedExtensions = ".pdf,.jpg,.png", MaxFileSizeMB = 5, DisplayOrder = 8, CreatedAt = DateTime.UtcNow }
        };

        modelBuilder.Entity<DocumentType>().HasData(types);
    }

    private static void SeedStatusCodes(ModelBuilder modelBuilder)
    {
        var codes = new List<StatusCode>();

        // Enterprise statuses
        codes.AddRange(new[]
        {
            new StatusCode { Id = Guid.NewGuid(), Code = "ACTIVE", Name = "Hoạt động", EntityType = "Enterprise", Color = "#28a745", IsFinal = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "SUSPENDED", Name = "Tạm ngừng", EntityType = "Enterprise", Color = "#ffc107", IsFinal = false, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "CLOSED", Name = "Đã đóng", EntityType = "Enterprise", Color = "#dc3545", IsFinal = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow }
        });

        // Project statuses
        codes.AddRange(new[]
        {
            new StatusCode { Id = Guid.NewGuid(), Code = "DRAFT", Name = "Nháp", EntityType = "Project", Color = "#6c757d", IsFinal = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "SUBMITTED", Name = "Đã nộp", EntityType = "Project", Color = "#17a2b8", IsFinal = false, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "UNDER_REVIEW", Name = "Đang thẩm định", EntityType = "Project", Color = "#ffc107", IsFinal = false, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "APPROVED", Name = "Đã phê duyệt", EntityType = "Project", Color = "#28a745", IsFinal = false, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "REJECTED", Name = "Từ chối", EntityType = "Project", Color = "#dc3545", IsFinal = true, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "IN_PROGRESS", Name = "Đang thực hiện", EntityType = "Project", Color = "#007bff", IsFinal = false, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "COMPLETED", Name = "Đã hoàn thành", EntityType = "Project", Color = "#28a745", IsFinal = true, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "CANCELLED", Name = "Đã hủy", EntityType = "Project", Color = "#dc3545", IsFinal = true, DisplayOrder = 8, CreatedAt = DateTime.UtcNow }
        });

        // Report statuses
        codes.AddRange(new[]
        {
            new StatusCode { Id = Guid.NewGuid(), Code = "PENDING", Name = "Chờ nộp", EntityType = "Report", Color = "#6c757d", IsFinal = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "SUBMITTED", Name = "Đã nộp", EntityType = "Report", Color = "#28a745", IsFinal = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new StatusCode { Id = Guid.NewGuid(), Code = "OVERDUE", Name = "Quá hạn", EntityType = "Report", Color = "#dc3545", IsFinal = false, DisplayOrder = 3, CreatedAt = DateTime.UtcNow }
        });

        modelBuilder.Entity<StatusCode>().HasData(codes);
    }

    private static void SeedConfigurations(ModelBuilder modelBuilder)
    {
        var configs = new List<Configuration>
        {
            new() { Id = Guid.NewGuid(), Key = "System.MaintenanceMode", Value = "false", Category = "System", DataType = "Boolean", Description = "Bật/tắt chế độ bảo trì hệ thống", IsSystem = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = "System.DefaultLanguage", Value = "vi-VN", Category = "System", DataType = "String", Description = "Ngôn ngữ mặc định của hệ thống", IsSystem = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = "Report.QuarterlyDeadlineDays", Value = "15", Category = "Report", DataType = "Int", Description = "Số ngày hạn nộp báo cáo định kỳ (sau khi kết thúc kỳ báo cáo)", IsSystem = false, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = "Report.AnnualDeadlineDays", Value = "30", Category = "Report", DataType = "Int", Description = "Số ngày hạn nộp báo cáo thường niên (sau khi kết thúc năm tài chính)", IsSystem = false, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = "Email.NotificationEnabled", Value = "true", Category = "Email", DataType = "Boolean", Description = "Bật/tắt gửi email thông báo", IsSystem = false, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Key = "Email.SenderAddress", Value = "noreply@axdd.gov.vn", Category = "Email", DataType = "String", Description = "Địa chỉ email người gửi", IsSystem = false, CreatedAt = DateTime.UtcNow }
        };

        modelBuilder.Entity<Configuration>().HasData(configs);
    }
}
