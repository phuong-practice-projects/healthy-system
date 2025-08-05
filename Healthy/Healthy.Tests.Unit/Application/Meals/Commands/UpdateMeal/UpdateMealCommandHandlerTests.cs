using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Healthy.Application.UseCases.Meals.Commands.UpdateMeal;
using Healthy.Application.Common.Interfaces;
using Healthy.Domain.Entities;
using Healthy.Tests.Unit.Common;

namespace Healthy.Tests.Unit.Application.Meals.Commands.UpdateMeal;

public class UpdateMealCommandHandlerTests
{
    private readonly TestDbContext _context;
    private readonly UpdateMealCommandHandler _handler;
    private readonly Guid _testUserId;
    private readonly Guid _testMealId;

    public UpdateMealCommandHandlerTests()
    {
        _context = (TestDbContext)TestHelper.GetInMemoryDbContext();
        _handler = new UpdateMealCommandHandler(_context);
        _testUserId = Guid.NewGuid();
        _testMealId = Guid.NewGuid();
        
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
    public async Task Handle_ValidUpdate_ShouldUpdateMealSuccessfully()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Dinner",
            Date = DateTime.Today.AddDays(1)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        // Verify meal was updated in database
        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        updatedMeal!.ImageUrl.Should().Be("https://example.com/updated-meal.jpg");
        updatedMeal.Type.Should().Be("Dinner");
        updatedMeal.Date.Should().Be(DateTime.Today.AddDays(1).Date);
        updatedMeal.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_NonExistentMeal_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = Guid.NewGuid(), // Non-existent meal
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Dinner",
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Meal not found or access denied.");
    }

    [Theory]
    [InlineData("Morning")]
    [InlineData("Lunch")]
    [InlineData("Dinner")]
    [InlineData("Snack")]
    public async Task Handle_AllValidMealTypes_ShouldUpdateSuccessfully(string mealType)
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = mealType,
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        updatedMeal!.Type.Should().Be(mealType);
    }

    [Fact]
    public async Task Handle_EmptyImageUrl_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
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
        result.Value.Should().BeTrue();

        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        updatedMeal!.ImageUrl.Should().Be("");
    }

    [Fact]
    public async Task Handle_FutureDate_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today.AddDays(5)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        updatedMeal!.Date.Should().Be(DateTime.Today.AddDays(5).Date);
    }

    [Fact]
    public async Task Handle_PastDate_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today.AddDays(-10)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        updatedMeal!.Date.Should().Be(DateTime.Today.AddDays(-10).Date);
    }

    [Fact]
    public async Task Handle_MultipleUpdates_ShouldUpdateSuccessfully()
    {
        // Arrange
        var updates = new[]
        {
            new UpdateMealCommand
            {
                Id = _testMealId,
                UserId = _testUserId,
                ImageUrl = "https://example.com/update1.jpg",
                Type = "Morning",
                Date = DateTime.Today
            },
            new UpdateMealCommand
            {
                Id = _testMealId,
                UserId = _testUserId,
                ImageUrl = "https://example.com/update2.jpg",
                Type = "Lunch",
                Date = DateTime.Today.AddDays(1)
            },
            new UpdateMealCommand
            {
                Id = _testMealId,
                UserId = _testUserId,
                ImageUrl = "https://example.com/update3.jpg",
                Type = "Dinner",
                Date = DateTime.Today.AddDays(2)
            }
        };

        // Act
        var results = new List<Result<bool>>();
        foreach (var update in updates)
        {
            var result = await _handler.Handle(update, CancellationToken.None);
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(3);
        results.Should().OnlyContain(r => r.IsSuccess);
        results.Should().OnlyContain(r => r.Value == true);

        // Verify final state
        var finalMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        finalMeal.Should().NotBeNull();
        finalMeal!.ImageUrl.Should().Be("https://example.com/update3.jpg");
        finalMeal.Type.Should().Be("Dinner");
        finalMeal.Date.Should().Be(DateTime.Today.AddDays(2).Date);
    }

    [Fact]
    public async Task Handle_DatabaseException_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Dinner",
            Date = DateTime.Today
        };

        // Simulate database exception by disposing context
        _context.Dispose();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Error updating meal:");
    }

    [Fact]
    public async Task Handle_CancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Dinner",
            Date = DateTime.Today
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Error updating meal:");
    }

    [Fact]
    public async Task Handle_UpdatePreservesOriginalData_ShouldMaintainUnchangedFields()
    {
        // Arrange
        var originalMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        originalMeal.Should().NotBeNull();

        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Dinner",
            Date = DateTime.Today.AddDays(1)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        
        // Verify unchanged fields are preserved
        updatedMeal!.UserId.Should().Be(originalMeal!.UserId);
        updatedMeal.CreatedAt.Should().Be(originalMeal.CreatedAt);
        
        // Verify changed fields are updated
        updatedMeal.ImageUrl.Should().Be("https://example.com/updated-meal.jpg");
        updatedMeal.Type.Should().Be("Dinner");
        updatedMeal.Date.Should().Be(DateTime.Today.AddDays(1).Date);
        updatedMeal.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_UpdateDeletedMeal_ShouldReturnFailure()
    {
        // Arrange
        // First delete the meal
        var deleteHandler = new DeleteMealCommandHandler(_context);
        var deleteCommand = new DeleteMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId
        };
        await deleteHandler.Handle(deleteCommand, CancellationToken.None);

        // Try to update the deleted meal
        var updateCommand = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = "https://example.com/updated-meal.jpg",
            Type = "Dinner",
            Date = DateTime.Today
        };

        // Act
        var result = await _handler.Handle(updateCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Meal not found or access denied.");
    }

    [Fact]
    public async Task Handle_UpdateWithSameData_ShouldUpdateTimestamp()
    {
        // Arrange
        var originalMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        originalMeal.Should().NotBeNull();

        var command = new UpdateMealCommand
        {
            Id = _testMealId,
            UserId = _testUserId,
            ImageUrl = originalMeal!.ImageUrl, // Same data
            Type = originalMeal.Type, // Same data
            Date = originalMeal.Date // Same data
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedMeal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == _testMealId);
        updatedMeal.Should().NotBeNull();
        updatedMeal!.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
} 