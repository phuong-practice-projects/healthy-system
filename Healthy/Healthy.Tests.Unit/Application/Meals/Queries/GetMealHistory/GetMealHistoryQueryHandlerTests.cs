using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Healthy.Application.UseCases.Meals.Queries.GetMealHistory;
using Healthy.Application.Common.Interfaces;
using Healthy.Domain.Entities;
using Healthy.Tests.Unit.Common;

namespace Healthy.Tests.Unit.Application.Meals.Queries.GetMealHistory;

public class GetMealHistoryQueryHandlerTests
{
    private readonly TestDbContext _context;
    private readonly GetMealHistoryQueryHandler _handler;
    private readonly Mock<ILogger<GetMealHistoryQueryHandler>> _loggerMock;
    private readonly Guid _testUserId;

    public GetMealHistoryQueryHandlerTests()
    {
        _context = (TestDbContext)TestHelper.GetInMemoryDbContext();
        _loggerMock = new Mock<ILogger<GetMealHistoryQueryHandler>>();
        _handler = new GetMealHistoryQueryHandler(_context, _loggerMock.Object);
        _testUserId = Guid.NewGuid();
        
        // Seed test user
        var testUser = new User
        {
            Id = _testUserId,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(testUser);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_NoMealsForDate_ShouldReturnEmptyListWithZeroStatistics()
    {
        // Arrange
        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.Statistics.TotalMeals.Should().Be(0);
        result.Statistics.CompletionPercentage.Should().Be(0);
        result.Statistics.MealTypeCount.Should().HaveCount(4); // All meal types should be present
        result.Statistics.MealTypeCount["Morning"].Should().Be(0);
        result.Statistics.MealTypeCount["Lunch"].Should().Be(0);
        result.Statistics.MealTypeCount["Dinner"].Should().Be(0);
        result.Statistics.MealTypeCount["Snack"].Should().Be(0);
    }

    [Fact]
    public async Task Handle_MealsForDate_ShouldReturnOrderedMealsWithStatistics()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = today,
                CreatedAt = today.AddHours(7)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch.jpg",
                Type = "Lunch",
                Date = today,
                CreatedAt = today.AddHours(12)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/dinner.jpg",
                Type = "Dinner",
                Date = today,
                CreatedAt = today.AddHours(19)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(3);
        result.Statistics.TotalMeals.Should().Be(3);
        result.Statistics.CompletionPercentage.Should().Be(100); // All 3 main meals completed
        result.Statistics.MealTypeCount["Morning"].Should().Be(1);
        result.Statistics.MealTypeCount["Lunch"].Should().Be(1);
        result.Statistics.MealTypeCount["Dinner"].Should().Be(1);
        result.Statistics.MealTypeCount["Snack"].Should().Be(0);

        // Verify meals are ordered by type then by creation time
        result.Meals[0].Type.Should().Be("Morning");
        result.Meals[1].Type.Should().Be("Lunch");
        result.Meals[2].Type.Should().Be("Dinner");
    }

    [Fact]
    public async Task Handle_MealsWithTypeFilter_ShouldReturnFilteredMeals()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = today,
                CreatedAt = today.AddHours(7)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch.jpg",
                Type = "Lunch",
                Date = today,
                CreatedAt = today.AddHours(12)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/dinner.jpg",
                Type = "Dinner",
                Date = today,
                CreatedAt = today.AddHours(19)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today,
            Type = "Lunch"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(1);
        result.Meals[0].Type.Should().Be("Lunch");
        result.Statistics.TotalMeals.Should().Be(3); // Statistics should include all meals for the day
        result.Statistics.CompletionPercentage.Should().Be(100);
    }

