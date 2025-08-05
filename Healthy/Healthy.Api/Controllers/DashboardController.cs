using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecordGraph;
using Healthy.Application.UseCases.Dashboard.Queries.GetDashboardAchievement;
using Healthy.Application.UseCases.Dashboard.Queries.GetDashboardSummary;
using Healthy.Application.UseCases.Meals.Queries.GetMealHistory;
using Healthy.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Healthy.Api.Controllers;

/// <summary>
/// Advanced Dashboard Controller - Architect-level implementation
/// Supports both micro-APIs for performance and aggregated API for convenience
/// </summary>
[ApiController]
[Route("api/dashboard")]
[SwaggerTag("Enterprise-grade dashboard with both micro-APIs and aggregated endpoints for optimal performance")]
public class DashboardController(IMediator mediator) : BaseController
{
    #region Micro-APIs for Optimal Performance  

    /// <summary>
    /// Get today's meal summary - Optimized for quick loading
    /// </summary>
    /// <param name="type">Optional meal type filter for targeted loading</param>
    /// <returns>Lightweight meal data for immediate display</returns>
    [HttpGet("meals-today")]
    [SwaggerOperation(
        Summary = "Get today's meals",
        Description = "Fast-loading endpoint for today's meal data with statistics. Optimized for immediate UI rendering and progressive loading.",
        OperationId = "GetTodayMeals",
        Tags = new[] { "Dashboard" }
    )]
    [ProducesResponseType(typeof(Result<MealHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<MealHistoryResponse>>> GetTodayMeals([FromQuery] string? type = null)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetMealHistoryQuery
        {
            UserId = userId,
            Date = DateTime.Today,
            Type = type  // Add the type filter parameter
        };

        var result = await mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get achievement completion rates - Fast goal tracking
    /// </summary>
    /// <param name="date">Optional date for historical data (default: today)</param>
    /// <returns>Goal completion percentages</returns>
    [HttpGet("achievements")]
    [SwaggerOperation(
        Summary = "Get achievement rates",
        Description = "Lightweight endpoint for goal completion tracking. Perfect for progress indicators and charts.",
        OperationId = "GetAchievements",
        Tags = new[] { "Dashboard" }
    )]
    [ProducesResponseType(typeof(Result<CompletionRateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<CompletionRateResponse>>> GetAchievements([FromQuery] DateTime? date = null)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetDashboardAchievementQuery
        {
            UserId = userId,
            Date = date ?? DateTime.Today
        };

        var result = await mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get user activity summary - Quick stats overview
    /// </summary>
    /// <returns>User engagement and streak data</returns>
    [HttpGet("summary")]
    [SwaggerOperation(
        Summary = "Get user summary",
        Description = "Essential user statistics including streaks and activity trends. Optimized for dashboard header display.",
        OperationId = "GetUserSummary",
        Tags = new[] { "Dashboard" }
    )]
    [ProducesResponseType(typeof(Result<DashboardSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<DashboardSummaryDto>>> GetUserSummary()
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetDashboardSummaryQuery
        {
            UserId = userId
        };

        var result = await mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get weight trend chart data - Optimized graphs
    /// </summary>
    /// <param name="months">Number of months to include (default: 3, max: 12)</param>
    /// <returns>Chart data for weight visualization</returns>
    [HttpGet("weight-chart")]
    [SwaggerOperation(
        Summary = "Get weight chart data",
        Description = "Optimized chart data for weight trend visualization. Configurable time range for performance tuning.",
        OperationId = "GetWeightChart",
        Tags = new[] { "Dashboard" }
    )]
    [ProducesResponseType(typeof(Result<BodyRecordGraphResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<BodyRecordGraphResponse>>> GetWeightChart([FromQuery] int months = 3)
    {
        // Limit months for performance
        months = Math.Max(1, Math.Min(12, months));
        
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetBodyRecordGraphQuery
        {
            UserId = userId,
            StartDate = DateTime.Today.AddMonths(-months),
            EndDate = DateTime.Today
        };

        var result = await mediator.Send(query);
        
        return Ok(result);
    }

    #endregion
}