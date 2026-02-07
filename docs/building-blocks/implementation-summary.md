# BuildingBlocks Enhancement Implementation

## Overview
This document describes the foundational patterns and base classes implemented in the AXDD BuildingBlocks projects for use across all microservices.

## Implementation Date
February 6, 2025

## Projects Enhanced

### 1. AXDD.BuildingBlocks.Domain
Core domain layer components that define business entities, repositories, and domain events.

#### Repositories (Domain/Repositories/)
- **IReadRepository<T>**: Read-only repository interface with query operations
  - GetByIdAsync with optional includes
  - GetAllAsync, FindAsync, FirstOrDefaultAsync
  - AnyAsync, CountAsync
  - AsQueryable for advanced queries
  
- **IRepository<T>**: Full repository interface extending IReadRepository
  - AddAsync, AddRangeAsync
  - Update, UpdateRange
  - Delete (soft delete), DeleteRange
  - HardDelete for permanent removal
  
- **IUnitOfWork**: Transaction management interface
  - Repository<T>() for accessing repositories
  - SaveChangesAsync
  - BeginTransactionAsync, CommitTransactionAsync, RollbackTransactionAsync

#### Domain Events (Domain/Events/)
- **IDomainEvent**: Base interface for all domain events
  - OccurredOn: DateTime
  - EventId: Guid
  
- **IDomainEventHandler<T>**: Handler interface for domain events
  - HandleAsync method
  
- **DomainEvent**: Abstract base record implementation

#### Value Objects (Domain/ValueObjects/)
- **PhoneNumber**: Vietnamese phone number validation
  - Supports formats: 0912345678 or +84912345678
  - Validation for Vietnamese mobile prefixes (03, 05, 07, 08, 09)
  - ToInternationalFormat() method
  
- **Email**: Email address validation
  - RFC-compliant email validation
  - Domain and LocalPart properties
  - Max length 254 characters
  
- **TaxCode**: Vietnamese Tax Identification Number
  - Format: 10 digits or 10 digits + 3-digit branch code
  - MainCode and BranchCode properties
  - IsBranchCode boolean

#### Entity Enhancements (Domain/Entities/)
- **BaseEntity**: Enhanced with domain events support
  - Domain events collection (DomainEvents)
  - AddDomainEvent, RemoveDomainEvent, ClearDomainEvents
  - Audit fields: CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
  - Soft delete: IsDeleted, DeletedAt, DeletedBy

### 2. AXDD.BuildingBlocks.Infrastructure
Infrastructure implementations for data access and persistence.

#### Repositories (Infrastructure/Repositories/)
- **Repository<T>**: Generic EF Core repository implementation
  - Implements IRepository<T>
  - Automatic soft delete filtering
  - Timestamp management for CreatedAt/UpdatedAt
  
- **UnitOfWork**: EF Core Unit of Work implementation
  - Repository caching using ConcurrentDictionary
  - Transaction management
  - Proper disposal pattern

#### Persistence (Infrastructure/Persistence/)
- **BaseDbContext**: Abstract base DbContext
  - Automatic audit field management
  - Domain events extraction and clearing
  - Global soft delete query filter
  - Converts hard deletes to soft deletes
  - Current user tracking for audit
  
- **IDbConnectionFactory / SqlConnectionFactory**: Connection factory pattern
  - Creates and opens SQL Server connections
  - Supports connection string injection

- **MigrationHelper**: Database migration utilities
  - MigrateDatabaseAsync<TContext>
  - EnsureDatabaseCreatedAsync<TContext>
  - Logging support

#### Extensions (Infrastructure/Extensions/)
- **ServiceCollectionExtensions**:
  - AddDatabaseInfrastructure<TContext>: Configures EF Core with retry logic
  - AddSqlConnectionFactory: Registers connection factory

#### Dependencies
- Microsoft.EntityFrameworkCore 9.0.0
- Microsoft.EntityFrameworkCore.Relational 9.0.0
- Microsoft.EntityFrameworkCore.SqlServer 9.0.0
- Microsoft.Extensions.Hosting.Abstractions 9.0.0

### 3. AXDD.BuildingBlocks.Common
Common utilities, DTOs, validation, and exception handling.

#### Results (Common/Results/)
- **Result<T>**: Operation result pattern
  - IsSuccess, IsFailure properties
  - Value for success, Error for failure
  - Multiple errors support
  - Match() for functional composition
  - Map<TNew>() for transformation
  
