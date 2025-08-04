using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>; 