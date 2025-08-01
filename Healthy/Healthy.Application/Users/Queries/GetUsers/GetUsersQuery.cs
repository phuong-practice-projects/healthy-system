using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<PaginatedList<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool IsDescending { get; init; } = false;
    public bool? IsActive { get; init; }
} 