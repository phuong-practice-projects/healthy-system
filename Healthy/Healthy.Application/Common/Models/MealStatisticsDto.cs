namespace Healthy.Application.Common.Models;

/// <summary>
/// Statistics for meal data analysis
/// </summary>
public class MealStatisticsDto
{
    /// <summary>
    /// Total number of meals for the day
    /// </summary>
    public int TotalMeals { get; set; }
    
    /// <summary>
    /// Breakdown by meal type
    /// </summary>
    public Dictionary<string, int> MealTypeCount { get; set; } = new();
    
    /// <summary>
    /// Completion percentage for daily meal goals (if goal system exists)
    /// </summary>
    public decimal CompletionPercentage { get; set; }
}
