namespace Healthy.Application.Common.Models;

/// <summary>
/// Base request for pagination parameters with validation
/// </summary>
public abstract class PaginatedRequest
{
    private int _page = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (minimum: 1, default: 1)
    /// </summary>
    public int Page 
    { 
        get => _page; 
        set => _page = value < 1 ? 1 : value; 
    }

    /// <summary>
    /// Page size (minimum: 1, maximum: 100, default: 10)
    /// </summary>
    public int PageSize 
    { 
        get => _pageSize; 
        set => _pageSize = value switch
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
/// Base request with user scoping and pagination
/// </summary>
public abstract class UserScopedPaginatedRequest : PaginatedRequest
{
    /// <summary>
    /// User ID to scope the query to
    /// </summary>
    public required Guid UserId { get; init; }
}

/// <summary>
/// Base request with date range filtering
/// </summary>
public abstract class DateRangeRequest : UserScopedPaginatedRequest
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

/// <summary>
/// Base request with search and sorting capabilities
/// </summary>
public abstract class SearchableSortableRequest : DateRangeRequest
{
    /// <summary>
    /// Optional search term
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Sort field (override in derived classes for specific fields)
    /// </summary>
    public virtual string SortBy { get; init; } = "Date";

    /// <summary>
    /// Sort direction: "Asc" or "Desc"
    /// </summary>
    public string SortDirection { get; init; } = "Desc";

    /// <summary>
    /// Validate sort direction
    /// </summary>
    public bool IsAscending => SortDirection.Equals("Asc", StringComparison.OrdinalIgnoreCase);
}
