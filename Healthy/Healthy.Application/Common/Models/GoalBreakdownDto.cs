namespace Healthy.Application.Common.Models;

/// <summary>
/// Goal completion breakdown
/// </summary>
public class GoalBreakdownDto
{
    /// <summary>
    /// Number of completed goals today
    /// </summary>
    public int CompletedGoals { get; set; }
    
    /// <summary>
    /// Total number of goals for today
    /// </summary>
    public int TotalGoals { get; set; }
    
    /// <summary>
    /// List of goal categories and their completion status
    /// </summary>
    public List<GoalCategoryStatusDto> Categories { get; set; } = new();
}
