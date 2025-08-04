using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Healthy.Application.Exercises.Commands.CreateExercise;
using Healthy.Application.Exercises.Commands.UpdateExercise;
using Healthy.Application.Exercises.Commands.DeleteExercise;
using Healthy.Application.Exercises.Queries.GetExercise;
using Healthy.Application.Exercises.Queries.GetExercises;
using Healthy.Application.Common.Models;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController(IMediator mediator) : BaseController
{

    /// <summary>
    /// Get exercise list for the current user with advanced filters
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="searchTerm">Optional search term for title or description</param>
    /// <param name="minDuration">Optional minimum duration filter (minutes)</param>
    /// <param name="maxDuration">Optional maximum duration filter (minutes)</param>
    /// <param name="minCalories">Optional minimum calories burned filter</param>
    /// <param name="maxCalories">Optional maximum calories burned filter</param>
    /// <param name="sortBy">Optional sorting field (Date, Duration, Calories, Title, Category)</param>
    /// <param name="sortDirection">Sort direction (Asc, Desc)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <returns>List of exercises with pagination info</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ExerciseListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ExerciseListResponse>> GetExercises(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? category = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? minDuration = null,
        [FromQuery] int? maxDuration = null,
        [FromQuery] int? minCalories = null,
        [FromQuery] int? maxCalories = null,
        [FromQuery] string sortBy = "Date",
        [FromQuery] string sortDirection = "Desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetExercisesQuery
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            Category = category,
            SearchTerm = searchTerm,
            MinDuration = minDuration,
            MaxDuration = maxDuration,
            MinCalories = minCalories,
            MaxCalories = maxCalories,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific exercise by ID
    /// </summary>
    /// <param name="id">Exercise ID</param>
    /// <returns>Exercise details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ExerciseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<ExerciseDto>>> GetExercise(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetExerciseQuery
        {
            Id = id,
            UserId = userId
        };

        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new exercise for the current user
    /// </summary>
    /// <param name="request">Exercise data</param>
    /// <returns>Created exercise ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Guid>>> CreateExercise([FromBody] CreateExerciseRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new CreateExerciseCommand
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            CaloriesBurned = request.CaloriesBurned,
            ExerciseDate = request.ExerciseDate,
            Category = request.Category,
            Notes = request.Notes
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetExercise), new { id = result.Value }, result);
    }

    /// <summary>
    /// Update an existing exercise
    /// </summary>
    /// <param name="id">Exercise ID</param>
    /// <param name="request">Updated exercise data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> UpdateExercise(Guid id, [FromBody] UpdateExerciseRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new UpdateExerciseCommand
        {
            Id = id,
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            CaloriesBurned = request.CaloriesBurned,
            ExerciseDate = request.ExerciseDate,
            Category = request.Category,
            Notes = request.Notes
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
    /// Delete an exercise
    /// </summary>
    /// <param name="id">Exercise ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> DeleteExercise(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new DeleteExerciseCommand
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
