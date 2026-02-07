# AXDD Report Service

Enterprise report submission and review system for industrial zone management.

## Overview

The Report Service allows enterprises to submit various types of reports (Labor, Environment, Production, Financial) and enables staff to review, approve, or reject them.

## Features

- **Report Submission**: Enterprises can submit reports with attachments
- **Report Templates**: Configurable templates for different report types
- **Review Workflow**: Staff can approve or reject reports with notes
- **Report Status Tracking**: Pending → Approved/Rejected workflow
- **Report Filtering**: Filter by type, status, enterprise, date range
- **Immutable Reports**: Once submitted, reports cannot be edited (only reviewed)

## Architecture

### Clean Architecture Layers

```
├── Domain/
│   ├── Entities/          # EnterpriseReport, ReportTemplate
│   ├── Enums/             # ReportType, ReportStatus, ReportPeriod
│   └── Repositories/      # IReportRepository, IReportTemplateRepository
├── Application/
│   ├── Services/          # ReportService, ReportTemplateService
│   ├── DTOs/              # Request/Response models
│   └── Validators/        # FluentValidation rules
├── Infrastructure/
│   ├── Data/              # DbContext, Configurations, Migrations
│   └── Repositories/      # Repository implementations, UnitOfWork
└── Controllers/           # API endpoints
```

## Entities

### EnterpriseReport

Core report entity with:
- Enterprise information (ID, Name, Code)
- Report details (Type, Period, Year, Month)
- Status tracking (Pending → Approved/Rejected)
- Review information (Reviewer, Notes, Rejection Reason)
- Audit fields (Created, Updated)

### ReportTemplate

Template definitions for report data structure:
- Report type mapping
- JSON schema for fields
- Version control
- Active/Inactive status

## Enums

### ReportType
- `Labor` - Employee, safety, working conditions
- `Environment` - Environmental impact, waste management
- `Production` - Capacity, output, inventory
- `Financial` - Revenue, expenses, investments
- `Other` - Miscellaneous reports

### ReportStatus
- `Pending` - Submitted, awaiting review
- `UnderReview` - Being reviewed by staff
- `Approved` - Approved by reviewer
- `Rejected` - Rejected with reason

### ReportPeriod
- `Monthly` - Month 1-12
- `Q1`, `Q2`, `Q3`, `Q4` - Quarterly reports
- `Annual` - Yearly reports

## API Endpoints

### Reports

```
POST   /api/v1/reports              # Submit new report
GET    /api/v1/reports              # Get filtered reports
GET    /api/v1/reports/{id}         # Get report by ID
PUT    /api/v1/reports/{id}/approve # Approve report
PUT    /api/v1/reports/{id}/reject  # Reject report
GET    /api/v1/reports/pending      # Get pending reports
GET    /api/v1/reports/my-reports   # Get current user's reports
```

### Report Templates

```
GET    /api/v1/report-templates            # Get active templates
GET    /api/v1/report-templates/{id}       # Get template by ID
GET    /api/v1/report-templates/by-type/{type}  # Get template by report type
POST   /api/v1/report-templates            # Create template (admin)
```

## Business Rules

1. **Uniqueness**: One report per enterprise + type + period + year (+ month for monthly)
2. **Immutability**: Reports cannot be edited after submission
3. **Status Workflow**: Pending → (UnderReview) → Approved/Rejected
4. **Rejection Reason**: Required when rejecting
5. **Reviewer Tracking**: System tracks who reviewed and when
6. **Monthly Reports**: Must specify month (1-12)
7. **Non-Monthly Reports**: Month must be null

## Configuration

### Database Connection

```json
{
  "ConnectionStrings": {
    "ReportDatabase": "Server=localhost;Database=AXDD_Report;..."
  }
}
```

### Dependencies

- .NET 9.0
- Entity Framework Core 9.0
- FluentValidation 11.3.0
- SQL Server

## Getting Started

### 1. Update Connection String

Edit `appsettings.json` with your SQL Server connection.

### 2. Apply Migrations

```bash
dotnet ef database update
```

### 3. Run the Service

```bash
dotnet run
```

Service runs on `http://localhost:5000` (or configured port).

### 4. Access Swagger UI

Navigate to `http://localhost:5000/swagger` for API documentation.

## Example Usage

### Submit a Report

```http
POST /api/v1/reports
Content-Type: application/json

{
  "enterpriseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "enterpriseName": "ABC Manufacturing",
  "enterpriseCode": "DN-BH1-001",
  "reportType": "Labor",
  "reportPeriod": "Q1",
  "year": 2024,
  "dataJson": "{\"totalEmployees\": 150, \"newHires\": 10}",
  "attachments": [
    "https://storage.example.com/reports/labor-q1-2024.pdf"
  ]
}
```

### Approve a Report

```http
PUT /api/v1/reports/{id}/approve
Content-Type: application/json

{
  "reviewerNotes": "All data verified. Approved."
}
```

### Reject a Report

```http
PUT /api/v1/reports/{id}/reject
Content-Type: application/json

{
  "rejectionReason": "Missing employee safety data",
  "reviewerNotes": "Please add section 3 and resubmit"
}
```

### Get Reports with Filters

```http
GET /api/v1/reports?status=Pending&reportType=Labor&year=2024&pageNumber=1&pageSize=20
```

## Validation Rules

### CreateReportRequest
- Enterprise ID required
- Report type and period required
- Year: 2001 to next year
- Month: 1-12 (required for monthly reports, null otherwise)
- Data JSON required

### RejectReportRequest
- Rejection reason required (max 1000 chars)
- Reviewer notes optional (max 2000 chars)

### CreateTemplateRequest
- Template name required, unique (max 200 chars)
- Report type required
- Fields JSON required
- Version > 0

## Health Checks

```http
GET /health
```

Returns `200 OK` if service and database are healthy.

## Logging

Logs are written to console and can be configured for file/cloud logging.

## Future Enhancements

- [ ] File upload service integration
- [ ] Email notifications for status changes
- [ ] Report analytics dashboard
- [ ] Export reports to PDF/Excel
- [ ] Bulk report operations
- [ ] Report history and versioning
- [ ] Advanced search and filtering
- [ ] Role-based access control integration

## License

Copyright © 2025 AXDD Project
