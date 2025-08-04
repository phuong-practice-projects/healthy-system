using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Common.Extensions;

/// <summary>
/// Extension methods for IQueryable to support pagination
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Apply pagination to IQueryable and return paginated result
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TResult">Result DTO type</typeparam>
    /// <param name="query">Base query</param>
    /// <param name="selector">Projection selector</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated response</returns>
    public static async Task<PaginatedResponse<TResult>> ToPaginatedResponseAsync<T, TResult>(
        this IQueryable<T> query,
        System.Linq.Expressions.Expression<Func<T, TResult>> selector,
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

        return PaginatedResponse<TResult>.Create(items, totalItems, page, pageSize);
    }

    /// <summary>
    /// Apply pagination to IQueryable without projection
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="query">Base query</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated response</returns>
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
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
            .ToListAsync(cancellationToken);

        return PaginatedResponse<T>.Create(items, totalItems, page, pageSize);
    }

    /// <summary>
    /// Apply pagination parameters to query (skip and take)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="query">Base query</param>
    /// <param name="paginationRequest">Pagination parameters</param>
    /// <returns>Query with pagination applied</returns>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationRequest paginationRequest)
    {
        return query
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take);
    }
}
