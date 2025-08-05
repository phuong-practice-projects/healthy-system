using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Exercises.Queries.GetExercise;

public class GetExerciseQueryHandler : IRequestHandler<GetExerciseQuery, Result<ExerciseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetExerciseQueryHandler> _logger;

    public GetExerciseQueryHandler(
        IApplicationDbContext context,
        ILogger<GetExerciseQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<ExerciseDto>> Handle(GetExerciseQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (exercise == null)
            {
                return Result<ExerciseDto>.Failure("Exercise not found or you don't have permission to view it");
            }

            var dto = new ExerciseDto
            {
                Id = exercise.Id,
                UserId = exercise.UserId,
                Title = exercise.Title,
                Description = exercise.Description,
                DurationMinutes = exercise.DurationMinutes,
                CaloriesBurned = exercise.CaloriesBurned,
                ExerciseDate = exercise.ExerciseDate,
                Category = exercise.Category,
                Notes = exercise.Notes,
                CreatedAt = exercise.CreatedAt,
                UpdatedAt = exercise.UpdatedAt
            };

            return Result<ExerciseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exercise {Id} for user {UserId}", request.Id, request.UserId);
            return Result<ExerciseDto>.Failure("An error occurred while retrieving the exercise");
        }
    }
}
