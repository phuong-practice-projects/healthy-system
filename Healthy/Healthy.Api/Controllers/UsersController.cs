using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Healthy.Application.Users.Commands.CreateUser;
using Healthy.Application.Users.Queries.GetUsers;
using Healthy.Application.Common.Models;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("User management operations")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="command">User creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created user</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new user",
        Description = "Creates a new user with the provided information",
        OperationId = "CreateUser",
        Tags = new[] { "Users" }
    )]
    [SwaggerResponse(201, "User created successfully", typeof(Guid))]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(409, "User with email already exists")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<Guid>> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("already exists") == true)
                return Conflict(result.Error);
            
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetUsers), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Gets a paginated list of users
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get users",
        Description = "Retrieves a paginated list of users with optional filtering and sorting",
        OperationId = "GetUsers",
        Tags = new[] { "Users" }
    )]
    [SwaggerResponse(200, "Users retrieved successfully", typeof(PaginatedList<UserDto>))]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetUsers(
        [FromQuery] GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get user by ID",
        Description = "Retrieves a specific user by their unique identifier",
        OperationId = "GetUserById",
        Tags = new[] { "Users" }
    )]
    [SwaggerResponse(200, "User found", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<UserDto>> GetUserById(
        Guid id,
        CancellationToken cancellationToken)
    {
        // TODO: Implement GetUserByIdQuery
        return NotFound("GetUserById not implemented yet");
    }

    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update user",
        Description = "Updates an existing user with the provided information",
        OperationId = "UpdateUser",
        Tags = new[] { "Users" }
    )]
    [SwaggerResponse(200, "User updated successfully")]
    [SwaggerResponse(400, "Invalid input data")]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult> UpdateUser(
        Guid id,
        [FromBody] object command, // TODO: Replace with UpdateUserCommand
        CancellationToken cancellationToken)
    {
        // TODO: Implement UpdateUserCommand
        return NotFound("UpdateUser not implemented yet");
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Delete user",
        Description = "Soft deletes a user by marking them as deleted",
        OperationId = "DeleteUser",
        Tags = new[] { "Users" }
    )]
    [SwaggerResponse(204, "User deleted successfully")]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult> DeleteUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        // TODO: Implement DeleteUserCommand
        return NotFound("DeleteUser not implemented yet");
    }
} 