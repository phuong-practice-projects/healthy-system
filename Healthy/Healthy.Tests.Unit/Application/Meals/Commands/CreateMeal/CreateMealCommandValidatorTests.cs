using FluentAssertions;
using Healthy.Application.UseCases.Meals.Commands.CreateMeal;

namespace Healthy.Tests.Unit.Application.Meals.Commands.CreateMeal;

public class CreateMealCommandValidatorTests
{
    private readonly CreateMealCommandValidator _validator;

    public CreateMealCommandValidatorTests()
    {
        _validator = new CreateMealCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFailValidation()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.Empty,
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "UserId");
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "User ID is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyOrNullImageUrl_ShouldFailValidation(string imageUrl)
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = imageUrl,
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Image URL is required.");
    }

    [Fact]
    public void Validate_ImageUrlTooLong_ShouldFailValidation()
    {
        // Arrange
        var longImageUrl = new string('a', 501); // 501 characters
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = longImageUrl,
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Image URL must not exceed 500 characters.");
    }

    [Theory]
    [InlineData("InvalidType")]
    [InlineData("breakfast")]
    [InlineData("LUNCH")]
    [InlineData("dinner")]
    public void Validate_InvalidMealType_ShouldFailValidation(string mealType)
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = mealType,
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Type");
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Meal type must be one of: Morning, Lunch, Dinner, Snack.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyOrNullMealType_ShouldFailValidation(string mealType)
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = mealType,
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Meal type is required.");
    }

    [Theory]
    [InlineData("Morning")]
    [InlineData("Lunch")]
    [InlineData("Dinner")]
    [InlineData("Snack")]
    public void Validate_ValidMealTypes_ShouldPassValidation(string mealType)
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = mealType,
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_DefaultDate_ShouldFailValidation()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = default(DateTime)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Date");
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Date is required.");
    }

    [Fact]
    public void Validate_FutureDateBeyondTomorrow_ShouldFailValidation()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today.AddDays(2) // Beyond tomorrow
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Date");
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Date cannot be in the future beyond tomorrow.");
    }

    [Theory]
    [InlineData(0)] // Today
    [InlineData(1)] // Tomorrow
    [InlineData(-1)] // Yesterday
    [InlineData(-30)] // Past date
    public void Validate_ValidDates_ShouldPassValidation(int daysOffset)
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/meal.jpg",
            Type = "Lunch",
            Date = DateTime.Today.AddDays(daysOffset)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.Empty,
            ImageUrl = "",
            Type = "InvalidType",
            Date = DateTime.Today.AddDays(5) // Future date beyond tomorrow
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
        result.Errors.Should().Contain(e => e.PropertyName == "ImageUrl");
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
        result.Errors.Should().Contain(e => e.PropertyName == "Date");
    }

    [Fact]
    public void Validate_ImageUrlWithValidLength_ShouldPassValidation()
    {
        // Arrange
        var validImageUrl = new string('a', 500); // Exactly 500 characters
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = validImageUrl,
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EdgeCaseImageUrl_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateMealCommand
        {
            UserId = Guid.NewGuid(),
            ImageUrl = "https://example.com/very-long-url-with-many-characters-and-special-symbols-1234567890-abcdefghijklmnopqrstuvwxyz-ABCDEFGHIJKLMNOPQRSTUVWXYZ-!@#$%^&*()_+-=[]{}|;':\",./<>?`~",
            Type = "Lunch",
            Date = DateTime.Today
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 