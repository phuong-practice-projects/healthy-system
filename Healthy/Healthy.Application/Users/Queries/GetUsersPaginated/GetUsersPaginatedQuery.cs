using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQuery : IRequest<UsersListResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
