namespace Healthy.Application.Common.Models;

/// <summary>
/// Achievement/Completion rate response for dashboard
/// </summary>
public class CompletionRateResponse
{
    /// <summary>
    /// Achievement rate percentage (0-100)
    /// </summary>
    public decimal Rate { get; set; }
    
    /// <summary>
    /// Descriptive message about the achievement
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Breakdown of completed vs total goals
    /// </summary>
    public GoalBreakdownDto Breakdown { get; set; } = new();
}
