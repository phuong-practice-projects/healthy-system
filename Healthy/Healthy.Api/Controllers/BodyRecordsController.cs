using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecords;
using Healthy.Application.UseCases.BodyRecords.Commands.DeleteBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecordGraph;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Commands.CreateBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Commands.UpdateBodyRecord;
using Healthy.Infrastructure.Authorization;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/records")]
[UserOrAdmin]
public class BodyRecordsController(IMediator mediator) : BaseController
{

    /// <summary>
    /// Get all body records for the current user with advanced filters
    /// </summary>
    /// <param name="request">Query parameters for filtering and pagination</param>
    /// <returns>List of body records with pagination</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BodyRecordsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BodyRecordsListResponse>> GetBodyRecords([FromQuery] GetBodyRecordsQueryRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        // Create query with user ID - using record 'with' expression for immutable update
        var query = request with { UserId = userId };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific body record by ID
    /// </summary>
    /// <param name="id">Body record ID</param>
    /// <returns>Body record details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<BodyRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<BodyRecordDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<BodyRecordDto>>> GetBodyRecord(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetBodyRecordQuery
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
    /// Get body record graph data for the current user
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Body record graph data</returns>
    [HttpGet("graph")]
    [ProducesResponseType(typeof(BodyRecordGraphResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BodyRecordGraphResponse>> GetBodyRecordGraph(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetBodyRecordGraphQuery
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new body record for the current user
    /// </summary>
    /// <param name="request">Body record data</param>
    /// <returns>Created body record ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Guid>>> CreateBodyRecord([FromBody] CreateBodyRecordRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new CreateBodyRecordCommand
        {
            UserId = userId,
            Weight = request.Weight,
            BodyFatPercentage = request.BodyFatPercentage,
            RecordDate = request.RecordDate,
            Notes = request.Notes
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetBodyRecord), new { id = result.Value }, result);
    }

    /// <summary>
    /// Update an existing body record
    /// </summary>
    /// <param name="id">Body record ID</param>
    /// <param name="request">Updated body record data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> UpdateBodyRecord(Guid id, [FromBody] UpdateBodyRecordRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new UpdateBodyRecordCommand
        {
            Id = id,
            UserId = userId,
            Weight = request.Weight,
            BodyFatPercentage = request.BodyFatPercentage,
            RecordDate = request.RecordDate,
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
    /// Delete a body record
    /// </summary>
    /// <param name="id">Body record ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> DeleteBodyRecord(Guid id)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new DeleteBodyRecordCommand
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
