using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Models;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Application.Meals.Queries.GetMealHistory;

/// <summary>
/// Enhanced meal history query handler with advanced analytics
/// Provides comprehensive meal data and statistics for dashboard
/// </summary>
public class GetMealHistoryQueryHandler : IRequestHandler<GetMealHistoryQuery, MealHistoryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMealHistoryQueryHandler> _logger;

    public GetMealHistoryQueryHandler(
        IApplicationDbContext context, 
        ILogger<GetMealHistoryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MealHistoryResponse> Handle(GetMealHistoryQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.Date?.Date ?? DateTime.Today;
        
        _logger.LogInformation("Retrieving meal history for user {UserId} on {Date} with type filter: {Type}", 
            request.UserId, targetDate, request.Type ?? "None");

        var query = _context.Meals
            .Where(m => m.UserId.ToString().ToLower() == request.UserId.ToString().ToLower() && m.Date.Date == targetDate);

        // Apply type filter if specified (for dashboard filter buttons)
        if (!string.IsNullOrEmpty(request.Type))
        {
            query = query.Where(m => m.Type == request.Type);
        }

        var meals = await query
            .OrderBy(m => GetMealTypeOrder(m.Type)) // Order by meal time: Morning -> Lunch -> Dinner -> Snack
            .ThenByDescending(m => m.CreatedAt)
            .Select(m => new MealDto
            {
                Id = m.Id,
                UserId = m.UserId,
                ImageUrl = m.ImageUrl,
                Type = m.Type,
                Date = m.Date,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        // Calculate comprehensive statistics
        var statistics = await CalculateMealStatistics(request.UserId, targetDate, cancellationToken);

        _logger.LogInformation("Retrieved {Count} meals with {Stats} statistics", 
            meals.Count, $"{statistics.TotalMeals} total, {statistics.CompletionPercentage}% completion");

        return new MealHistoryResponse
        {
            Meals = meals,
            Statistics = statistics
        };
    }

    private async Task<MealStatisticsDto> CalculateMealStatistics(Guid userId, DateTime targetDate, CancellationToken cancellationToken)
    {
        // Get all meals for the day (without type filter for complete statistics)
        var allMealsToday = await _context.Meals
            .Where(m => m.UserId == userId && m.Date.Date == targetDate)
            .ToListAsync(cancellationToken);

        // Count by meal type
        var mealTypeCount = allMealsToday
            .GroupBy(m => m.Type)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all main meal types are represented
        var standardMealTypes = new[] { "Morning", "Lunch", "Dinner", "Snack" };
        foreach (var mealType in standardMealTypes)
        {
            if (!mealTypeCount.ContainsKey(mealType))
            {
                mealTypeCount[mealType] = 0;
            }
        }

        // Calculate completion percentage based on main meals (Morning, Lunch, Dinner)
        var mainMealTypes = new[] { "Morning", "Lunch", "Dinner" };
        var completedMainMeals = mainMealTypes.Count(type => mealTypeCount.GetValueOrDefault(type, 0) > 0);
        var completionPercentage = (decimal)completedMainMeals / mainMealTypes.Length * 100;

        return new MealStatisticsDto
        {
            TotalMeals = allMealsToday.Count,
            MealTypeCount = mealTypeCount,
            CompletionPercentage = Math.Round(completionPercentage, 1)
        };
    }

    private static int GetMealTypeOrder(string type)
    {
        return type switch
        {
            "Morning" => 1,
            "Lunch" => 2,
            "Dinner" => 3,
            "Snack" => 4,
            _ => 5
        };
    }
}
