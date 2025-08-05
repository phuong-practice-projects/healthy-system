using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Meals.Commands.CreateMeal;

/// <summary>
/// Command to create a new meal record for a user
/// </summary>
public class CreateMealCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// ID of the user creating the meal
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// URL or path to the meal image
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of meal: "Morning", "Lunch", "Dinner", "Snack"
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the meal was consumed
    /// </summary>
    public DateTime Date { get; set; }
}
