using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Dashboard.Queries.GetDashboardSummary;

/// <summary>
/// Query to get dashboard summary statistics
/// </summary>
public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
{
    /// <summary>
    /// User ID to get summary data for
    /// </summary>
    public Guid UserId { get; init; }
}
