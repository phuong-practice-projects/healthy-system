using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Commands.DeleteExercise;

public class DeleteExerciseCommandHandler : IRequestHandler<DeleteExerciseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteExerciseCommandHandler> _logger;

    public DeleteExerciseCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteExerciseCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exercise = await _context.Exercises
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (exercise == null)
            {
                return Result<bool>.Failure("Exercise not found or you don't have permission to delete it");
            }

            if (exercise.DeletedAt.HasValue)
            {
                return Result<bool>.Failure("Exercise is already deleted");
            }

            // Soft delete the exercise
            exercise.Delete(); // This will set DeletedAt and DeletedBy
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Exercise {Id} soft deleted successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting exercise {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while deleting the exercise");
        }
    }
}
