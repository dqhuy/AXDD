using AXDD.Services.Auth.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Auth.Api.Data;

/// <summary>
/// Database context for authentication service
/// </summary>
public class AuthDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    private readonly string? _currentUser;

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public AuthDbContext(DbContextOptions<AuthDbContext> options, IHttpContextAccessor? httpContextAccessor) : base(options)
    {
        _currentUser = httpContextAccessor?.HttpContext?.User?.Identity?.Name;
    }

    /// <summary>
    /// Gets or sets the RefreshTokens DbSet
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Gets or sets the UserSessions DbSet
    /// </summary>
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    /// <summary>
    /// Saves all changes made in this context to the database with audit support
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
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
        var entries = ChangeTracker.Entries();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Entity is ApplicationUser user)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        user.CreatedAt = now;
                        user.CreatedBy = _currentUser;
                        break;
                    case EntityState.Modified:
                        user.UpdatedAt = now;
                        user.UpdatedBy = _currentUser;
                        break;
                }
            }
            else if (entry.Entity is ApplicationRole role)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        role.CreatedAt = now;
                        role.CreatedBy = _currentUser;
                        break;
                    case EntityState.Modified:
                        role.UpdatedAt = now;
                        role.UpdatedBy = _currentUser;
                        break;
                }
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize table names
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // Configure RefreshToken
        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.CreatedByIp)
                .HasMaxLength(50);

            entity.Property(e => e.RevokedByIp)
                .HasMaxLength(50);

            entity.Property(e => e.ReplacedByToken)
                .HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Token);
            entity.HasIndex(e => e.UserId);
        });

        // Configure UserSession
        builder.Entity<UserSession>(entity =>
        {
            entity.ToTable("UserSessions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SessionToken)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.IpAddress)
                .HasMaxLength(50);

            entity.Property(e => e.UserAgent)
                .HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserSessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.SessionToken);
            entity.HasIndex(e => e.UserId);
        });

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FullName)
                .HasMaxLength(100);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256);

            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.UserName);
        });

        // Configure ApplicationRole
        builder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256);
        });
    }
}
