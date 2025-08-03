using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Meals.Commands.DeleteMeal;

public class DeleteMealCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