- **Result**: Non-generic result for void operations

#### DTOs (Common/DTOs/)
- **ApiResponse<T>**: Enhanced API response wrapper
  - IsSuccess, Message, Data, Errors
  - StatusCode, Timestamp, Metadata
  - Success(), Failure(), NotFound(), ValidationError() factory methods
  - Backward compatible with legacy SuccessResponse/ErrorResponse
  
- **ApiResponse**: Non-generic response variant

- **PaginatedList<T>**: Paginated collection support
  - Items, PageNumber, PageSize, TotalCount, TotalPages
  - HasPreviousPage, HasNextPage
  - FirstItemIndex, LastItemIndex
  - CreateAsync from IQueryable
  - Create from IReadOnlyList
  - Empty() factory method

#### Validation (Common/Validation/)
- **VietnamesePhoneNumberAttribute**: Phone validation attribute
  - DataAnnotations compatible
  - Vietnamese mobile format validation
  
- **TaxCodeAttribute**: Tax code validation attribute
  - 10-digit or 10+3 digit format validation

#### Exceptions (Common/Exceptions/)
- **ValidationException**: Validation error with field-level errors
  - Errors dictionary (field -> error messages)
  
- **BusinessRuleException**: Business rule violation
  - Optional RuleName property
  
- **ConflictException**: Resource conflict (e.g., duplicate)
  
- **ForbiddenException**: Access forbidden
  
- **NotFoundException**: Resource not found (existing)
  
- **BadRequestException**: Bad request (existing)

#### Dependencies
- Microsoft.AspNetCore.App (framework reference)
- Microsoft.EntityFrameworkCore 9.0.0

## Design Principles Applied

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Extensible via inheritance and interfaces
- **Liskov Substitution**: Proper inheritance hierarchies
- **Interface Segregation**: Separate IReadRepository from IRepository
- **Dependency Inversion**: Depend on abstractions (interfaces)

### Patterns Implemented
1. **Repository Pattern**: Abstraction over data access
2. **Unit of Work Pattern**: Transaction management
3. **Domain Events**: Loose coupling between domain logic
4. **Value Objects**: Type-safe domain primitives
5. **Result Pattern**: Explicit success/failure handling
6. **Soft Delete**: Non-destructive data removal
7. **Audit Trail**: Automatic change tracking
8. **Factory Pattern**: Connection and response creation

### .NET 9 Features Used
- File-scoped namespaces
- Nullable reference types enabled
- Records for immutable value objects
- Collection expressions: `[]`
- ArgumentNullException.ThrowIfNull
- ArgumentException.ThrowIfNullOrWhiteSpace

## Usage Examples

### Using Repository and UnitOfWork
```csharp
public class EnterpriseService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnterpriseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Enterprise>> CreateEnterpriseAsync(CreateEnterpriseDto dto)
    {
        var repository = _unitOfWork.Repository<Enterprise>();
        
        // Check for duplicates
        if (await repository.AnyAsync(e => e.TaxCode == dto.TaxCode))
        {
            return Result<Enterprise>.Failure("Enterprise with this tax code already exists");
        }

        var enterprise = new Enterprise
        {
            Name = dto.Name,
            TaxCode = TaxCode.Create(dto.TaxCode),
            PhoneNumber = PhoneNumber.Create(dto.PhoneNumber)
        };

        await repository.AddAsync(enterprise);
        await _unitOfWork.SaveChangesAsync();

        return Result<Enterprise>.Success(enterprise);
    }
}
```

### Using Value Objects
```csharp
public class Contact
{
    public PhoneNumber PhoneNumber { get; set; }
    public Email Email { get; set; }
    
    public static Contact Create(string phone, string email)
    {
        return new Contact
        {
            PhoneNumber = PhoneNumber.Create(phone),
            Email = Email.Create(email)
        };
    }
}
```

### Using Validation Attributes
```csharp
public class CreateEnterpriseDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [TaxCode]
    public string TaxCode { get; set; }
    
    [VietnamesePhoneNumber]
    public string? PhoneNumber { get; set; }
}
```

### Using Result Pattern
```csharp
public async Task<IActionResult> CreateEnterprise(CreateEnterpriseDto dto)
{
    var result = await _service.CreateEnterpriseAsync(dto);
    
    return result.IsSuccess
        ? Ok(ApiResponse<Enterprise>.Success(result.Value!))
        : BadRequest(ApiResponse<Enterprise>.Failure(result.Error!));
}
```

