using Healthy.Application.Common.Models;

namespace Healthy.Application.Common.Queries;

/// <summary>
/// Base class for paginated queries with common filtering and pagination parameters
/// </summary>
public abstract record PaginatedQueryBase
{
    private int _page = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (minimum: 1, default: 1)
    /// </summary>
    public int Page 
    { 
        get => _page; 
        init => _page = value < 1 ? 1 : value; 
    }

    /// <summary>
    /// Page size (minimum: 1, maximum: 100, default: 10)
    /// </summary>
    public int PageSize 
    { 
        get => _pageSize; 
        init => _pageSize = value switch
        {
            < 1 => 1,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Calculate skip count for database queries
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Take count for database queries
    /// </summary>
    public int Take => PageSize;
}

/// <summary>
/// Base class for user-scoped paginated queries
/// </summary>
public abstract record UserScopedPaginatedQueryBase : PaginatedQueryBase
{
    /// <summary>
    /// User ID to scope the query to
    /// </summary>
    public required Guid UserId { get; init; }
}

/// <summary>
/// Base class for queries with date range filtering
/// </summary>
public abstract record DateRangeQueryBase : UserScopedPaginatedQueryBase
{
    /// <summary>
    /// Optional start date filter (inclusive)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Optional end date filter (inclusive)
    /// </summary>
    public DateTime? EndDate { get; init; }
}
