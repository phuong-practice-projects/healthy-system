using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Meals.Queries.GetMeals;

/// <summary>
/// Query to get paginated list of meals for a user with optional filters
/// </summary>
public record GetMealsQuery : IRequest<MealListResponse>
{
    /// <summary>
    /// ID of the user whose meals to retrieve
    /// </summary>
    public Guid UserId { get; init; }
    
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
    /// Page number for pagination (starts from 1)
    /// </summary>
    public int Page { get; init; } = 1;
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; } = 10;
}
