using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Queries.GetExercises;

public record GetExercisesQuery : IRequest<ExerciseListResponse>
{
    public Guid UserId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Category { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
