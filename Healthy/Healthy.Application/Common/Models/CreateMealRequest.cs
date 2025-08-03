using System.ComponentModel.DataAnnotations;

namespace Healthy.Application.Common.Models;

/// <summary>
/// Request model for creating a new meal
/// </summary>
public class CreateMealRequest
{
    /// <summary>
    /// URL or path to the meal image (required)
    /// </summary>
    [Required]
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of meal: "Morning", "Lunch", "Dinner", "Snack" (required)
    /// </summary>
    [Required]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the meal was consumed (required)
    /// </summary>
    [Required]
    public DateTime Date { get; set; }
}
