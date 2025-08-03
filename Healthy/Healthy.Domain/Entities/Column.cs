using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class Column : EntityAuditableBase
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [MaxLength(500)]
    public string? Tags { get; set; }
    
    public bool IsPublished { get; set; } = true;
    
    public Column()
    {
    }
}