    [Fact]
    public async Task Handle_MealsWithPartialCompletion_ShouldCalculateCorrectStatistics()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = today,
                CreatedAt = today.AddHours(7)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/snack.jpg",
                Type = "Snack",
                Date = today,
                CreatedAt = today.AddHours(10)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(2);
        result.Statistics.TotalMeals.Should().Be(2);
        result.Statistics.CompletionPercentage.Should().Be(33.3m); // 1 out of 3 main meals (33.33%)
        result.Statistics.MealTypeCount["Morning"].Should().Be(1);
        result.Statistics.MealTypeCount["Lunch"].Should().Be(0);
        result.Statistics.MealTypeCount["Dinner"].Should().Be(0);
        result.Statistics.MealTypeCount["Snack"].Should().Be(1);
    }

    [Fact]
    public async Task Handle_MealsWithSameType_ShouldOrderByCreationTime()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch1.jpg",
                Type = "Lunch",
                Date = today,
                CreatedAt = today.AddHours(12)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch2.jpg",
                Type = "Lunch",
                Date = today,
                CreatedAt = today.AddHours(13)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = today,
                CreatedAt = today.AddHours(7)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(3);
        
        // Verify order: Morning first, then Lunch meals by creation time (newest first)
        result.Meals[0].Type.Should().Be("Morning");
        result.Meals[1].Type.Should().Be("Lunch");
        result.Meals[1].CreatedAt.Should().Be(today.AddHours(13));
        result.Meals[2].Type.Should().Be("Lunch");
        result.Meals[2].CreatedAt.Should().Be(today.AddHours(12));
    }

    [Fact]
    public async Task Handle_DifferentDate_ShouldReturnEmptyList()
    {
        // Arrange
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);
        
        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = yesterday,
            CreatedAt = yesterday.AddHours(12)
        };

        _context.Meals.Add(meal);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.Statistics.TotalMeals.Should().Be(0);
    }

    [Fact]
    public async Task Handle_NoDateSpecified_ShouldUseToday()
    {
        // Arrange
        var today = DateTime.Today;
        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = today,
            CreatedAt = today.AddHours(12)
        };

        _context.Meals.Add(meal);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId
            // Date not specified, should default to today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(1);
        result.Meals[0].Type.Should().Be("Lunch");
        result.Statistics.TotalMeals.Should().Be(1);
    }

    [Fact]
    public async Task Handle_MealsFromDifferentUsers_ShouldOnlyReturnCurrentUserMeals()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var today = DateTime.Today;
        
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal1.jpg",
                Type = "Lunch",
                Date = today,
                CreatedAt = today.AddHours(12)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                ImageUrl = "https://example.com/meal2.jpg",
                Type = "Dinner",
                Date = today,
                CreatedAt = today.AddHours(19)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(1);
        result.Meals[0].UserId.Should().Be(_testUserId);
        result.Statistics.TotalMeals.Should().Be(1);
    }

    [Fact]
    public async Task Handle_DeletedMeals_ShouldNotBeIncluded()
    {
        // Arrange
        var today = DateTime.Today;
        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = today,
            CreatedAt = today.AddHours(12)
        };

        _context.Meals.Add(meal);
        _context.SaveChanges();

        // Soft delete the meal
        meal.Delete();
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.Statistics.TotalMeals.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Exception_ShouldLogAndReturnEmptyResult()
    {
        // Arrange
        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = DateTime.Today
        };

        // Simulate database exception by disposing context
        _context.Dispose();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.Statistics.TotalMeals.Should().Be(0);
    }

    [Fact]
    public async Task Handle_AllMealTypes_ShouldCalculateCorrectCompletion()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = today,
                CreatedAt = today.AddHours(7)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch.jpg",
                Type = "Lunch",
                Date = today,
                CreatedAt = today.AddHours(12)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/dinner.jpg",
                Type = "Dinner",
                Date = today,
                CreatedAt = today.AddHours(19)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/snack1.jpg",
                Type = "Snack",
                Date = today,
                CreatedAt = today.AddHours(10)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/snack2.jpg",
                Type = "Snack",
                Date = today,
                CreatedAt = today.AddHours(15)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealHistoryQuery
        {
            UserId = _testUserId,
            Date = today
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(5);
        result.Statistics.TotalMeals.Should().Be(5);
        result.Statistics.CompletionPercentage.Should().Be(100); // All 3 main meals completed
        result.Statistics.MealTypeCount["Morning"].Should().Be(1);
        result.Statistics.MealTypeCount["Lunch"].Should().Be(1);
        result.Statistics.MealTypeCount["Dinner"].Should().Be(1);
        result.Statistics.MealTypeCount["Snack"].Should().Be(2);
    }
} 