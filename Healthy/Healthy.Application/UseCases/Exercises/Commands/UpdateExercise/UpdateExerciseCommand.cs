using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Exercises.Commands.UpdateExercise;

public class UpdateExerciseCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public DateTime ExerciseDate { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
}
