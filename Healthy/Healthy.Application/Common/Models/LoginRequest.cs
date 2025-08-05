using System.ComponentModel.DataAnnotations;

namespace Healthy.Application.Common.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(1, ErrorMessage = "Password cannot be empty")]
    public string Password { get; set; } = string.Empty;
} 