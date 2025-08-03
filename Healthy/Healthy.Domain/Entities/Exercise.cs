using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class Exercise : EntityAuditableBase
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public int DurationMinutes { get; set; }
    
    [Range(0, 10000)]
    public int CaloriesBurned { get; set; }
    
    [Required]
    public DateTime ExerciseDate { get; set; }
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
