using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Exercises.Commands.CreateExercise;
using Healthy.Application.UseCases.Exercises.Commands.DeleteExercise;
using Healthy.Application.UseCases.Exercises.Commands.UpdateExercise;
using Healthy.Application.UseCases.Exercises.Queries.GetExercise;
using Healthy.Application.UseCases.Exercises.Queries.GetExercises;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class ExercisesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly ExercisesController _controller;

    public ExercisesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new ExercisesController(_mediatorMock.Object);
        
        // Setup default current user
        var testUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(testUserId.ToString());
        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(true);
        
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
    public async Task GetExercises_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var exerciseListResponse = new ExerciseListResponse
        {
            Items = new List<ExerciseDto>(),
            TotalItems = 0,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetExercisesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(exerciseListResponse);

        // Act
        var result = await _controller.GetExercises();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ExerciseListResponse;
        response.Should().NotBeNull();
        response!.Items.Should().NotBeNull();
        response.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetExercises_WithCustomParameters_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today;
        var category = "Cardio";
        var searchTerm = "running";
        var minDuration = 30;
        var maxDuration = 60;
        var minCalories = 100;
        var maxCalories = 500;
        var sortBy = "Duration";
        var sortDirection = "Asc";
        var page = 2;
        var pageSize = 5;

        var exerciseListResponse = new ExerciseListResponse
        {
            Items = new List<ExerciseDto>(),
            TotalItems = 0,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetExercisesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(exerciseListResponse);

        // Act
        await _controller.GetExercises(startDate, endDate, category, searchTerm, minDuration, maxDuration, minCalories, maxCalories, sortBy, sortDirection, page, pageSize);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetExercisesQuery>(q => 
                    q.StartDate == startDate &&
                    q.EndDate == endDate &&
                    q.Category == category &&
                    q.SearchTerm == searchTerm &&
                    q.MinDuration == minDuration &&
                    q.MaxDuration == maxDuration &&
                    q.MinCalories == minCalories &&
                    q.MaxCalories == maxCalories &&
                    q.SortBy == sortBy &&
                    q.SortDirection == sortDirection &&
                    q.Page == page &&
                    q.PageSize == pageSize),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetExercises_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var result = await _controller.GetExercises();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetExercises_WithInvalidGuidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns("invalid-guid");

        // Act
        var result = await _controller.GetExercises();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetExercises_WithValidExercises_ShouldReturnCorrectResponse()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var exerciseDto = new ExerciseDto
        {
            Id = exerciseId,
            UserId = Guid.NewGuid(),
            Title = "Morning Run",
            Description = "5km morning run",
            Category = "Cardio",
            DurationMinutes = 45,
            CaloriesBurned = 300,
            ExerciseDate = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };

        var exerciseListResponse = new ExerciseListResponse
        {
            Items = new List<ExerciseDto> { exerciseDto },
            TotalItems = 1,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetExercisesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(exerciseListResponse);

        // Act
        var result = await _controller.GetExercises();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as ExerciseListResponse;
        response.Should().NotBeNull();
        
        response!.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        
        var firstExercise = response.Items.First();
        firstExercise.Id.Should().Be(exerciseId);
        firstExercise.Title.Should().Be("Morning Run");
        firstExercise.Description.Should().Be("5km morning run");
        firstExercise.Category.Should().Be("Cardio");
        firstExercise.DurationMinutes.Should().Be(45);
        firstExercise.CaloriesBurned.Should().Be(300);
        firstExercise.ExerciseDate.Should().Be(DateTime.Today);

        response.TotalItems.Should().Be(1);
        response.CurrentPage.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetExercise_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var exerciseDto = new ExerciseDto
        {
            Id = exerciseId,
            UserId = Guid.NewGuid(),
            Title = "Morning Run",
            Description = "5km morning run",
            Category = "Cardio",
            DurationMinutes = 45,
            CaloriesBurned = 300,
            ExerciseDate = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };

        var result = Result<ExerciseDto>.Success(exerciseDto);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetExerciseQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetExercise(exerciseId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetExercise_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var result = Result<ExerciseDto>.Failure("Exercise not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetExerciseQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetExercise(exerciseId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetExercise_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var exerciseId = Guid.NewGuid();

        // Act
        var response = await _controller.GetExercise(exerciseId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task CreateExercise_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var request = new CreateExerciseRequest
        {
            Title = "Morning Run",
            Description = "5km morning run",
            Category = "Cardio",
            DurationMinutes = 45,
            CaloriesBurned = 300,
            ExerciseDate = DateTime.Today
        };

        var result = Result<Guid>.Success(exerciseId);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateExerciseCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateExercise(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(ExercisesController.GetExercise));
        createdResult.RouteValues!["id"].Should().Be(exerciseId);
        createdResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateExercise_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateExerciseRequest
        {
            Title = "", // Invalid empty title
            Description = "5km morning run",
            Category = "Cardio",
            DurationMinutes = 45,
            CaloriesBurned = 300,
            ExerciseDate = DateTime.Today
        };

        var result = Result<Guid>.Failure("Title is required");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateExerciseCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateExercise(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateExercise_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var request = new CreateExerciseRequest
        {
            Title = "Morning Run",
            Description = "5km morning run",
            Category = "Cardio",
            DurationMinutes = 45,
            CaloriesBurned = 300,
            ExerciseDate = DateTime.Today
        };

        // Act
        var response = await _controller.CreateExercise(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateExercise_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var request = new UpdateExerciseRequest
        {
            Title = "Updated Morning Run",
            Description = "Updated 5km morning run",
            Category = "Cardio",
            DurationMinutes = 50,
            CaloriesBurned = 350,
            ExerciseDate = DateTime.Today
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateExerciseCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateExercise(exerciseId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateExercise_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var request = new UpdateExerciseRequest
        {
            Title = "Updated Morning Run",
            Description = "Updated 5km morning run",
            Category = "Cardio",
            DurationMinutes = 50,
            CaloriesBurned = 350,
            ExerciseDate = DateTime.Today
        };

        var result = Result<bool>.Failure("Exercise not found");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateExerciseCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateExercise(exerciseId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateExercise_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var exerciseId = Guid.NewGuid();
        var request = new UpdateExerciseRequest
        {
            Title = "Updated Morning Run",
            Description = "Updated 5km morning run",
            Category = "Cardio",
            DurationMinutes = 50,
            CaloriesBurned = 350,
            ExerciseDate = DateTime.Today
        };

        // Act
        var response = await _controller.UpdateExercise(exerciseId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task DeleteExercise_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteExerciseCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteExercise(exerciseId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteExercise_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var result = Result<bool>.Failure("Exercise not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteExerciseCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteExercise(exerciseId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteExercise_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var exerciseId = Guid.NewGuid();

        // Act
        var response = await _controller.DeleteExercise(exerciseId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }
} 