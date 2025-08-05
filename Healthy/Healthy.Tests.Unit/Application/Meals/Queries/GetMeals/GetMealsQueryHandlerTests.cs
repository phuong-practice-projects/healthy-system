using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Healthy.Application.UseCases.Meals.Queries.GetMeals;
using Healthy.Application.Common.Interfaces;
using Healthy.Domain.Entities;
using Healthy.Tests.Unit.Common;

namespace Healthy.Tests.Unit.Application.Meals.Queries.GetMeals;

public class GetMealsQueryHandlerTests
{
    private readonly TestDbContext _context;
    private readonly GetMealsQueryHandler _handler;
    private readonly Guid _testUserId;

    public GetMealsQueryHandlerTests()
    {
        _context = (TestDbContext)TestHelper.GetInMemoryDbContext();
        _handler = new GetMealsQueryHandler(_context);
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
    public async Task Handle_NoMeals_ShouldReturnEmptyPaginatedList()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithMeals_ShouldReturnPaginatedList()
    {
        // Arrange
        var meals = CreateTestMeals(15); // Create 15 meals
        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(10); // First page with 10 items
        result.TotalItems.Should().Be(15);
        result.TotalPages.Should().Be(2); // 15 items / 10 per page = 2 pages
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_SecondPage_ShouldReturnCorrectItems()
    {
        // Arrange
        var meals = CreateTestMeals(15);
        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 2,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(5); // Second page with remaining 5 items
        result.TotalItems.Should().Be(15);
        result.TotalPages.Should().Be(2);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_DateRangeFilter_ShouldReturnFilteredMeals()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal1.jpg",
                Type = "Lunch",
                Date = today.AddDays(-1),
                CreatedAt = today.AddDays(-1)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal2.jpg",
                Type = "Dinner",
                Date = today,
                CreatedAt = today
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal3.jpg",
                Type = "Breakfast",
                Date = today.AddDays(1),
                CreatedAt = today.AddDays(1)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            StartDate = today,
            EndDate = today,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(1);
        result.Meals[0].Date.Should().Be(today);
        result.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task Handle_TypeFilter_ShouldReturnFilteredMeals()
    {
        // Arrange
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = DateTime.Today,
                CreatedAt = DateTime.Today
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch.jpg",
                Type = "Lunch",
                Date = DateTime.Today,
                CreatedAt = DateTime.Today
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/dinner.jpg",
                Type = "Dinner",
                Date = DateTime.Today,
                CreatedAt = DateTime.Today
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Type = "Lunch",
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(1);
        result.Meals[0].Type.Should().Be("Lunch");
        result.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task Handle_MealsFromDifferentUsers_ShouldOnlyReturnCurrentUserMeals()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal1.jpg",
                Type = "Lunch",
                Date = DateTime.Today,
                CreatedAt = DateTime.Today
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                ImageUrl = "https://example.com/meal2.jpg",
                Type = "Dinner",
                Date = DateTime.Today,
                CreatedAt = DateTime.Today
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(1);
        result.Meals[0].UserId.Should().Be(_testUserId);
        result.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task Handle_DeletedMeals_ShouldNotBeIncluded()
    {
        // Arrange
        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today,
            CreatedAt = DateTime.Today
        };

        _context.Meals.Add(meal);
        _context.SaveChanges();

        // Soft delete the meal
        meal.Delete();
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task Handle_OrderedByDateDesc_ShouldReturnCorrectOrder()
    {
        // Arrange
        var today = DateTime.Today;
        var meals = new List<Meal>
        {
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal1.jpg",
                Type = "Lunch",
                Date = today.AddDays(-2),
                CreatedAt = today.AddDays(-2)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal2.jpg",
                Type = "Dinner",
                Date = today.AddDays(-1),
                CreatedAt = today.AddDays(-1)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal3.jpg",
                Type = "Breakfast",
                Date = today,
                CreatedAt = today
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(3);
        
        // Verify order: newest first (today, yesterday, day before yesterday)
        result.Meals[0].Date.Should().Be(today);
        result.Meals[1].Date.Should().Be(today.AddDays(-1));
        result.Meals[2].Date.Should().Be(today.AddDays(-2));
    }

    [Fact]
    public async Task Handle_OrderedByCreationTime_ShouldReturnCorrectOrder()
    {
        // Arrange
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
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal2.jpg",
                Type = "Dinner",
                Date = today,
                CreatedAt = today.AddHours(19)
            },
            new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = "https://example.com/meal3.jpg",
                Type = "Breakfast",
                Date = today,
                CreatedAt = today.AddHours(7)
            }
        };

        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(3);
        
        // Verify order: by date desc, then by creation time desc
        result.Meals[0].CreatedAt.Should().Be(today.AddHours(19));
        result.Meals[1].CreatedAt.Should().Be(today.AddHours(12));
        result.Meals[2].CreatedAt.Should().Be(today.AddHours(7));
    }

    [Fact]
    public async Task Handle_LargePageSize_ShouldRespectMaximum()
    {
        // Arrange
        var meals = CreateTestMeals(50);
        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 150 // Larger than max
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(50); // Should return all meals
        result.TotalItems.Should().Be(50);
        result.PageSize.Should().Be(150); // Should keep the requested page size
    }

    [Fact]
    public async Task Handle_InvalidPage_ShouldUseValidPage()
    {
        // Arrange
        var meals = CreateTestMeals(10);
        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 0, // Invalid page
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().HaveCount(10);
        result.CurrentPage.Should().Be(1); // Should default to page 1
        result.TotalItems.Should().Be(10);
    }

    [Fact]
    public async Task Handle_EmptyPage_ShouldReturnEmptyList()
    {
        // Arrange
        var meals = CreateTestMeals(5);
        _context.Meals.AddRange(meals);
        _context.SaveChanges();

        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 2, // Page beyond available data
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.TotalItems.Should().Be(5);
        result.TotalPages.Should().Be(1);
        result.CurrentPage.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Exception_ShouldReturnEmptyResult()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = _testUserId,
            Page = 1,
            PageSize = 10
        };

        // Simulate database exception by disposing context
        _context.Dispose();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Meals.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
    }

    private List<Meal> CreateTestMeals(int count)
    {
        var meals = new List<Meal>();
        var today = DateTime.Today;

        for (int i = 0; i < count; i++)
        {
            meals.Add(new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = $"https://example.com/meal{i}.jpg",
                Type = i % 4 == 0 ? "Morning" : i % 4 == 1 ? "Lunch" : i % 4 == 2 ? "Dinner" : "Snack",
                Date = today.AddDays(-i),
                CreatedAt = today.AddDays(-i).AddHours(12)
            });
        }

        return meals;
    }
} 