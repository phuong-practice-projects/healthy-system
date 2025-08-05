namespace Healthy.Application.Common.Models;

/// <summary>
/// Response model for meal history (used in dashboard)
/// </summary>
public class MealHistoryResponse
{
    /// <summary>
    /// List of meals for the specified time period
    /// </summary>
    public List<MealDto> Meals { get; set; } = new List<MealDto>();
    
    /// <summary>
    /// Statistics summary for the meal data
    /// </summary>
    public MealStatisticsDto Statistics { get; set; } = new();
}
