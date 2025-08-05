using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Healthy.Application.UseCases.Meals.Commands.DeleteMeal;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;
using Healthy.Infrastructure.Persistence;
using Healthy.Tests.Unit.Common;

namespace Healthy.Tests.Unit.Application.Meals.Commands.DeleteMeal;

public class DeleteMealCommandHandlerTests
{
    private readonly TestDbContext _context;
    private readonly DeleteMealCommandHandler _handler;
    private readonly Guid _testUserId;
    private readonly Guid _testMealId;

    public DeleteMealCommandHandlerTests()
    {
        _context = (TestDbContext)TestHelper.GetInMemoryDbContext();
        _handler = new DeleteMealCommandHandler(_context);
        _testUserId = Guid.NewGuid();
        _testMealId = Guid.NewGuid();
        
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
        
        // Seed test meal
        var testMeal = new Meal
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };
        _context.Meals.Add(testMeal);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidMealId_ShouldSoftDeleteMealSuccessfully()
    {
        // Arrange
        var command = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        // Verify meal is soft deleted
        var deletedMeal = await _context.Meals
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == _testMealId);
        
        deletedMeal.Should().NotBeNull();
        deletedMeal!.DeletedAt.Should().NotBeNull();
        deletedMeal.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_NonExistentMeal_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteMealCommand
        {
            Id = Guid.NewGuid(), // Non-existent meal
            UserId = _testUserId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Meal not found or access denied.");
    }

    [Fact]
    public async Task Handle_AlreadyDeletedMeal_ShouldReturnFailure()
    {
        // Arrange
        // First delete the meal
        var firstDeleteCommand = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };
        await _handler.Handle(firstDeleteCommand, CancellationToken.None);

        // Try to delete again
        var secondDeleteCommand = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };

        // Act
        var result = await _handler.Handle(secondDeleteCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Meal is already deleted");
    }

    [Fact]
    public async Task Handle_DeletedMealNotInQuery_ShouldReturnFailure()
    {
        // Arrange
        // Create and delete a meal
        var mealToDelete = new Meal
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ImageUrl = "https://example.com/meal2.jpg",
            Type = "Dinner",
            Date = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };
        _context.Meals.Add(mealToDelete);
        _context.SaveChanges();

        // Delete the meal
        var deleteCommand = new DeleteMealCommand
        {
            Id = mealToDelete.Id,
            UserId = _testUserId
        };
        await _handler.Handle(deleteCommand, CancellationToken.None);

        // Try to delete with non-existent ID
        var command = new DeleteMealCommand
        {
            Id = Guid.NewGuid(), // Non-existent meal
            UserId = _testUserId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Meal not found or access denied.");
    }

    [Fact]
    public async Task Handle_MultipleMeals_ShouldDeleteAllSuccessfully()
    {
        // Arrange
        var mealIds = new List<Guid>();
        for (int i = 0; i < 3; i++)
        {
            var meal = new Meal
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                ImageUrl = $"https://example.com/meal{i}.jpg",
                Type = "Lunch",
                Date = DateTime.Today,
                CreatedAt = DateTime.UtcNow
            };
            _context.Meals.Add(meal);
            mealIds.Add(meal.Id);
        }
        _context.SaveChanges();

        // Act
        var results = new List<Result<bool>>();
        foreach (var mealId in mealIds)
        {
            var command = new DeleteMealCommand
            {
                Id = mealId,
                UserId = _testUserId
            };
            var result = await _handler.Handle(command, CancellationToken.None);
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(3);
        results.Should().OnlyContain(r => r.IsSuccess);
        results.Should().OnlyContain(r => r.Value == true);

        // Verify all meals are soft deleted
        var deletedMeals = await _context.Meals
            .IgnoreQueryFilters()
            .Where(m => mealIds.Contains(m.Id))
            .ToListAsync();
        
        deletedMeals.Should().HaveCount(3);
        deletedMeals.Should().OnlyContain(m => m.DeletedAt.HasValue);
    }

    [Fact]
    public async Task Handle_DatabaseException_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };

        // Simulate database exception by disposing context
        _context.Dispose();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Error deleting meal:");
    }

    [Fact]
    public async Task Handle_CancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var command = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Error deleting meal:");
    }

    [Fact]
    public async Task Handle_SoftDeletePreservesData_ShouldMaintainOriginalData()
    {
        // Arrange
        var originalMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        originalMeal.Should().NotBeNull();

        var command = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify original data is preserved
        var deletedMeal = await _context.Meals
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == _testMealId);
        
        deletedMeal.Should().NotBeNull();
        deletedMeal!.UserId.Should().Be(originalMeal!.UserId);
        deletedMeal.ImageUrl.Should().Be(originalMeal.ImageUrl);
        deletedMeal.Type.Should().Be(originalMeal.Type);
        deletedMeal.Date.Should().Be(originalMeal.Date);
        deletedMeal.CreatedAt.Should().Be(originalMeal.CreatedAt);
        deletedMeal.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_QueryFilters_ShouldExcludeDeletedMeals()
    {
        // Arrange
        var command = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify meal is not returned in normal queries (due to query filters)
        var mealsInQuery = await _context.Meals
            .Where(m => m.UserId == _testUserId)
            .ToListAsync();
        
        mealsInQuery.Should().BeEmpty();

        // Verify meal is returned when ignoring query filters
        var mealsWithIgnoreFilters = await _context.Meals
            .IgnoreQueryFilters()
            .Where(m => m.UserId == _testUserId)
            .ToListAsync();
        
        mealsWithIgnoreFilters.Should().HaveCount(1);
        mealsWithIgnoreFilters.First().DeletedAt.Should().NotBeNull();
    }
} 