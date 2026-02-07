# Enterprise Service - Quick Start Guide

## 🚀 5-Minute Setup

### 1. Navigate to Project
```bash
cd src/Services/Enterprise/AXDD.Services.Enterprise.Api
```

### 2. Apply Database Migration
```bash
dotnet ef database update
```

### 3. Run the Service
```bash
dotnet run
```

### 4. Access Swagger
Open browser: `https://localhost:5001/swagger`

---

## 📝 Common Commands

### Database
```bash
# Create new migration
dotnet ef migrations add MigrationName --output-dir Infrastructure/Data/Migrations

# Apply migrations
dotnet ef database update

# Rollback to previous migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove

# View migration SQL
dotnet ef migrations script
```

### Build & Run
```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Watch mode (auto-restart on changes)
dotnet watch run
```

### Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## 🎯 Quick API Examples

### Create Enterprise (Copy & Paste to Swagger)
```json
{
  "code": "DN-BH1-001",
  "name": "Công ty TNHH Test",
  "taxCode": "0123456789",
  "industryCode": "C1011",
  "industryName": "Chế biến thực phẩm",
  "status": "Active",
  "address": "Lô A1, KCN Test",
  "district": "Biên Hòa",
  "province": "Đồng Nai",
  "phone": "0251-3-xxx-xxx",
  "email": "test@example.com",
  "registeredCapital": 50000000000
}
```

### Create Contact
```json
{
  "enterpriseId": "guid-from-previous-step",
  "fullName": "Nguyễn Văn A",
  "position": "Giám đốc",
  "phone": "0909-xxx-xxx",
  "email": "nvA@example.com",
  "isMain": true
}
```

### Create License
```json
{
  "enterpriseId": "guid-from-previous-step",
  "licenseType": "Environment",
  "licenseNumber": "1234/GP-STNMT",
  "issuedDate": "2023-01-15",
  "expiryDate": "2028-01-14",
  "issuingAuthority": "Sở Tài nguyên và Môi trường",
  "status": "Active"
}
```

### Change Status
```json
{
  "newStatus": "Suspended",
  "reason": "Environmental compliance review"
}
```

---

## 🔍 Testing Workflow

### 1. Create an Enterprise
```
POST /api/v1/enterprises
(Use JSON above)
```
**Copy the returned ID!**

### 2. Get the Enterprise
```
GET /api/v1/enterprises/{id}
```

### 3. List All Enterprises
```
GET /api/v1/enterprises?pageNumber=1&pageSize=20
```

### 4. Add a Contact
```
POST /api/v1/contactpersons
(Use enterpriseId from step 1)
```

### 5. Add a License
```
POST /api/v1/licenses
(Use enterpriseId from step 1)
```

### 6. View History
```
GET /api/v1/enterprises/{id}/history
```

### 7. Get Statistics
```
GET /api/v1/enterprises/statistics
```

---

## ⚡ Hot Tips

### Connection String
Default: `appsettings.json`
```json
"ConnectionStrings": {
  "EnterpriseDatabase": "Server=localhost;Database=AXDD_Enterprise;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

### Change Port
In `launchSettings.json` or:
```bash
dotnet run --urls "https://localhost:5002"
```

### Enable SQL Logging
In `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Test Tax Code Validation
✅ Valid: `"0123456789"` (10 digits)
✅ Valid: `"0123456789012"` (13 digits)
❌ Invalid: `"012345"` (too short)
❌ Invalid: `"ABC1234567"` (contains letters)

---

## 🐛 Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Migration Errors
```bash
# Check migration status
dotnet ef migrations list

# Drop database and recreate
dotnet ef database drop
dotnet ef database update
```

### Port Already in Use
```bash
# Change port in launchSettings.json or use:
dotnet run --urls "https://localhost:5002"
```

### SQL Server Connection Failed
1. Check SQL Server is running
2. Verify connection string in appsettings.json
3. Check Windows Authentication is enabled
4. Try: `Integrated Security=true;TrustServerCertificate=true`

---

## 📚 Key Files to Know

```
├── Program.cs                          # DI registration & app config
├── appsettings.json                    # Configuration
├── Controllers/
│   ├── EnterprisesController.cs       # Main enterprise API
│   ├── ContactPersonsController.cs    # Contact management
│   └── LicensesController.cs          # License management
├── Application/
│   ├── Services/                      # Business logic
│   ├── DTOs/                          # Request/response models
│   └── Validators/                    # Validation rules
├── Domain/
│   ├── Entities/                      # Database entities
│   ├── Enums/                         # Status, types
│   └── Repositories/                  # Repository interfaces
└── Infrastructure/
    ├── Data/
    │   ├── EnterpriseDbContext.cs     # EF Core context
    │   ├── Configurations/            # Entity configs
    │   └── Migrations/                # DB migrations
    └── Repositories/                  # Repository implementations
```

---

## 🎓 Learn More

- **Full Documentation**: See `README.md`
- **Completion Report**: See `ENTERPRISE_SERVICE_COMPLETION.md`
- **API Reference**: Run service and visit `/swagger`
- **Swagger JSON**: `/swagger/v1/swagger.json`

---

## ✅ Health Check

```bash
curl https://localhost:5001/health
```

Should return: `Healthy`

---

## 🔐 Default Credentials

This service doesn't implement authentication yet. 
The `userId` in operations defaults to `User.Identity?.Name ?? "system"`.

For testing, all operations will be recorded as performed by "system".

---

## 📞 Support

For issues or questions:
1. Check `README.md` for detailed documentation
2. Review `ENTERPRISE_SERVICE_COMPLETION.md` for implementation details
3. Check Swagger UI for API reference
4. Review code comments in source files

---

**Happy Coding! 🎉**
