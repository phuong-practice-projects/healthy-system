using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, UsersListResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUsersPaginatedQueryHandler> _logger;

    public GetUsersPaginatedQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUsersPaginatedQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UsersListResponse> Handle(GetUsersPaginatedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            // Apply filters
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == request.Role));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = $"{u.FirstName} {u.LastName}",
                    PhoneNumber = u.PhoneNumber,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    IsActive = u.IsActive,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return new UsersListResponse
            {
                Users = users,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated users list");
            return new UsersListResponse
            {
                Users = new List<UserDto>(),
                TotalItems = 0,
                TotalPages = 0,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
