using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class BodyRecord : EntityAuditableBase
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [Range(0, 500)]
    public decimal Weight { get; set; }
    
    [Range(0, 100)]
    public decimal? BodyFatPercentage { get; set; }
    
    [Required]
    public DateTime RecordDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
