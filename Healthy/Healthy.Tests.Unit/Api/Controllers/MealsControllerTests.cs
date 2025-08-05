using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Meals.Commands.CreateMeal;
using Healthy.Application.UseCases.Meals.Commands.DeleteMeal;
using Healthy.Application.UseCases.Meals.Commands.UpdateMeal;
using Healthy.Application.UseCases.Meals.Queries.GetMeals;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class MealsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly MealsController _controller;

    public MealsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new MealsController(_mediatorMock.Object);
        
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
    public async Task GetMeals_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var mealListResponse = new MealListResponse
        {
            Items = new List<MealDto>(),
            TotalItems = 0,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetMealsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mealListResponse);

        // Act
        var result = await _controller.GetMeals();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as MealListResponse;
        response.Should().NotBeNull();
        response!.Items.Should().NotBeNull();
        response.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetMeals_WithCustomParameters_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today;
        var type = "Morning";
        var searchTerm = "breakfast";
        var minCalories = 100;
        var maxCalories = 500;
        var sortBy = "Calories";
        var sortDirection = "Asc";
        var page = 2;
        var pageSize = 5;

        var mealListResponse = new MealListResponse
        {
            Items = new List<MealDto>(),
            TotalItems = 0,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetMealsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mealListResponse);

        // Act
        await _controller.GetMeals(startDate, endDate, type, searchTerm, minCalories, maxCalories, sortBy, sortDirection, page, pageSize);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetMealsQuery>(q => 
                    q.StartDate == startDate &&
                    q.EndDate == endDate &&
                    q.Type == type &&
                    q.SearchTerm == searchTerm &&
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
    public async Task GetMeals_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var result = await _controller.GetMeals();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetMeals_WithInvalidGuidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns("invalid-guid");

        // Act
        var result = await _controller.GetMeals();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetMeals_WithValidMeals_ShouldReturnCorrectResponse()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        var mealDto = new MealDto
        {
            Id = mealId,
            UserId = Guid.NewGuid(),
            Type = "Morning",
            ImageUrl = "https://example.com/meal.jpg",
            Date = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };

        var mealListResponse = new MealListResponse
        {
            Items = new List<MealDto> { mealDto },
            TotalItems = 1,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetMealsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mealListResponse);

        // Act
        var result = await _controller.GetMeals();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as MealListResponse;
        response.Should().NotBeNull();
        
        response!.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        
        var firstMeal = response.Items.First();
        firstMeal.Id.Should().Be(mealId);
        firstMeal.Type.Should().Be("Morning");
        firstMeal.ImageUrl.Should().Be("https://example.com/meal.jpg");
        firstMeal.Date.Should().Be(DateTime.Today);

        response.TotalItems.Should().Be(1);
        response.CurrentPage.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task CreateMeal_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        var request = new CreateMealRequest
        {
            Type = "Morning",
            ImageUrl = "https://example.com/meal.jpg",
            Date = DateTime.Today
        };

        var result = Result<Guid>.Success(mealId);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMealCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateMeal(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(MealsController.GetMeals));
        createdResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateMeal_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateMealRequest
        {
            Type = "", // Invalid empty type
            ImageUrl = "https://example.com/meal.jpg",
            Date = DateTime.Today
        };

        var result = Result<Guid>.Failure("Meal type is required");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMealCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateMeal(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateMeal_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var request = new CreateMealRequest
        {
            Type = "Morning",
            ImageUrl = "https://example.com/meal.jpg",
            Date = DateTime.Today
        };

        // Act
        var response = await _controller.CreateMeal(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateMeal_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        var request = new UpdateMealRequest
        {
            Type = "Lunch",
            ImageUrl = "https://example.com/updated-meal.jpg",
            Date = DateTime.Today
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMealCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateMeal(mealId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateMeal_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        var request = new UpdateMealRequest
        {
            Type = "Lunch",
            ImageUrl = "https://example.com/updated-meal.jpg",
            Date = DateTime.Today
        };

        var result = Result<bool>.Failure("Meal not found");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMealCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateMeal(mealId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateMeal_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var mealId = Guid.NewGuid();
        var request = new UpdateMealRequest
        {
            Type = "Lunch",
            ImageUrl = "https://example.com/updated-meal.jpg",
            Date = DateTime.Today
        };

        // Act
        var response = await _controller.UpdateMeal(mealId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task DeleteMeal_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMealCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteMeal(mealId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteMeal_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        var result = Result<bool>.Failure("Meal not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMealCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteMeal(mealId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteMeal_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var mealId = Guid.NewGuid();

        // Act
        var response = await _controller.DeleteMeal(mealId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }
} 