using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Queries.GetExercises;

public class GetExercisesQueryHandler : IRequestHandler<GetExercisesQuery, ExerciseListResponse>
{
    private readonly IApplicationDbContext _context;

    public GetExercisesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExerciseListResponse> Handle(GetExercisesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Exercises
            .Where(e => e.UserId == request.UserId && !e.IsDeleted);

        if (request.StartDate.HasValue)
        {
            query = query.Where(e => e.ExerciseDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(e => e.ExerciseDate <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(e => e.Category == request.Category);
        }

        var exercises = await query
            .OrderByDescending(e => e.ExerciseDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new ExerciseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                DurationMinutes = e.DurationMinutes,
                CaloriesBurned = e.CaloriesBurned,
                ExerciseDate = e.ExerciseDate,
                Category = e.Category,
                Notes = e.Notes
            })
            .ToListAsync(cancellationToken);

        return new ExerciseListResponse
        {
            Exercises = exercises
        };
    }
}
