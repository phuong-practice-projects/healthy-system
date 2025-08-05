using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class Category : EntityAuditableBase
{
    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? CategoryType { get; set; }
    
    public DateTime PublishedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Tags stored as JSON string or comma-separated values
    [MaxLength(500)]
    public string Tags { get; set; } = string.Empty;
    
    public Category()
    {
        PublishedAt = DateTime.UtcNow;
    }
}
