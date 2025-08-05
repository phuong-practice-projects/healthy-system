using FluentAssertions;
using Healthy.Application.UseCases.Meals.Queries.GetMeals;

namespace Healthy.Tests.Unit.Application.Meals.Queries.GetMeals;

public class GetMealsQueryTests
{
    [Fact]
    public void ValidPage_ShouldReturnValidPageNumber()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 5,
            PageSize = 10
        };

        // Act & Assert
        query.ValidPage.Should().Be(5);
    }

    [Fact]
    public void InvalidPage_ShouldReturnMinimumPageNumber()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 0,
            PageSize = 10
        };

        // Act & Assert
        query.ValidPage.Should().Be(1);
    }

    [Fact]
    public void NegativePage_ShouldReturnMinimumPageNumber()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = -5,
            PageSize = 10
        };

        // Act & Assert
        query.ValidPage.Should().Be(1);
    }

    [Fact]
    public void ValidPageSize_ShouldReturnValidPageSize()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 25
        };

        // Act & Assert
        query.ValidPageSize.Should().Be(25);
    }

    [Fact]
    public void PageSizeExceedsMaximum_ShouldReturnMaximumPageSize()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 150
        };

        // Act & Assert
        query.ValidPageSize.Should().Be(100);
    }

    [Fact]
    public void ZeroPageSize_ShouldReturnMinimumPageSize()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 0
        };

        // Act & Assert
        query.ValidPageSize.Should().Be(1);
    }

    [Fact]
    public void NegativePageSize_ShouldReturnMinimumPageSize()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = -10
        };

        // Act & Assert
        query.ValidPageSize.Should().Be(1);
    }

    [Fact]
    public void Skip_ShouldCalculateCorrectSkipCount()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 3,
            PageSize = 10
        };

        // Act & Assert
        query.Skip.Should().Be(20); // (3-1) * 10 = 20
    }

    [Fact]
    public void Take_ShouldReturnValidPageSize()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 25
        };

        // Act & Assert
        query.Take.Should().Be(25);
    }

    [Theory]
    [InlineData("Asc", true)]
    [InlineData("asc", true)]
    [InlineData("ASC", true)]
    [InlineData("Desc", false)]
    [InlineData("desc", false)]
    [InlineData("DESC", false)]
    [InlineData("Invalid", false)]
    public void IsAscending_ShouldReturnCorrectValue(string sortDirection, bool expected)
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 10,
            SortDirection = sortDirection
        };

        // Act & Assert
        query.IsAscending.Should().Be(expected);
    }

    [Theory]
    [InlineData("Date", "CreatedAt")]
    [InlineData("Calories", "Calories")]
    [InlineData("Name", "Name")]
    [InlineData("Type", "Type")]
    [InlineData("Invalid", "CreatedAt")]
    [InlineData("", "CreatedAt")]
    [InlineData(null, "CreatedAt")]
    public void NormalizedSortBy_ShouldReturnCorrectValue(string sortBy, string expected)
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 10,
            SortBy = sortBy
        };

        // Act & Assert
        query.NormalizedSortBy.Should().Be(expected);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void HasSearchTerm_ShouldReturnCorrectValue(string searchTerm, bool expected)
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 10,
            SearchTerm = searchTerm
        };

        // Act & Assert
        query.HasSearchTerm.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    [InlineData(false, false, false)]
    public void HasDateRange_ShouldReturnCorrectValue(bool hasStartDate, bool hasEndDate, bool expected)
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 10,
            StartDate = hasStartDate ? DateTime.Today : null,
            EndDate = hasEndDate ? DateTime.Today.AddDays(1) : null
        };

        // Act & Assert
        query.HasDateRange.Should().Be(expected);
    }

    [Theory]
    [InlineData(100, null, true)]
    [InlineData(null, 500, true)]
    [InlineData(100, 500, true)]
    [InlineData(null, null, false)]
    public void HasCalorieRange_ShouldReturnCorrectValue(int? minCalories, int? maxCalories, bool expected)
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = 1,
            PageSize = 10,
            MinCalories = minCalories,
            MaxCalories = maxCalories
        };

        // Act & Assert
        query.HasCalorieRange.Should().Be(expected);
    }

    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid()
        };

        // Assert
        query.SortBy.Should().Be("Date");
        query.SortDirection.Should().Be("Desc");
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
    }

    [Fact]
    public void RequiredUserId_ShouldBeSet()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var query = new GetMealsQuery
        {
            UserId = userId
        };

        // Assert
        query.UserId.Should().Be(userId);
    }

    [Fact]
    public void OptionalParameters_ShouldBeNullable()
    {
        // Arrange & Act
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            StartDate = null,
            EndDate = null,
            Type = null,
            SearchTerm = null,
            MinCalories = null,
            MaxCalories = null
        };

        // Assert
        query.StartDate.Should().BeNull();
        query.EndDate.Should().BeNull();
        query.Type.Should().BeNull();
        query.SearchTerm.Should().BeNull();
        query.MinCalories.Should().BeNull();
        query.MaxCalories.Should().BeNull();
    }

    [Fact]
    public void ComputedProperties_ShouldWorkWithInvalidInputs()
    {
        // Arrange
        var query = new GetMealsQuery
        {
            UserId = Guid.NewGuid(),
            Page = -5,
            PageSize = -10,
            SortBy = "InvalidSort",
            SortDirection = "InvalidDirection"
        };

        // Act & Assert
        query.ValidPage.Should().Be(1);
        query.ValidPageSize.Should().Be(1);
        query.Skip.Should().Be(0); // (1-1) * 1 = 0
        query.Take.Should().Be(1);
        query.IsAscending.Should().BeFalse();
        query.NormalizedSortBy.Should().Be("CreatedAt");
    }
} 