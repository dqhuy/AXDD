using System.Linq.Expressions;
using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace AXDD.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base DbContext with audit fields support and domain events handling
/// </summary>
public abstract class BaseDbContext : DbContext
{
    private readonly string? _currentUser;

    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    protected BaseDbContext(DbContextOptions options, string? currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    /// <summary>
    /// Saves all changes made in this context to the database with audit support
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        
        var domainEvents = GetDomainEvents();
        
        var result = await base.SaveChangesAsync(cancellationToken);
        
        // Clear domain events after saving
        ClearDomainEvents();
        
        // Domain events can be dispatched here if using a domain event dispatcher
        // await DispatchDomainEventsAsync(domainEvents, cancellationToken);
        
        return result;
    }

    /// <summary>
    /// Saves all changes made in this context to the database with audit support
    /// </summary>
    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    /// <summary>
    /// Applies audit information to entities before saving
    /// </summary>
    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = _currentUser;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = _currentUser;
                    
                    // If entity is being soft deleted
                    if (entry.Entity.IsDeleted && entry.Entity.DeletedAt == null)
                    {
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = _currentUser;
                    }
                    break;

                case EntityState.Deleted:
                    // Convert hard deletes to soft deletes
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = _currentUser;
                    break;
            }
        }
    }

    /// <summary>
    /// Gets all domain events from tracked entities
    /// </summary>
    private List<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .SelectMany(e => e.DomainEvents)
            .ToList();
    }

    /// <summary>
    /// Clears domain events from all tracked entities
    /// </summary>
    private void ClearDomainEvents()
    {
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(property), parameter);
                
                entityType.SetQueryFilter(filter);
            }
        }
    }
}
