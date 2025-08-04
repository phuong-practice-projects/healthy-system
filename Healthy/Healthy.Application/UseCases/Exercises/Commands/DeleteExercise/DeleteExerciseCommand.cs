using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Exercises.Commands.DeleteExercise;

public class DeleteExerciseCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
