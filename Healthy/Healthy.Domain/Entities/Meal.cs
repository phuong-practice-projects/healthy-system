using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

/// <summary>
/// Represents a meal record with associated image and type for a specific user
/// </summary>
public class Meal : EntityAuditableBase
{
    /// <summary>
    /// The ID of the user who owns this meal record
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// URL or path to the meal image (max 500 characters)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of meal - "Morning", "Lunch", "Dinner", "Snack"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the meal was consumed
    /// </summary>
    [Required]
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Navigation property to the User entity
    /// </summary>
    public virtual User User { get; set; } = null!;
}
