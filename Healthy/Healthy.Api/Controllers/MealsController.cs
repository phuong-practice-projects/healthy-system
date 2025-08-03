using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Healthy.Application.Meals.Commands.CreateMeal;
using Healthy.Application.Meals.Commands.UpdateMeal;
using Healthy.Application.Meals.Commands.DeleteMeal;
using Healthy.Application.Meals.Queries.GetMeals;
using Healthy.Application.Common.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[SwaggerTag("Meal management endpoints for CRUD operations")]
public class MealsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Get meal list for the current user with pagination and filters
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="type">Optional meal type filter (Morning, Lunch, Dinner, Snack)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>List of meals with pagination info</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get user meals with pagination",
        Description = "Retrieves all meals for the current user with optional filters and pagination support",
        OperationId = "GetMeals",
        Tags = new[] { "Meals" }
    )]
    [ProducesResponseType(typeof(MealListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(200, "Meals retrieved successfully", typeof(MealListResponse))]
    [SwaggerResponse(401, "User not authenticated")]
    public async Task<ActionResult<MealListResponse>> GetMeals(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetMealsQuery
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            Type = type,
            Page = page,
            PageSize = pageSize
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new meal for the current user
    /// </summary>
    /// <param name="request">Meal data</param>
    /// <returns>Created meal ID</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create new meal",
        Description = "Creates a new meal record for the current user",
        OperationId = "CreateMeal",
        Tags = new[] { "Meals" }
    )]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(201, "Meal created successfully", typeof(Result<Guid>))]
    [SwaggerResponse(400, "Invalid meal data", typeof(Result<Guid>))]
    [SwaggerResponse(401, "User not authenticated")]
    public async Task<ActionResult<Result<Guid>>> CreateMeal([FromBody] CreateMealRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new CreateMealCommand
        {
            UserId = userId,
            ImageUrl = request.ImageUrl,
            Type = request.Type,
            Date = request.Date
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetMeals), new { id = result.Value }, result);
    }

    /// <summary>
    /// Update an existing meal
    /// </summary>
    /// <param name="id">Meal ID</param>
    /// <param name="request">Updated meal data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update meal",
        Description = "Updates an existing meal for the current user",
        OperationId = "UpdateMeal",
        Tags = new[] { "Meals" }
    )]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(200, "Meal updated successfully", typeof(Result<bool>))]
    [SwaggerResponse(400, "Invalid meal data", typeof(Result<bool>))]
    [SwaggerResponse(404, "Meal not found", typeof(Result<bool>))]
    [SwaggerResponse(401, "User not authenticated")]
    public async Task<ActionResult<Result<bool>>> UpdateMeal(Guid id, [FromBody] UpdateMealRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new UpdateMealCommand
        {
            Id = id,
            UserId = userId,
            ImageUrl = request.ImageUrl,
            Type = request.Type,
            Date = request.Date
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a meal
    /// </summary>
    /// <param name="id">Meal ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete meal",
        Description = "Deletes a meal for the current user",
        OperationId = "DeleteMeal",
        Tags = new[] { "Meals" }
    )]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(200, "Meal deleted successfully", typeof(Result<bool>))]
    [SwaggerResponse(404, "Meal not found", typeof(Result<bool>))]
    [SwaggerResponse(401, "User not authenticated")]
    public async Task<ActionResult<Result<bool>>> DeleteMeal(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new DeleteMealCommand
        {
            Id = id,
            UserId = userId
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }
}
