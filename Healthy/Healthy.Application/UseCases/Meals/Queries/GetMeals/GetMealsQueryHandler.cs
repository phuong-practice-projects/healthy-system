using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Models;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Application.UseCases.Meals.Queries.GetMeals;

public class GetMealsQueryHandler : IRequestHandler<GetMealsQuery, MealListResponse>
{
    private readonly IApplicationDbContext _context;

    public GetMealsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MealListResponse> Handle(GetMealsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Meals
            .Where(m => m.UserId.ToString().ToLower() == request.UserId.ToString().ToLower());

        // Apply date range filter
        if (request.StartDate.HasValue)
        {
            query = query.Where(m => m.Date >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(m => m.Date <= request.EndDate.Value);
        }

        // Apply type filter
        if (!string.IsNullOrEmpty(request.Type))
        {
            query = query.Where(m => m.Type == request.Type);
        }

        // Get total count for pagination
        var totalItems = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var meals = await query
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MealDto
            {
                Id = m.Id,
                UserId = m.UserId,
                ImageUrl = m.ImageUrl,
                Type = m.Type,
                Date = m.Date,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new MealListResponse
        {
            Meals = meals,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };
    }
}
