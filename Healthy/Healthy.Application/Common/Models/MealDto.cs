using System.ComponentModel.DataAnnotations;

namespace Healthy.Application.Common.Models;

/// <summary>
/// Data transfer object for meal information
/// </summary>
public class MealDto
{
    /// <summary>
    /// Unique identifier for the meal
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// ID of the user who owns this meal
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// URL or path to the meal image
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of meal: "Morning", "Lunch", "Dinner", "Snack"
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the meal was consumed
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// When the meal record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the meal record was last updated (if applicable)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
