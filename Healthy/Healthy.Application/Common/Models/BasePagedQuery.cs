namespace Healthy.Application.Common.Models;

/// <summary>
/// Base query model with common pagination, filtering, and sorting properties
/// </summary>
public abstract record BasePagedQuery
{
    /// <summary>
    /// ID of the user for the query
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Page number for pagination (starts from 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page (max 100)
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Optional search term for general text search
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Sorting field name
    /// </summary>
    public string SortBy { get; init; } = "Id";

    /// <summary>
    /// Sort direction: "Asc" or "Desc"
    /// </summary>
    public string SortDirection { get; init; } = "Desc";

    /// <summary>
    /// Optional start date filter (inclusive)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Optional end date filter (inclusive)
    /// </summary>
    public DateTime? EndDate { get; init; }

    // Computed properties for performance optimization
    /// <summary>
    /// Gets validated page number (minimum 1)
    /// </summary>
    public int ValidPage => Math.Max(1, Page);

    /// <summary>
    /// Gets validated page size (between 1 and 100)
    /// </summary>
    public int ValidPageSize => Math.Max(1, Math.Min(100, PageSize));

    /// <summary>
    /// Gets the number of items to skip for pagination
    /// </summary>
    public int Skip => (ValidPage - 1) * ValidPageSize;

    /// <summary>
    /// Gets the number of items to take
    /// </summary>
    public int Take => ValidPageSize;

    /// <summary>
    /// Gets whether sorting is ascending
    /// </summary>
    public bool IsAscending => SortDirection.Equals("Asc", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether sorting is descending
    /// </summary>
    public bool IsDescending => !IsAscending;

    /// <summary>
    /// Gets whether there is a search term
    /// </summary>
    public bool HasSearchTerm => !string.IsNullOrWhiteSpace(SearchTerm);

    /// <summary>
    /// Gets whether there is a date range filter
    /// </summary>
    public bool HasDateFilter => StartDate.HasValue || EndDate.HasValue;

    /// <summary>
    /// Gets normalized sort field name
    /// </summary>
    public string NormalizedSortBy => string.IsNullOrWhiteSpace(SortBy) ? "Id" : SortBy;
}
