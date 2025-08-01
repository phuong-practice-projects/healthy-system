using Microsoft.EntityFrameworkCore;
using Healthy.Domain.Entities;

namespace Healthy.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 