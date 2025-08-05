namespace Healthy.Application.Common.Models;

/// <summary>
/// Comprehensive dashboard response with all top page data
/// </summary>
public class DashboardOverviewResponse
{
    /// <summary>
    /// 12-month achievement chart data (weight/body fat trends)
    /// </summary>
    public BodyRecordGraphResponse AchievementChart { get; set; } = new();
    
    /// <summary>
    /// Today's meal history with statistics
    /// </summary>
    public MealHistoryResponse TodayMeals { get; set; } = new();
    
    /// <summary>
    /// Daily completion rate and goal achievement
    /// </summary>
    public CompletionRateResponse CompletionRate { get; set; } = new();
    
    /// <summary>
    /// Quick summary statistics for the dashboard
    /// </summary>
    public DashboardSummaryDto Summary { get; set; } = new();
}
