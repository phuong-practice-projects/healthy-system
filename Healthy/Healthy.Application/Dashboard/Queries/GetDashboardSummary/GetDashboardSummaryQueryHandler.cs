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
public class GetDashboardSummaryQueryHandler(
    IApplicationDbContext context,
    ILogger<GetDashboardSummaryQueryHandler> logger) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Calculating dashboard summary for user {UserId}", request.UserId);

            var summary = new DashboardSummaryDto();

            // Calculate activity streaks and engagement metrics
            await CalculateStreakData(request.UserId, summary, cancellationToken);
            
            // Get current weight and trend analysis
            await CalculateWeightMetrics(request.UserId, summary, cancellationToken);
            
            // Calculate total active days
            await CalculateActivityMetrics(request.UserId, summary, cancellationToken);

            logger.LogInformation("Dashboard summary calculated: {ActiveDays} active days, current streak: {Streak}", 
                summary.TotalActiveDays, summary.CurrentStreak);

            return summary;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating dashboard summary for user {UserId}", request.UserId);
            return new DashboardSummaryDto(); // Return empty summary on error
        }
    }

    private async Task CalculateStreakData(Guid userId, DashboardSummaryDto summary, CancellationToken cancellationToken)
    {
        string userIdStr = userId.ToString().ToLower();

        // Collect all activity dates in parallel
        var mealDatesTask = context.Meals
            .Where(m => m.UserId.ToString().ToLower() == userIdStr)
            .Select(m => m.Date.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        var exerciseDatesTask = context.Exercises
            .Where(e => e.UserId.ToString().ToLower() == userIdStr && e.DeletedAt == null)
            .Select(e => e.ExerciseDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        var bodyRecordDatesTask = context.BodyRecords
            .Where(br => br.UserId.ToString().ToLower() == userIdStr)
            .Select(br => br.RecordDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        var diaryDatesTask = context.Diaries
            .Where(d => d.UserId.ToString().ToLower() == userIdStr)
            .Select(d => d.DiaryDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        await Task.WhenAll(mealDatesTask, exerciseDatesTask, bodyRecordDatesTask, diaryDatesTask);

        var activityDates = new HashSet<DateTime>();
        activityDates.UnionWith(mealDatesTask.Result);
        activityDates.UnionWith(exerciseDatesTask.Result);
        activityDates.UnionWith(bodyRecordDatesTask.Result);
        activityDates.UnionWith(diaryDatesTask.Result);

        if (activityDates.Count == 0)
        {
            summary.CurrentStreak = 0;
            summary.BestStreak = 0;
            return;
        }

        // Calculate current streak (consecutive days from today backwards)
        var sortedDatesDesc = activityDates.OrderByDescending(d => d).ToList();
        int currentStreak = 0;
        DateTime checkDate = DateTime.Today;

        foreach (var date in sortedDatesDesc)
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
        var sortedDatesAsc = activityDates.OrderBy(d => d).ToList();
        int bestStreak = 1, streak = 1;
        for (int i = 1; i < sortedDatesAsc.Count; i++)
        {
            if (sortedDatesAsc[i] == sortedDatesAsc[i - 1].AddDays(1))
            {
                streak++;
                if (streak > bestStreak) bestStreak = streak;
            }
            else
            {
                streak = 1;
            }
        }

        summary.CurrentStreak = currentStreak;
        summary.BestStreak = bestStreak;
    }

    private static int CalculateLongestStreak(List<DateTime> sortedDates)
    {
        if (sortedDates.Count == 0) return 0;

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

        string userIdStr = userId.ToString().ToLower();
        // Get most recent weight record
        var latestWeight = await context.BodyRecords
            .Where(br => br.UserId.ToString().ToLower() == userIdStr)
            .OrderByDescending(br => br.RecordDate)
            .Select(br => (decimal?)br.Weight)
            .FirstOrDefaultAsync(cancellationToken);

        summary.CurrentWeight = latestWeight;

        // Calculate weight change from last month
        var oneMonthAgo = DateTime.Today.AddMonths(-1);
        var weightLastMonth = await context.BodyRecords
            .Where(br => br.UserId.ToString().ToLower() == userIdStr  && br.RecordDate.Date <= oneMonthAgo)
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
        string userIdStr = userId.ToString().ToLower();
        // Get user's first activity date to calculate total active period
        var firstActivityDates = await Task.WhenAll(
            context.Meals
                .Where(m => m.UserId.ToString().ToLower() == userIdStr)
                .OrderBy(m => m.CreatedAt)
                .Select(m => (DateTime?)m.CreatedAt.Date)
                .FirstOrDefaultAsync(cancellationToken),
            context.Exercises
                .Where(e => e.UserId.ToString().ToLower() == userIdStr && e.DeletedAt == null)
                .OrderBy(e => e.CreatedAt)
                .Select(e => (DateTime?)e.CreatedAt.Date)
                .FirstOrDefaultAsync(cancellationToken),
            context.BodyRecords
                .Where(br => br.UserId.ToString().ToLower() == userIdStr)
                .OrderBy(br => br.CreatedAt)
                .Select(br => (DateTime?)br.CreatedAt.Date)
                .FirstOrDefaultAsync(cancellationToken),
            context.Diaries
                .Where(d => d.UserId.ToString().ToLower() == userIdStr)
                .OrderBy(d => d.CreatedAt)
                .Select(d => (DateTime?)d.CreatedAt.Date)
                .FirstOrDefaultAsync(cancellationToken)
        );

        var firstActivityDate = firstActivityDates
            .Where(d => d.HasValue)
            .Select(d => d.Value)
            .DefaultIfEmpty(DateTime.MaxValue)
            .Min();

        if (firstActivityDate != DateTime.MaxValue)
        {
            summary.TotalActiveDays = (DateTime.Today - firstActivityDate).Days + 1;
        }
    }

    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}
