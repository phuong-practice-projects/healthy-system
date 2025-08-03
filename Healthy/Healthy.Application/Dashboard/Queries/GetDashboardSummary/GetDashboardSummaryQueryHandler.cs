using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Dashboard.Queries.GetDashboardSummary;

/// <summary>
/// Handler for getting dashboard summary statistics
/// Advanced analytics for user engagement and progress tracking
/// </summary>
public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDashboardSummaryQueryHandler> _logger;

    public GetDashboardSummaryQueryHandler(
        IApplicationDbContext context,
        ILogger<GetDashboardSummaryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Calculating dashboard summary for user {UserId}", request.UserId);

            var summary = new DashboardSummaryDto();

            // Calculate activity streaks and engagement metrics
            await CalculateStreakData(request.UserId, summary, cancellationToken);
            
            // Get current weight and trend analysis
            await CalculateWeightMetrics(request.UserId, summary, cancellationToken);
            
            // Calculate total active days
            await CalculateActivityMetrics(request.UserId, summary, cancellationToken);

            _logger.LogInformation("Dashboard summary calculated: {ActiveDays} active days, current streak: {Streak}", 
                summary.TotalActiveDays, summary.CurrentStreak);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating dashboard summary for user {UserId}", request.UserId);
            return new DashboardSummaryDto(); // Return empty summary on error
        }
    }

    private async Task CalculateStreakData(Guid userId, DashboardSummaryDto summary, CancellationToken cancellationToken)
    {
        // Get all days where user had some activity (meals, exercises, diaries, or body records)
        var activityDates = new HashSet<DateTime>();

        // Meal activity dates
        var mealDates = await _context.Meals
            .Where(m => m.UserId == userId)
            .Select(m => m.Date.Date)
            .Distinct()
            .ToListAsync(cancellationToken);
        activityDates.UnionWith(mealDates);

        // Exercise activity dates
        var exerciseDates = await _context.Exercises
            .Where(e => e.UserId == userId && !e.IsDeleted)
            .Select(e => e.ExerciseDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);
        activityDates.UnionWith(exerciseDates);

        // Body record dates
        var bodyRecordDates = await _context.BodyRecords
            .Where(br => br.UserId == userId)
            .Select(br => br.RecordDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);
        activityDates.UnionWith(bodyRecordDates);

        // Diary dates
        var diaryDates = await _context.Diaries
            .Where(d => d.UserId == userId)
            .Select(d => d.DiaryDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);
        activityDates.UnionWith(diaryDates);

        // Calculate current streak (consecutive days from today backwards)
        var sortedDates = activityDates.OrderByDescending(d => d).ToList();
        var currentStreak = 0;
        var checkDate = DateTime.Today;

        foreach (var date in sortedDates)
        {
            if (date == checkDate)
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }
            else if (date < checkDate)
            {
                break; // Gap found, streak ends
            }
        }

        // Calculate best streak (longest consecutive sequence)
        var bestStreak = CalculateLongestStreak(activityDates.OrderBy(d => d).ToList());

        summary.CurrentStreak = currentStreak;
        summary.BestStreak = bestStreak;
    }

    private static int CalculateLongestStreak(List<DateTime> sortedDates)
    {
        if (!sortedDates.Any()) return 0;

        var bestStreak = 1;
        var currentStreak = 1;

        for (int i = 1; i < sortedDates.Count; i++)
        {
            // Check if consecutive days
            if (sortedDates[i] == sortedDates[i - 1].AddDays(1))
            {
                currentStreak++;
                bestStreak = Math.Max(bestStreak, currentStreak);
            }
            else
            {
                currentStreak = 1; // Reset streak
            }
        }

        return bestStreak;
    }

    private async Task CalculateWeightMetrics(Guid userId, DashboardSummaryDto summary, CancellationToken cancellationToken)
    {
        // Get most recent weight record
        var latestWeight = await _context.BodyRecords
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.RecordDate)
            .Select(br => (decimal?)br.Weight)
            .FirstOrDefaultAsync(cancellationToken);

        summary.CurrentWeight = latestWeight;

        // Calculate weight change from last month
        var oneMonthAgo = DateTime.Today.AddMonths(-1);
        var weightLastMonth = await _context.BodyRecords
            .Where(br => br.UserId == userId && br.RecordDate.Date <= oneMonthAgo)
            .OrderByDescending(br => br.RecordDate)
            .Select(br => (decimal?)br.Weight)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestWeight.HasValue && weightLastMonth.HasValue)
        {
            summary.WeightChange = latestWeight.Value - weightLastMonth.Value;
        }
    }

    private async Task CalculateActivityMetrics(Guid userId, DashboardSummaryDto summary, CancellationToken cancellationToken)
    {
        // Get user's first activity date to calculate total active period
        var firstActivityDate = DateTime.MaxValue;

        var firstMeal = await _context.Meals
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => m.CreatedAt.Date)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstMeal != default) firstActivityDate = Min(firstActivityDate, firstMeal);

        var firstExercise = await _context.Exercises
            .Where(e => e.UserId == userId && !e.IsDeleted)
            .OrderBy(e => e.CreatedAt)
            .Select(e => e.CreatedAt.Date)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstExercise != default) firstActivityDate = Min(firstActivityDate, firstExercise);

        var firstBodyRecord = await _context.BodyRecords
            .Where(br => br.UserId == userId)
            .OrderBy(br => br.CreatedAt)
            .Select(br => br.CreatedAt.Date)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstBodyRecord != default) firstActivityDate = Min(firstActivityDate, firstBodyRecord);

        var firstDiary = await _context.Diaries
            .Where(d => d.UserId == userId)
            .OrderBy(d => d.CreatedAt)
            .Select(d => d.CreatedAt.Date)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstDiary != default) firstActivityDate = Min(firstActivityDate, firstDiary);

        if (firstActivityDate != DateTime.MaxValue)
        {
            summary.TotalActiveDays = (DateTime.Today - firstActivityDate).Days + 1;
        }
    }

    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}
