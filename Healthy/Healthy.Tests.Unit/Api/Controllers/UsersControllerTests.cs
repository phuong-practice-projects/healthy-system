using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Users.Commands.DeleteUser;
using Healthy.Application.UseCases.Users.Commands.UpdateUser;
using Healthy.Application.UseCases.Users.Queries.GetUser;
using Healthy.Application.UseCases.Users.Queries.GetUsers;
using Healthy.Application.UseCases.Users.Queries.GetUsersWithFilters;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new UsersController(_mediatorMock.Object);
        
        // Setup default current user
        var testUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(testUserId.ToString());
        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(true);
        _currentUserServiceMock.Setup(x => x.HasRole("Admin")).Returns(true);
        
        // Mock the HttpContext and RequestServices
        var httpContext = new DefaultHttpContext();
        var serviceProvider = new TestServiceProvider(_currentUserServiceMock.Object);
        httpContext.RequestServices = serviceProvider;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetUsers_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var usersListResponse = new UsersListResponse
        {
            Items = new List<UserDto>(),
            TotalItems = 0,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersWithFiltersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usersListResponse);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as UsersListResponse;
        response.Should().NotBeNull();
        response!.Items.Should().NotBeNull();
        response.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetUsers_WithCustomParameters_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var page = 2;
        var pageSize = 5;
        var searchTerm = "john";
        var role = "User";
        var isActive = true;

        var usersListResponse = new UsersListResponse
        {
            Items = new List<UserDto>(),
            TotalItems = 0,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersWithFiltersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usersListResponse);

        // Act
        await _controller.GetUsers(page, pageSize, searchTerm, role, isActive);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetUsersWithFiltersQuery>(q => 
                    q.Page == page &&
                    q.PageSize == pageSize &&
                    q.SearchTerm == searchTerm &&
                    q.Role == role &&
                    q.IsActive == isActive),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUsers_WithValidUsers_ShouldReturnCorrectResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var usersListResponse = new UsersListResponse
        {
            Items = new List<UserDto> { userDto },
            TotalItems = 1,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersWithFiltersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usersListResponse);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as UsersListResponse;
        response.Should().NotBeNull();
        
        response!.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        
        var firstUser = response.Items.First();
        firstUser.Id.Should().Be(userId);
        firstUser.FirstName.Should().Be("John");
        firstUser.LastName.Should().Be("Doe");
        firstUser.Email.Should().Be("john.doe@example.com");
        firstUser.IsActive.Should().BeTrue();

        response.TotalItems.Should().Be(1);
        response.CurrentPage.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOkResult()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new UserDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as List<UserDto>;
        response.Should().NotBeNull();
        response!.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUser_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = Result<UserDto>.Success(userDto);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetUser(userId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var result = Result<UserDto>.Failure("User not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetUser(userId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateCurrentUser_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateCurrentUser(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateCurrentUser_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            FirstName = "", // Invalid empty first name
            LastName = "Name",
            PhoneNumber = "+1234567890"
        };

        var result = Result<bool>.Failure("First name is required");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateCurrentUser(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateUser_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateUser(userId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateUser_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Gender = "Male"
        };

        var result = Result<bool>.Failure("User not found");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateUser(userId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteUser(userId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteUser_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var result = Result<bool>.Failure("User not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteUser(userId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void GetCurrentUser_ShouldReturnOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserInfo = new CurrentUserInfo
        {
            UserId = userId.ToString(),
            UserName = "currentuser",
            Email = "current@example.com",
            FullName = "Current User",
            Roles = new List<string> { "User" },
            Claims = new Dictionary<string, string>
            {
                { System.Security.Claims.ClaimTypes.GivenName, "Current" },
                { System.Security.Claims.ClaimTypes.Surname, "User" }
            },
            IsAuthenticated = true
        };

        _currentUserServiceMock.Setup(x => x.GetCurrentUserInfo()).Returns(currentUserInfo);

        // Act
        var result = _controller.GetCurrentUser();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public void GetCurrentUser_WithNullUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        var currentUserInfo = new CurrentUserInfo
        {
            UserId = null,
            UserName = null,
            Email = null,
            FullName = null,
            Roles = new List<string>(),
            Claims = new Dictionary<string, string>(),
            IsAuthenticated = false
        };

        _currentUserServiceMock.Setup(x => x.GetCurrentUserInfo()).Returns(currentUserInfo);
        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var result = _controller.GetCurrentUser();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }
} 