using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class Role : EntityBase
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
} 