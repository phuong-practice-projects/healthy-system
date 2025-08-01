using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUsersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Set<User>()
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(searchTerm)));
            }

            // Apply active filter
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "firstname" => request.IsDescending 
                    ? query.OrderByDescending(u => u.FirstName)
                    : query.OrderBy(u => u.FirstName),
                "lastname" => request.IsDescending 
                    ? query.OrderByDescending(u => u.LastName)
                    : query.OrderBy(u => u.LastName),
                "email" => request.IsDescending 
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "createdat" => request.IsDescending 
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                _ => request.IsDescending 
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt)
            };

            // Project to DTO
            var dtoQuery = query.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });

            var result = await PaginatedList<UserDto>.CreateAsync(
                dtoQuery, request.PageNumber, request.PageSize);

            _logger.LogInformation("Retrieved {Count} users from page {PageNumber}", 
                result.Items.Count, request.PageNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }
} 