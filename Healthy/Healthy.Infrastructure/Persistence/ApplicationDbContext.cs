using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Interfaces;
using Healthy.Domain.Entities;
using Healthy.Domain.Common;
using Healthy.Infrastructure.Persistence.Configurations;

namespace Healthy.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, 
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // DbSet properties
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<BodyRecord> BodyRecords => Set<BodyRecord>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Diary> Diaries => Set<Diary>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Apply specific configurations explicitly for better control
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ColumnConfiguration());
        modelBuilder.ApplyConfiguration(new BodyRecordConfiguration());
        modelBuilder.ApplyConfiguration(new ExerciseConfiguration());
        modelBuilder.ApplyConfiguration(new DiaryConfiguration());
        modelBuilder.ApplyConfiguration(new MealConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
 
    // Remove OnConfiguring - let DependencyInjection handle this
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     // Configuration is handled in DependencyInjection.cs
    // }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessAuditableEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ProcessAuditableEntities();
        return base.SaveChanges();
    }

    private void ProcessAuditableEntities()
    {
        var currentUserId = _currentUserService?.UserId;
        var currentTime = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IEntityAuditableBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.CreatedBy = currentUserId;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = currentUserId;
                    // Prevent modification of CreatedAt and CreatedBy
                    entry.Property(nameof(IEntityAuditableBase.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IEntityAuditableBase.CreatedBy)).IsModified = false;
                    break;

            }
        }

        // Handle BaseEntity instances that don't implement IEntityAuditableBase (like Role)
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            // Skip if this entity already implements IEntityAuditableBase (handled above)
            if (entry.Entity is IEntityAuditableBase)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    // For simple entities, we don't track audit info
                    // They only inherit Id from EntityBase
                    break;
            }
        }
    }
    }
