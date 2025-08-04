using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Users.Commands.DeleteUser;
using Healthy.Application.UseCases.Users.Commands.UpdateUser;
using Healthy.Application.UseCases.Users.Queries.GetUser;
using Healthy.Application.UseCases.Users.Queries.GetUsers;
using Healthy.Application.UseCases.Users.Queries.GetUsersWithFilters;
using Healthy.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AdminOnly]

public class UsersController(IMediator mediator) : BaseController
{

    /// <summary>
    /// Get users with filters and pagination (requires Admin role)
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for name or email</param>
    /// <param name="role">Filter by role</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>List of users with pagination</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UsersListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UsersListResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? role = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetUsersWithFiltersQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Role = role,
            IsActive = isActive
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all users without pagination (requires Admin role)
    /// </summary>
    /// <returns>Simple list of users</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var query = new GetUsersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific user by ID (requires Admin role)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<UserDto>>> GetUser(Guid id)
    {
        var query = new GetUserQuery { Id = id };
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="request">Updated user data</param>
    /// <returns>Update result</returns>
    [HttpPut("me")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> UpdateCurrentUser([FromBody] UpdateUserRequest request)
    {
        var userIdString = GetCurrentUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var command = new UpdateUserCommand
        {
            Id = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender
        };

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Update a specific user (requires Admin role)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender
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
    /// Delete a user (requires Admin role)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand { Id = id };
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
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize(Roles = "User,Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserDto> GetCurrentUser()
    {
        // Use CurrentUser service instead of accessing User claims directly
        if (!IsAuthenticated())
        {
            return Unauthorized();
        }

        var currentUserInfo = CurrentUser.GetCurrentUserInfo();
        
        var userDto = new UserDto
        {
            Id = Guid.TryParse(currentUserInfo.UserId, out var parsedId) ? parsedId : Guid.Empty,
            Email = currentUserInfo.Email ?? string.Empty,
            FirstName = currentUserInfo.GetClaimValue(System.Security.Claims.ClaimTypes.GivenName) ?? string.Empty,
            LastName = currentUserInfo.GetClaimValue(System.Security.Claims.ClaimTypes.Surname) ?? string.Empty,
            FullName = currentUserInfo.FullName ?? $"{currentUserInfo.GetClaimValue(System.Security.Claims.ClaimTypes.GivenName)} {currentUserInfo.GetClaimValue(System.Security.Claims.ClaimTypes.Surname)}".Trim(),
            Roles = currentUserInfo.Roles
        };

        return Ok(userDto);
    }
} 