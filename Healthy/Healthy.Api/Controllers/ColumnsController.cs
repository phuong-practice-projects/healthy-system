using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Columns.Commands.CreateColumn;
using Healthy.Application.UseCases.Columns.Commands.DeleteColumn;
using Healthy.Application.UseCases.Columns.Commands.UpdateColumn;
using Healthy.Application.UseCases.Columns.Queries.GetColumn;
using Healthy.Application.UseCases.Columns.Queries.GetColumns;
using Healthy.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[UserOrAdmin]
public class ColumnsController(IMediator mediator) : BaseController
{

    /// <summary>
    /// Get a specific column by ID
    /// </summary>
    /// <param name="id">Column ID</param>
    /// <returns>Column details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ColumnDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ColumnDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ColumnDto>>> GetColumn(Guid id)
    {
        var query = new GetColumnQuery { Id = id };
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new column (requires Admin role)
    /// </summary>
    /// <param name="request">Column data</param>
    /// <returns>Created column ID</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<Guid>>> CreateColumn([FromBody] CreateColumnRequest request)
    {
        var command = new CreateColumnCommand
        {
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            Tags = request.Tags,
            IsPublished = request.IsPublished
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetColumn), new { id = result.Value }, result);
    }

    /// <summary>
    /// Update an existing column (requires Admin role)
    /// </summary>
    /// <param name="id">Column ID</param>
    /// <param name="request">Updated column data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> UpdateColumn(Guid id, [FromBody] UpdateColumnRequest request)
    {
        var command = new UpdateColumnCommand
        {
            Id = id,
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            Tags = request.Tags,
            IsPublished = request.IsPublished
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
    /// Delete a column (requires Admin role)
    /// </summary>
    /// <param name="id">Column ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> DeleteColumn(Guid id)
    {
        var command = new DeleteColumnCommand { Id = id };
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
    /// Get list of health column articles
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="limit">Items per page (default: 10)</param>
    /// <param name="category">Filter by category (optional: diet, recommended, beauty)</param>
    /// <returns>List of column articles with pagination</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ColumnDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedList<ColumnDto>>> GetColumns(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? category = null)
    {
        if (page < 1) page = 1;
        if (limit < 1 || limit > 100) limit = 10; // Limit maximum items per page

        var query = new GetColumnsQuery
        {
            Page = page,
            Limit = limit,
            Category = category
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }
}
