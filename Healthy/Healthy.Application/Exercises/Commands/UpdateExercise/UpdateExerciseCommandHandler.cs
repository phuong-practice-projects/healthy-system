using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Commands.UpdateExercise;

public class UpdateExerciseCommandHandler : IRequestHandler<UpdateExerciseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateExerciseCommandHandler> _logger;

    public UpdateExerciseCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateExerciseCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == request.UserId, cancellationToken);

            if (exercise == null)
            {
                return Result<bool>.Failure("Exercise not found or you don't have permission to update it");
            }

            exercise.Title = request.Title;
            exercise.Description = request.Description;
            exercise.DurationMinutes = request.DurationMinutes;
            exercise.CaloriesBurned = request.CaloriesBurned;
            exercise.ExerciseDate = request.ExerciseDate;
            exercise.Category = request.Category;
            exercise.Notes = request.Notes;
            exercise.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Exercise {Id} updated successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exercise {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while updating the exercise");
        }
    }
}
