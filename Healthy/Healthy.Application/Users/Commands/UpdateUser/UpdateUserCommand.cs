using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
}
