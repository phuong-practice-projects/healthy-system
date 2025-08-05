using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Moq;
using Healthy.Application.UseCases.Meals.Commands.CreateMeal;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;
using Healthy.Infrastructure.Persistence;
using Healthy.Tests.Unit.Common;

namespace Healthy.Tests.Unit.Application.Meals.Commands.CreateMeal;

public class CreateMealCommandHandlerTests
{
    private readonly TestDbContext _context;
    private readonly CreateMealCommandHandler _handler;
    private readonly Guid _testUserId;

    public CreateMealCommandHandlerTests()
    {
        _context = (TestDbContext)TestHelper.GetInMemoryDbContext();
        _handler = new CreateMealCommandHandler(_context);
        _testUserId = Guid.NewGuid();
        
        // Seed test user
        var testUser = new User
        {
            Id = _testUserId,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "dummy-hash",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(testUser);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateMealSuccessfully()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Verify meal was created in database
        var createdMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == result.Value);
        createdMeal.Should().NotBeNull();
        createdMeal!.UserId.Should().Be(_testUserId);
        createdMeal.ImageUrl.Should().Be("https://example.com/meal.jpg");
        createdMeal.Type.Should().Be("Lunch");
        createdMeal.Date.Should().Be(DateTime.Today.Date);
        createdMeal.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(), // Non-existent user
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Theory]
    [InlineData("Morning")]
    [InlineData("Lunch")]
    [InlineData("Dinner")]
    [InlineData("Snack")]
    public async Task Handle_AllValidMealTypes_ShouldCreateMealSuccessfully(string mealType)
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = mealType,
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var createdMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == result.Value);
        createdMeal.Should().NotBeNull();
        createdMeal!.Type.Should().Be(mealType);
    }

    [Fact]
    public async Task Handle_EmptyImageUrl_ShouldCreateMealSuccessfully()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var createdMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == result.Value);
        createdMeal.Should().NotBeNull();
        createdMeal!.ImageUrl.Should().Be("");
    }

    [Fact]
    public async Task Handle_FutureDate_ShouldCreateMealSuccessfully()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today.AddDays(1)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var createdMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == result.Value);
        createdMeal.Should().NotBeNull();
        createdMeal!.Date.Should().Be(DateTime.Today.AddDays(1).Date);
    }

    [Fact]
    public async Task Handle_PastDate_ShouldCreateMealSuccessfully()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today.AddDays(-5)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var createdMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == result.Value);
        createdMeal.Should().NotBeNull();
        createdMeal!.Date.Should().Be(DateTime.Today.AddDays(-5).Date);
    }

    [Fact]
    public async Task Handle_DatabaseException_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Simulate database exception by disposing context
        _context.Dispose();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Error creating meal:");
    }

    [Fact]
    public async Task Handle_CancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Error creating meal:");
    }

    [Fact]
    public async Task Handle_MultipleMealsSameUser_ShouldCreateAllSuccessfully()
    {
        // Arrange
        var commands = new[]
        {
            new CreateMealCommand
            {
                UserId = _testUserId,
                ImageUrl = "https://example.com/breakfast.jpg",
                Type = "Morning",
                Date = DateTime.Today
            },
            new CreateMealCommand
            {
                UserId = _testUserId,
                ImageUrl = "https://example.com/lunch.jpg",
                Type = "Lunch",
                Date = DateTime.Today
            },
            new CreateMealCommand
            {
                UserId = _testUserId,
                ImageUrl = "https://example.com/dinner.jpg",
                Type = "Dinner",
                Date = DateTime.Today
            }
        };

        // Act
        var results = new List<Result<Guid>>();
        foreach (var command in commands)
        {
            var result = await _handler.Handle(command, CancellationToken.None);
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(3);
        results.Should().OnlyContain(r => r.IsSuccess);
        results.Should().OnlyContain(r => r.Value != Guid.Empty);

        var meals = await _context.Meals.Where(m => m.UserId == _testUserId).ToListAsync();
        meals.Should().HaveCount(3);
        meals.Should().OnlyContain(m => m.UserId == _testUserId);
    }
} 