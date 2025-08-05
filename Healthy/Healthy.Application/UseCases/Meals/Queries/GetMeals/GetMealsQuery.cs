using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Meals.Queries.GetMeals;

/// <summary>
/// Optimized query to get paginated list of meals for a user with advanced filters
/// </summary>
public record GetMealsQuery : IRequest<MealListResponse>
{
    /// <summary>
    /// ID of the user whose meals to retrieve
    /// </summary>
    public required Guid UserId { get; init; }
    
    /// <summary>
    /// Optional start date filter (inclusive)
    /// </summary>
    public DateTime? StartDate { get; init; }
    
    /// <summary>
    /// Optional end date filter (inclusive)
    /// </summary>
    public DateTime? EndDate { get; init; }
    
    /// <summary>
    /// Optional meal type filter: "Morning", "Lunch", "Dinner", "Snack"
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Optional search term for meal name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Minimum calories filter
    /// </summary>
    public int? MinCalories { get; init; }

    /// <summary>
    /// Maximum calories filter
    /// </summary>
    public int? MaxCalories { get; init; }

    /// <summary>
    /// Sorting field: "Date", "Calories", "Name", "Type"
    /// </summary>
    public string SortBy { get; init; } = "Date";

    /// <summary>
    /// Sort direction: "Asc" or "Desc"
    /// </summary>
    public string SortDirection { get; init; } = "Desc";
    
    /// <summary>
    /// Page number for pagination (starts from 1)
    /// </summary>
    public int Page { get; init; } = 1;
    
    /// <summary>
    /// Number of items per page (max 100)
    /// </summary>
    public int PageSize { get; init; } = 10;

    // Computed properties for better performance and consistency
    
    /// <summary>
    /// Get validated page number
    /// </summary>
    public int ValidPage => Math.Max(1, Page);

    /// <summary>
    /// Get validated page size
    /// </summary>
    public int ValidPageSize => Math.Max(1, Math.Min(100, PageSize));

    /// <summary>
    /// Calculate skip count for database queries
    /// </summary>
    public int Skip => (ValidPage - 1) * ValidPageSize;

    /// <summary>
    /// Get take count for database queries
    /// </summary>
    public int Take => ValidPageSize;

    /// <summary>
    /// Check if sort direction is ascending
    /// </summary>
    public bool IsAscending => SortDirection.Equals("Asc", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Get normalized sort field
    /// </summary>
    public string NormalizedSortBy => SortBy switch
    {
        "Date" => "CreatedAt",
        "Calories" => "Calories", 
        "Name" => "Name",
        "Type" => "Type",
        _ => "CreatedAt"
    };

    /// <summary>
    /// Check if search term is provided and valid
    /// </summary>
    public bool HasSearchTerm => !string.IsNullOrWhiteSpace(SearchTerm);

    /// <summary>
    /// Check if date range is specified
    /// </summary>
    public bool HasDateRange => StartDate.HasValue || EndDate.HasValue;

    /// <summary>
    /// Check if calorie range is specified
    /// </summary>
    public bool HasCalorieRange => MinCalories.HasValue || MaxCalories.HasValue;
}
