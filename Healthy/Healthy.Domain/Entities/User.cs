using System.ComponentModel.DataAnnotations;
using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

public class User : EntityAuditableBase
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [MaxLength(10)]
    public string? Gender { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<BodyRecord> BodyRecords { get; set; } = new List<BodyRecord>();
    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    public virtual ICollection<Diary> Diaries { get; set; } = new List<Diary>();
    public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public int Age => DateOfBirth.HasValue 
        ? DateTime.UtcNow.Year - DateOfBirth.Value.Year - (DateTime.UtcNow.Date < DateOfBirth.Value.Date ? 1 : 0)
        : 0;
} 