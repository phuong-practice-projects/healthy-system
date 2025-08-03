using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