### Configuring Services
```csharp
// In Program.cs or Startup.cs
services.AddDatabaseInfrastructure<EnterpriseDbContext>(
    connectionString: configuration.GetConnectionString("EnterpriseDb"),
    configureOptions: options => 
    {
        options.EnableSensitiveDataLogging(isDevelopment);
        options.EnableDetailedErrors(isDevelopment);
    });
```

### Creating Custom DbContext
```csharp
public class EnterpriseDbContext : BaseDbContext
{
    public DbSet<Enterprise> Enterprises { get; set; }
    
    public EnterpriseDbContext(
        DbContextOptions<EnterpriseDbContext> options,
        IHttpContextAccessor httpContextAccessor) 
        : base(options, httpContextAccessor.HttpContext?.User?.Identity?.Name)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Enterprise>(entity =>
        {
            entity.OwnsOne(e => e.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value).HasColumnName("PhoneNumber");
            });
        });
    }
}
```

## Testing Recommendations

### Unit Tests
- Test value object validation logic
- Test repository filtering and queries
- Test Result pattern success/failure paths
- Test validation attributes

### Integration Tests
- Test DbContext with in-memory database
- Test repository operations with real EF Core
- Test transaction rollback scenarios
- Test soft delete filtering

## Security Considerations

1. **SQL Injection**: Protected by EF Core parameterization
2. **Soft Delete**: Query filters prevent accidental data exposure
3. **Audit Trail**: All changes tracked with user information
4. **Validation**: Input validation at multiple layers

## Performance Considerations

1. **Repository Caching**: UnitOfWork caches repository instances
2. **Query Filters**: Global soft delete filter applied at DbContext level
3. **Async/Await**: All I/O operations are async
4. **Connection Pooling**: SQL Server connection pooling enabled
5. **Retry Logic**: Automatic retry for transient failures

## Future Enhancements

1. **Domain Event Dispatcher**: MediatR or custom implementation
2. **Outbox Pattern**: For reliable event publishing
3. **Specification Pattern**: For complex queries
4. **CQRS Support**: Command and query separation
5. **Caching Layer**: Distributed caching support
6. **Health Checks**: Database connectivity checks

## Compilation Status
✅ All BuildingBlocks projects compile successfully
✅ Full solution builds without errors
✅ No warnings generated

## Files Created/Modified

### Domain Project (11 files)
- Repositories/IReadRepository.cs ✓
- Repositories/IRepository.cs ✓
- Repositories/IUnitOfWork.cs ✓
- Events/IDomainEvent.cs ✓
- Events/IDomainEventHandler.cs ✓
- Events/DomainEvent.cs ✓
- ValueObjects/PhoneNumber.cs ✓
- ValueObjects/Email.cs ✓
- ValueObjects/TaxCode.cs ✓
- Entities/BaseEntity.cs (modified) ✓
- Entities/AuditableEntity.cs (existing)

### Infrastructure Project (7 files)
- Repositories/Repository.cs ✓
- Repositories/UnitOfWork.cs ✓
- Persistence/BaseDbContext.cs ✓
- Persistence/DbConnectionFactory.cs ✓
- Persistence/MigrationHelper.cs ✓
- Extensions/ServiceCollectionExtensions.cs ✓
- AXDD.BuildingBlocks.Infrastructure.csproj (modified) ✓

### Common Project (11 files)
- Results/Result.cs ✓
- DTOs/ApiResponse.cs (modified) ✓
- DTOs/PaginatedList.cs ✓
- Validation/VietnamesePhoneNumberAttribute.cs ✓
- Validation/TaxCodeAttribute.cs ✓
- Exceptions/ValidationException.cs ✓
- Exceptions/BusinessRuleException.cs ✓
- Exceptions/ConflictException.cs ✓
- Exceptions/ForbiddenException.cs ✓
- Exceptions/NotFoundException.cs (existing)
- AXDD.BuildingBlocks.Common.csproj (modified) ✓

**Total: 29 files created/modified**

## Conclusion
The BuildingBlocks infrastructure has been successfully enhanced with production-ready patterns and base classes. All services can now leverage these components for consistent data access, validation, error handling, and business logic implementation.
