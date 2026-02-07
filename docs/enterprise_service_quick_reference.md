# Enterprise Service Quick Reference

## Files Created

```
Application/Services/
├── Interfaces/
│   ├── IEnterpriseService.cs           # Main enterprise operations (9 methods)
│   ├── IEnterpriseHistoryService.cs    # Audit/history logging (7 methods)
│   ├── IContactPersonService.cs        # Contact management (6 methods)
│   └── IEnterpriseLicenseService.cs    # License management (6 methods)
└── Implementations/
    ├── EnterpriseService.cs            # 24 KB - Full CRUD + validation
    ├── EnterpriseHistoryService.cs     # 7 KB - Change tracking
    ├── ContactPersonService.cs         # 10 KB - Contact CRUD
    └── EnterpriseLicenseService.cs     # 10 KB - License CRUD
```

## Quick Usage

### 1. Register Services
```csharp
// Program.cs
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IEnterpriseHistoryService, EnterpriseHistoryService>();
builder.Services.AddScoped<IContactPersonService, ContactPersonService>();
builder.Services.AddScoped<IEnterpriseLicenseService, EnterpriseLicenseService>();
```

### 2. Use in Controller
```csharp
public class EnterprisesController : ControllerBase
{
    private readonly IEnterpriseService _service;

    public EnterprisesController(IEnterpriseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(
            page, size, null, null, null, null, null, false, ct);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }
}
```

### 3. Result Pattern
```csharp
var result = await _service.CreateAsync(request, userId, ct);

if (result.IsSuccess)
{
    // Success path
    var enterprise = result.Value;
    return Ok(enterprise);
}
else
{
    // Failure path
    var error = result.Error;
    return BadRequest(error);
}
```

## Key Methods

### IEnterpriseService
- `GetAllAsync()` - Paginated list with filters/sorting
- `GetByIdAsync()` - Get single with contacts & licenses
- `CreateAsync()` - Create with validation
- `UpdateAsync()` - Update with change tracking
- `ChangeStatusAsync()` - Change status with validation

### IContactPersonService
- `GetByEnterpriseIdAsync()` - Get all contacts
- `CreateAsync()` - Add contact
- `SetAsMainContactAsync()` - Set main contact

### IEnterpriseLicenseService
- `GetByEnterpriseIdAsync()` - Get all licenses
- `CreateAsync()` - Add license
- `GetExpiringSoonAsync()` - Find expiring licenses

### IEnterpriseHistoryService
- `GetHistoryAsync()` - Get change log
- `LogCreationAsync()` - Log creation
- `LogUpdateAsync()` - Log updates with field tracking

## Business Rules

### Status Transitions
```
UnderConstruction → Active, Closed
Active → Suspended, Closed, Liquidated
Suspended → Active, Closed, Liquidated
Closed → Active, Liquidated
Liquidated → [terminal state]
```

### Validation
- Code must be unique
- Tax code must be unique
- Only one main contact per enterprise
- License numbers must be unique per enterprise

## Dependencies
- IUnitOfWork (for database operations)
- IEnterpriseRepository (for enterprise queries)
- IEnterpriseHistoryService (for audit logging)

## Features
✅ Pagination & filtering  
✅ Full-text search  
✅ Sorting (multiple fields)  
✅ Related entity loading  
✅ Change history tracking  
✅ Statistics/aggregations  
✅ Status validation  
✅ Duplicate detection  

## Compilation Status
✅ All services compile successfully  
⚠️ Project has pre-existing EnterpriseRepository error (not related to services)  

## Documentation
- `/docs/enterprise_service_implementation_summary.md` - Detailed overview
- `/docs/enterprise_service_registration.md` - DI registration guide
- `/docs/enterprise_service_completion_checklist.md` - Completion status
