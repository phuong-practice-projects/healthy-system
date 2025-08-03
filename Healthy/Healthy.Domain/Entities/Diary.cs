using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class Diary : EntityAuditableBase
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Tags { get; set; }
    
    [MaxLength(100)]
    public string? Mood { get; set; }
    
    public bool IsPrivate { get; set; } = true;
    
    [Required]
    public DateTime DiaryDate { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
