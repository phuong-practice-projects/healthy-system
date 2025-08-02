using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Healthy.Application.Users.Queries.GetUsers;
using Healthy.Application.Common.Models;

namespace Healthy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all users (requires authentication)
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")] // Only Admin role can access
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var query = new GetUsersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        // Get user from JWT token claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var firstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
        var lastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

        if (userId == null)
        {
            return Unauthorized();
        }

        var userDto = new UserDto
        {
            Id = Guid.Parse(userId),
            Email = email ?? string.Empty,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            FullName = $"{firstName} {lastName}".Trim(),
            Roles = roles
        };

        return Ok(userDto);
    }
} 