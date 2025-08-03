using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Dashboard.Queries.GetDashboardAchievement;

/// <summary>
/// Query to get dashboard achievement/completion rate data
/// </summary>
public record GetDashboardAchievementQuery : IRequest<CompletionRateResponse>
{
    /// <summary>
    /// User ID to get achievement data for
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Date to calculate achievement for (defaults to today)
    /// </summary>
    public DateTime? Date { get; init; }
}
