using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Users.Commands.Login;

public record LoginCommand : IRequest<AuthResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
} 