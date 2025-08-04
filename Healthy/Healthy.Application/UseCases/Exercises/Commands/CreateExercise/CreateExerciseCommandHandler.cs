using MediatR;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.UseCases.Exercises.Commands.CreateExercise;

public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly ILogger<CreateExerciseCommandHandler> _logger;

    public CreateExerciseCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        ILogger<CreateExerciseCommandHandler> logger)
    {
        _context = context;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var exercise = new Exercise
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                CaloriesBurned = request.CaloriesBurned,
                ExerciseDate = request.ExerciseDate,
                Category = request.Category,
                Notes = request.Notes,
                CreatedAt = _dateTime.UtcNow,
            };

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created exercise {Id} for user {UserId}", exercise.Id, request.UserId);

            return Result<Guid>.Success(exercise.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exercise for user {UserId}", request.UserId);
            return Result<Guid>.Failure("Failed to create exercise");
        }
    }
}
