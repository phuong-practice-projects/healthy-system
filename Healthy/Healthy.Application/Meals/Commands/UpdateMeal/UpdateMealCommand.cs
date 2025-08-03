using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Meals.Commands.UpdateMeal;

public class UpdateMealCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Morning", "Lunch", "Dinner", "Snack"
    public DateTime Date { get; set; }
}
