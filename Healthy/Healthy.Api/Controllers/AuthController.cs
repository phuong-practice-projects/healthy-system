using Microsoft.AspNetCore.Mvc;
using MediatR;
using Healthy.Application.Common.Models;
using Swashbuckle.AspNetCore.Annotations;
using Healthy.Application.UseCases.Users.Commands.Login;
using Healthy.Application.UseCases.Users.Commands.Register;

namespace Healthy.Api.Controllers;

[Route("api/[controller]")]
[SwaggerTag("Authentication endpoints for user login and registration")]
public class AuthController(IMediator mediator) : BaseController
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication result with JWT token</returns>
    /// <response code="200">Login successful - returns JWT token and user information</response>
    /// <response code="400">Invalid request data - validation errors</response>
    /// <response code="401">Invalid credentials - email or password incorrect</response>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "User Login",
        Description = "Authenticates a user with email and password, returns JWT token on success",
        OperationId = "Login",
        Tags = new[] { "Auth" }
    )]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(200, "Authentication successful", typeof(AuthResult))]
    [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "Invalid credentials", typeof(AuthResult))]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <returns>Authentication result with JWT token</returns>
    /// <response code="201">Registration successful - user created and JWT token returned</response>
    /// <response code="400">Invalid request data - validation errors</response>
    /// <response code="409">Email already exists - conflict error</response>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "User Registration", 
        Description = "Creates a new user account and returns JWT token on success",
        OperationId = "Register",
        Tags = new[] { "Auth" }
    )]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status409Conflict)]
    [SwaggerResponse(201, "Registration successful", typeof(AuthResult))]
    [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
    [SwaggerResponse(409, "Email already exists", typeof(AuthResult))]
    public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new RegisterCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender
        };

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            // Check if it's a conflict (email already exists)
            if (result.Error?.Contains("already exists") == true)
            {
                return Conflict(result);
            }
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(Login), new { }, result);
    }
} 