namespace Healthy.Application.Common.Models;

/// <summary>
/// Goal category completion status
/// </summary>
public class GoalCategoryStatusDto
{
    /// <summary>
    /// Category name (e.g., "Meals", "Exercise", "Hydration")
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of completed items in this category
    /// </summary>
    public int Completed { get; set; }
    
    /// <summary>
    /// Total number of items in this category
    /// </summary>
    public int Total { get; set; }
    
    /// <summary>
    /// Completion percentage for this category
    /// </summary>
    public decimal Percentage { get; set; }
}
