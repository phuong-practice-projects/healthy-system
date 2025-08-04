using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Users.Queries.GetUser;

public class GetUserQuery : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
}
