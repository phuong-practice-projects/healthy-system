using Microsoft.EntityFrameworkCore;
using Healthy.Domain.Entities;

namespace Healthy.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Column> Columns { get; }
    DbSet<BodyRecord> BodyRecords { get; }
    DbSet<Exercise> Exercises { get; }
    DbSet<Diary> Diaries { get; }
    DbSet<Meal> Meals { get; }
    
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 