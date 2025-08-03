namespace Healthy.Application.Common.Models;

/// <summary>
/// Dashboard summary statistics
/// </summary>
public class DashboardSummaryDto
{
    /// <summary>
    /// Current streak (consecutive days of goal achievement)
    /// </summary>
    public int CurrentStreak { get; set; }
    
    /// <summary>
    /// Best streak achieved
    /// </summary>
    public int BestStreak { get; set; }
    
    /// <summary>
    /// Total days active in the system
    /// </summary>
    public int TotalActiveDays { get; set; }
    
    /// <summary>
    /// Latest body weight record
    /// </summary>
    public decimal? CurrentWeight { get; set; }
    
    /// <summary>
    /// Weight change from last month (positive = gained, negative = lost)
    /// </summary>
    public decimal? WeightChange { get; set; }
}
