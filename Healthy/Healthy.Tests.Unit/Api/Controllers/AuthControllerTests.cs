using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Users.Commands.Login;
using Healthy.Application.UseCases.Users.Commands.Register;

namespace Healthy.Tests.Unit.Api.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AuthController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkResult()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var authResult = new AuthResult
        {
            Succeeded = true,
            Token = "jwt-token-here",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as AuthResult;
        response.Should().NotBeNull();
        response!.Succeeded.Should().BeTrue();
        response.Token.Should().Be("jwt-token-here");
        response.User.Should().NotBeNull();
        response.User!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var authResult = new AuthResult
        {
            Succeeded = false,
            Error = "Invalid email or password"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);

        var response = unauthorizedResult.Value as AuthResult;
        response.Should().NotBeNull();
        response!.Succeeded.Should().BeFalse();
        response.Error.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task Login_WithInvalidModelState_ShouldReturnValidationProblem()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "", // Invalid empty email
            Password = "password123"
        };

        // Add model error to simulate validation failure
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<ObjectResult>();
        
        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        
        var validationResult = objectResult.Value as ValidationProblemDetails;
        validationResult.Should().NotBeNull();
        validationResult!.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Login_ShouldPassCorrectCommandToMediator()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var authResult = new AuthResult
        {
            Succeeded = true,
            Token = "jwt-token-here",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        await _controller.Login(request);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<LoginCommand>(c => 
                    c.Email == request.Email &&
                    c.Password == request.Password),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var authResult = new AuthResult
        {
            Succeeded = true,
            Token = "jwt-token-here",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(AuthController.Login));

        var response = createdResult.Value as AuthResult;
        response.Should().NotBeNull();
        response!.Succeeded.Should().BeTrue();
        response.Token.Should().Be("jwt-token-here");
    }

    [Fact]
    public async Task Register_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            Password = "password123",
            ConfirmPassword = "password123",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var authResult = new AuthResult
        {
            Succeeded = false,
            Error = "Invalid email format"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);

        var response = badRequestResult.Value as AuthResult;
        response.Should().NotBeNull();
        response!.Succeeded.Should().BeFalse();
        response.Error.Should().Be("Invalid email format");
    }

    [Fact]
    public async Task Register_WithEmailAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var authResult = new AuthResult
        {
            Succeeded = false,
            Error = "Email already exists"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<ConflictObjectResult>();
        
        var conflictResult = result.Result as ConflictObjectResult;
        conflictResult.Should().NotBeNull();
        conflictResult!.StatusCode.Should().Be(409);

        var response = conflictResult.Value as AuthResult;
        response.Should().NotBeNull();
        response!.Succeeded.Should().BeFalse();
        response.Error.Should().Be("Email already exists");
    }

    [Fact]
    public async Task Register_WithInvalidModelState_ShouldReturnValidationProblem()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "", // Invalid empty first name
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        // Add model error to simulate validation failure
        _controller.ModelState.AddModelError("FirstName", "First name is required");

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<ObjectResult>();
        
        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        
        var validationResult = objectResult.Value as ValidationProblemDetails;
        validationResult.Should().NotBeNull();
        validationResult!.Errors.Should().ContainKey("FirstName");
    }

    [Fact]
    public async Task Register_ShouldPassCorrectCommandToMediator()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var authResult = new AuthResult
        {
            Succeeded = true,
            Token = "jwt-token-here",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(authResult);

        // Act
        await _controller.Register(request);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<RegisterCommand>(c => 
                    c.FirstName == request.FirstName &&
                    c.LastName == request.LastName &&
                    c.Email == request.Email &&
                    c.Password == request.Password &&
                    c.ConfirmPassword == request.ConfirmPassword &&
                    c.PhoneNumber == request.PhoneNumber &&
                    c.DateOfBirth == request.DateOfBirth &&
                    c.Gender == request.Gender),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
} 