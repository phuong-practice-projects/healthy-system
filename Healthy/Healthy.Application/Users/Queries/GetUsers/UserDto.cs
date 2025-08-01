namespace Healthy.Application.Users.Queries.GetUsers;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}".Trim();
    public int Age => DateOfBirth.HasValue 
        ? DateTime.UtcNow.Year - DateOfBirth.Value.Year - (DateTime.UtcNow.Date < DateOfBirth.Value.Date ? 1 : 0)
        : 0;
} 