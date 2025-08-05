using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecordGraph;
using Healthy.Application.UseCases.Dashboard.Queries.GetDashboardAchievement;
using Healthy.Application.UseCases.Dashboard.Queries.GetDashboardSummary;
using Healthy.Application.UseCases.Meals.Queries.GetMealHistory;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new DashboardController(_mediatorMock.Object);
        
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
    public async Task GetTodayMeals_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var mealHistoryResponse = new MealHistoryResponse
        {
            Meals = new List<MealDto>(),
            Statistics = new MealStatisticsDto
            {
                TotalMeals = 0,
                MealTypeCount = new Dictionary<string, int>(),
                CompletionPercentage = 0
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetMealHistoryQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mealHistoryResponse);

        // Act
        var response = await _controller.GetTodayMeals();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = response.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var responseValue = okResult.Value as MealHistoryResponse;
        responseValue.Should().NotBeNull();
        responseValue!.Meals.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTodayMeals_WithTypeFilter_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var type = "Morning";
        var mealHistoryResponse = new MealHistoryResponse
        {
            Meals = new List<MealDto>(),
            Statistics = new MealStatisticsDto
            {
                TotalMeals = 0,
                MealTypeCount = new Dictionary<string, int>(),
                CompletionPercentage = 0
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetMealHistoryQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(mealHistoryResponse);

        // Act
        await _controller.GetTodayMeals(type);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetMealHistoryQuery>(q => 
                    q.Date == DateTime.Today &&
                    q.Type == type),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTodayMeals_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var response = await _controller.GetTodayMeals();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetTodayMeals_WithInvalidGuidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns("invalid-guid");

        // Act
        var response = await _controller.GetTodayMeals();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetAchievements_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var completionRateResponse = new CompletionRateResponse
        {
            Rate = 71.7m,
            Message = "Good progress!",
            Breakdown = new GoalBreakdownDto
            {
                CompletedGoals = 3,
                TotalGoals = 4,
                Categories = new List<GoalCategoryStatusDto>
                {
                    new GoalCategoryStatusDto { Category = "Meals", Completed = 3, Total = 4 },
                    new GoalCategoryStatusDto { Category = "Exercises", Completed = 2, Total = 3 },
                    new GoalCategoryStatusDto { Category = "Body Records", Completed = 1, Total = 1 }
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDashboardAchievementQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(completionRateResponse);

        // Act
        var response = await _controller.GetAchievements();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = response.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var responseValue = okResult.Value as CompletionRateResponse;
        responseValue.Should().NotBeNull();
        responseValue!.Rate.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetAchievements_WithCustomDate_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var customDate = DateTime.Today.AddDays(-7);
        var completionRateResponse = new CompletionRateResponse
        {
            Rate = 71.7m,
            Message = "Good progress!",
            Breakdown = new GoalBreakdownDto
            {
                CompletedGoals = 3,
                TotalGoals = 4,
                Categories = new List<GoalCategoryStatusDto>
                {
                    new GoalCategoryStatusDto { Category = "Meals", Completed = 3, Total = 4 },
                    new GoalCategoryStatusDto { Category = "Exercises", Completed = 2, Total = 3 },
                    new GoalCategoryStatusDto { Category = "Body Records", Completed = 1, Total = 1 }
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDashboardAchievementQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(completionRateResponse);

        // Act
        await _controller.GetAchievements(customDate);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetDashboardAchievementQuery>(q => 
                    q.Date == customDate),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAchievements_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var response = await _controller.GetAchievements();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetUserSummary_ShouldReturnOkResult()
    {
        // Arrange
        var dashboardSummaryDto = new DashboardSummaryDto
        {
            CurrentStreak = 7,
            BestStreak = 15,
            TotalActiveDays = 30,
            CurrentWeight = 70.5m,
            WeightChange = -2.5m
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDashboardSummaryQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(dashboardSummaryDto);

        // Act
        var response = await _controller.GetUserSummary();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = response.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var responseValue = okResult.Value as DashboardSummaryDto;
        responseValue.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserSummary_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var response = await _controller.GetUserSummary();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetWeightChart_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var bodyRecordGraphResponse = new BodyRecordGraphResponse
        {
            GraphData = new List<BodyRecordGraphData>
            {
                new BodyRecordGraphData
                {
                    Date = DateTime.Today.AddDays(-1),
                    Weight = 70.0m,
                    BodyFat = 15.0m
                },
                new BodyRecordGraphData
                {
                    Date = DateTime.Today,
                    Weight = 70.5m,
                    BodyFat = 14.8m
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordGraphQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bodyRecordGraphResponse);

        // Act
        var response = await _controller.GetWeightChart();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = response.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var responseValue = okResult.Value as BodyRecordGraphResponse;
        responseValue.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWeightChart_WithCustomMonths_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var months = 6;
        var bodyRecordGraphResponse = new BodyRecordGraphResponse
        {
            GraphData = new List<BodyRecordGraphData>
            {
                new BodyRecordGraphData
                {
                    Date = DateTime.Today.AddDays(-1),
                    Weight = 70.0m,
                    BodyFat = 15.0m
                },
                new BodyRecordGraphData
                {
                    Date = DateTime.Today,
                    Weight = 70.5m,
                    BodyFat = 14.8m
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordGraphQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bodyRecordGraphResponse);

        // Act
        await _controller.GetWeightChart(months);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetBodyRecordGraphQuery>(q => 
                    q.StartDate == DateTime.Today.AddMonths(-months) &&
                    q.EndDate == DateTime.Today),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 1)] // Should clamp to minimum 1
    [InlineData(-1, 1)] // Should clamp to minimum 1
    [InlineData(15, 12)] // Should clamp to maximum 12
    [InlineData(20, 12)] // Should clamp to maximum 12
    public async Task GetWeightChart_WithInvalidMonths_ShouldClampToValidRange(int inputMonths, int expectedMonths)
    {
        // Arrange
        var bodyRecordGraphResponse = new BodyRecordGraphResponse
        {
            GraphData = new List<BodyRecordGraphData>()
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordGraphQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bodyRecordGraphResponse);

        // Act
        await _controller.GetWeightChart(inputMonths);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetBodyRecordGraphQuery>(q => 
                    q.StartDate == DateTime.Today.AddMonths(-expectedMonths) &&
                    q.EndDate == DateTime.Today),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWeightChart_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var response = await _controller.GetWeightChart();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetWeightChart_WithInvalidGuidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns("invalid-guid");

        // Act
        var response = await _controller.GetWeightChart();

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }
} 