using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Queries.GetExercise;

public class GetExerciseQuery : IRequest<Result<ExerciseDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
