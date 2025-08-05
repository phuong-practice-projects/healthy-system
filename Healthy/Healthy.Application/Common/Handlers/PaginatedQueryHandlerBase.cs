using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Common.Handlers;

/// <summary>
/// Base class for query handlers that need pagination support
/// </summary>
public abstract class PaginatedQueryHandlerBase
{
    /// <summary>
    /// Create a paginated response from a query
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TDto">DTO type</typeparam>
    /// <param name="query">Base queryable</param>
    /// <param name="selector">Projection selector</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated response</returns>
    protected static async Task<PaginatedResponse<TDto>> CreatePaginatedResponseAsync<TEntity, TDto>(
        IQueryable<TEntity> query,
        System.Linq.Expressions.Expression<Func<TEntity, TDto>> selector,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Validate parameters
        page = Math.Max(1, page);
        pageSize = Math.Max(1, Math.Min(100, pageSize));

        // Get total count
        var totalItems = await query.CountAsync(cancellationToken);

        // Get paginated data
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return PaginatedResponse<TDto>.Create(items, totalItems, page, pageSize);
    }

    /// <summary>
    /// Create pagination info only (without data)
    /// </summary>
    /// <param name="totalItems">Total number of items</param>
    /// <param name="page">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Pagination info</returns>
    protected static (int TotalPages, bool HasPrevious, bool HasNext) CalculatePaginationInfo(int totalItems, int page, int pageSize)
    {
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);
        var hasPrevious = page > 1;
        var hasNext = page < totalPages;

        return (totalPages, hasPrevious, hasNext);
    }
}
