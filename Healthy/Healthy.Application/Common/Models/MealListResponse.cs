namespace Healthy.Application.Common.Models;

/// <summary>
/// Response model for paginated meal list
/// </summary>
public class MealListResponse
{
    /// <summary>
    /// List of meals for the current page
    /// </summary>
    public List<MealDto> Meals { get; set; } = new List<MealDto>();
    
    /// <summary>
    /// Total number of meals matching the filter criteria
    /// </summary>
    public int TotalItems { get; set; }
    
    /// <summary>
    /// Total number of pages available
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
}
