using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Meals.Queries.GetMealHistory;

public record GetMealHistoryQuery : IRequest<MealHistoryResponse>
{
    public Guid UserId { get; init; }
    public DateTime? Date { get; init; } // Specific date, defaults to today
    public string? Type { get; init; } // Filter by meal type for dashboard buttons
}
