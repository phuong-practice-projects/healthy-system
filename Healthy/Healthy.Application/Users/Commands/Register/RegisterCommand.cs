using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Users.Commands.Register;

public record RegisterCommand : IRequest<AuthResult>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
} 