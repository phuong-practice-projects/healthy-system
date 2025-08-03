using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Dashboard.Queries.GetDashboardAchievement;

/// <summary>
/// Handler for getting dashboard achievement/completion rate data
/// Advanced algorithm for calculating user progress across multiple health metrics
/// </summary>
public class GetDashboardAchievementQueryHandler : IRequestHandler<GetDashboardAchievementQuery, CompletionRateResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDashboardAchievementQueryHandler> _logger;

    public GetDashboardAchievementQueryHandler(
        IApplicationDbContext context,
        ILogger<GetDashboardAchievementQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CompletionRateResponse> Handle(GetDashboardAchievementQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var targetDate = request.Date?.Date ?? DateTime.Today;
            
            _logger.LogInformation("Calculating achievement rate for user {UserId} on {Date}", 
                request.UserId, targetDate);

            // Initialize categories with default goals
            var categories = new List<GoalCategoryStatusDto>();

            // 1. Meal completion analysis
            var mealCategory = await AnalyzeMealCompletion(request.UserId, targetDate, cancellationToken);
            categories.Add(mealCategory);

            // 2. Exercise completion analysis
            var exerciseCategory = await AnalyzeExerciseCompletion(request.UserId, targetDate, cancellationToken);
            categories.Add(exerciseCategory);

            // 3. Body record tracking analysis
            var trackingCategory = await AnalyzeTrackingCompletion(request.UserId, targetDate, cancellationToken);
            categories.Add(trackingCategory);

            // 4. Diary/reflection completion analysis
            var reflectionCategory = await AnalyzeReflectionCompletion(request.UserId, targetDate, cancellationToken);
            categories.Add(reflectionCategory);

            // Calculate overall completion
            var totalCompleted = categories.Sum(c => c.Completed);
            var totalGoals = categories.Sum(c => c.Total);
            var overallRate = totalGoals > 0 ? (decimal)totalCompleted / totalGoals * 100 : 0;

            var response = new CompletionRateResponse
            {
                Rate = Math.Round(overallRate, 1),
                Message = GenerateMotivationalMessage(overallRate, totalCompleted, totalGoals),
                Breakdown = new GoalBreakdownDto
                {
                    CompletedGoals = totalCompleted,
                    TotalGoals = totalGoals,
                    Categories = categories
                }
            };

            _logger.LogInformation("Achievement calculation completed: {Rate}% ({Completed}/{Total})",
                response.Rate, totalCompleted, totalGoals);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating achievement rate for user {UserId}", request.UserId);
            
            // Return graceful fallback
            return new CompletionRateResponse
            {
                Rate = 0,
                Message = "Unable to calculate achievement rate at this time",
                Breakdown = new GoalBreakdownDto()
            };
        }
    }

    private async Task<GoalCategoryStatusDto> AnalyzeMealCompletion(Guid userId, DateTime targetDate, CancellationToken cancellationToken)
    {
        // Define ideal meal structure: Morning, Lunch, Dinner (3 main meals)
        var idealMealTypes = new[] { "Morning", "Lunch", "Dinner" };
        
        var mealsToday = await _context.Meals
            .Where(m => m.UserId == userId && m.Date.Date == targetDate)
            .Select(m => m.Type)
            .ToListAsync(cancellationToken);

        var completedMeals = idealMealTypes.Count(mealType => mealsToday.Contains(mealType));

        return new GoalCategoryStatusDto
        {
            Category = "Daily Meals",
            Completed = completedMeals,
            Total = idealMealTypes.Length,
            Percentage = Math.Round((decimal)completedMeals / idealMealTypes.Length * 100, 1)
        };
    }

    private async Task<GoalCategoryStatusDto> AnalyzeExerciseCompletion(Guid userId, DateTime targetDate, CancellationToken cancellationToken)
    {
        // Goal: At least 1 exercise session per day
        var exerciseCount = await _context.Exercises
            .Where(e => e.UserId == userId && e.ExerciseDate.Date == targetDate && !e.IsDeleted)
            .CountAsync(cancellationToken);

        var completed = Math.Min(exerciseCount, 1); // Cap at 1 for daily goal
        
        return new GoalCategoryStatusDto
        {
            Category = "Exercise",
            Completed = completed,
            Total = 1,
            Percentage = completed * 100
        };
    }

    private async Task<GoalCategoryStatusDto> AnalyzeTrackingCompletion(Guid userId, DateTime targetDate, CancellationToken cancellationToken)
    {
        // Goal: Record body metrics at least once per day
        var bodyRecordCount = await _context.BodyRecords
            .Where(br => br.UserId == userId && br.RecordDate.Date == targetDate)
            .CountAsync(cancellationToken);

        var completed = Math.Min(bodyRecordCount, 1); // Cap at 1 for daily goal
        
        return new GoalCategoryStatusDto
        {
            Category = "Body Tracking",
            Completed = completed,
            Total = 1,
            Percentage = completed * 100
        };
    }

    private async Task<GoalCategoryStatusDto> AnalyzeReflectionCompletion(Guid userId, DateTime targetDate, CancellationToken cancellationToken)
    {
        // Goal: Write at least 1 diary entry per day for reflection
        var diaryCount = await _context.Diaries
            .Where(d => d.UserId == userId && d.DiaryDate.Date == targetDate)
            .CountAsync(cancellationToken);

        var completed = Math.Min(diaryCount, 1); // Cap at 1 for daily goal
        
        return new GoalCategoryStatusDto
        {
            Category = "Daily Reflection",
            Completed = completed,
            Total = 1,
            Percentage = completed * 100
        };
    }

    private static string GenerateMotivationalMessage(decimal rate, int completed, int total)
    {
        return rate switch
        {
            100 => "🎉 Perfect day! You've achieved all your health goals!",
            >= 80 => $"🌟 Excellent progress! {completed}/{total} goals completed.",
            >= 60 => $"👍 Good work! {completed}/{total} goals done. Keep pushing!",
            >= 40 => $"💪 You're on track! {completed}/{total} goals completed.",
            >= 20 => $"🏃 Getting started! {completed}/{total} goals done. Every step counts!",
            > 0 => $"🌱 Great start! {completed}/{total} goals completed. Build momentum!",
            _ => "🎯 New day, new opportunities! Start with one small goal."
        };
    }
}
