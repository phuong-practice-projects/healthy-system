using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Diaries.Commands.CreateDiary;
using Healthy.Application.UseCases.Diaries.Commands.DeleteDiary;
using Healthy.Application.UseCases.Diaries.Commands.UpdateDiary;
using Healthy.Application.UseCases.Diaries.Queries.GetDiaries;
using Healthy.Application.UseCases.Diaries.Queries.GetDiary;
using Healthy.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[UserOrAdmin]
public class DiariesController(IMediator mediator) : BaseController
{

    /// <summary>
    /// Get diary list for the current user with advanced filters
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="isPrivate">Optional privacy filter</param>
    /// <param name="searchTerm">Optional search term for title or content</param>
    /// <param name="mood">Optional mood filter</param>
    /// <param name="tags">Optional tags filter (comma-separated)</param>
    /// <param name="sortBy">Optional sorting field (Date, Title, Mood)</param>
    /// <param name="sortDirection">Sort direction (Asc, Desc)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <returns>List of diaries with pagination info</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DiaryListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DiaryListResponse>> GetDiaries(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool? isPrivate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? mood = null,
        [FromQuery] string? tags = null,
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

        var query = new GetDiariesQuery
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            IsPrivate = isPrivate,
            SearchTerm = searchTerm,
            Mood = mood,
            Tags = tags,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific diary by ID
    /// </summary>
    /// <param name="id">Diary ID</param>
    /// <returns>Diary details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<DiaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<DiaryDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<DiaryDto>>> GetDiary(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetDiaryQuery
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
    /// Create a new diary for the current user
    /// </summary>
    /// <param name="request">Diary data</param>
    /// <returns>Created diary ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Guid>>> CreateDiary([FromBody] CreateDiaryRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new CreateDiaryCommand
        {
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            Tags = request.Tags,
            Mood = request.Mood,
            IsPrivate = request.IsPrivate,
            DiaryDate = request.DiaryDate
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetDiary), new { id = result.Value }, result);
    }

    /// <summary>
    /// Update an existing diary
    /// </summary>
    /// <param name="id">Diary ID</param>
    /// <param name="request">Updated diary data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> UpdateDiary(Guid id, [FromBody] UpdateDiaryRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new UpdateDiaryCommand
        {
            Id = id,
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            Tags = request.Tags,
            Mood = request.Mood,
            IsPrivate = request.IsPrivate,
            DiaryDate = request.DiaryDate
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
    /// Delete a diary
    /// </summary>
    /// <param name="id">Diary ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> DeleteDiary(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new DeleteDiaryCommand
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
