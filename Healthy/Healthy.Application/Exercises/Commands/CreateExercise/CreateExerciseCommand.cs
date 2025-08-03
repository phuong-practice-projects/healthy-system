using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Commands.CreateExercise;

public record CreateExerciseCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DurationMinutes { get; init; }
    public int CaloriesBurned { get; init; }
    public DateTime ExerciseDate { get; init; }
    public string? Category { get; init; }
    public string? Notes { get; init; }
}
