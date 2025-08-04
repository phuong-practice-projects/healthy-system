namespace Healthy.Application.Common.Models;

/// <summary>
/// Response model for paginated meal list - optimized and standardized
/// </summary>
public class MealListResponse : BasePaginatedResponse<MealDto>
{
    /// <summary>
    /// List of meals for the current page (alias for Items property)
    /// </summary>
    public List<MealDto> Meals 
    { 
        get => Items; 
        set => Items = value; 
    }

    /// <summary>
    /// Create a meal list response with validated pagination info
    /// </summary>
    public static MealListResponse Create(List<MealDto> meals, int totalItems, int currentPage, int pageSize)
    {
        var response = new MealListResponse();
        response.SetPaginationData(meals, totalItems, currentPage, pageSize);
        return response;
    }
}
