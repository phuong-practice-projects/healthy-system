using FluentAssertions;
using Healthy.Domain.Entities;

namespace Healthy.Tests.Unit.Domain.Entities;

public class ColumnTests
{
    [Fact]
    public void Column_Constructor_ShouldSetDefaultValues()
    {
        // Act
        var column = new Column();

        // Assert
        column.Id.Should().NotBe(Guid.Empty);
        column.Title.Should().Be(string.Empty);
        column.Content.Should().Be(string.Empty);
        column.ImageUrl.Should().BeNull();
        column.Category.Should().BeNull();
        column.Tags.Should().BeNull();
        column.IsPublished.Should().BeTrue();
        column.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        column.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Column_SetProperties_ShouldStoreCorrectValues()
    {
        // Arrange
        var column = new Column();
        var title = "Test Column Title";
        var content = "Test column content";
        var imageUrl = "/images/test.jpg";
        var category = "diet";
        var tags = "#test,#unit";

        // Act
        column.Title = title;
        column.Content = content;
        column.ImageUrl = imageUrl;
        column.Category = category;
        column.Tags = tags;
        column.IsPublished = false;

        // Assert
        column.Title.Should().Be(title);
        column.Content.Should().Be(content);
        column.ImageUrl.Should().Be(imageUrl);
        column.Category.Should().Be(category);
        column.Tags.Should().Be(tags);
        column.IsPublished.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Column_EmptyOrNullCategory_ShouldBeAllowed(string? category)
    {
        // Arrange
        var column = new Column();

        // Act & Assert
        var action = () => column.Category = category;
        action.Should().NotThrow();
        column.Category.Should().Be(category);
    }

    [Fact]
    public void Column_InheritsFromBaseEntity_ShouldHaveBaseProperties()
    {
        // Arrange & Act
        var column = new Column();

        // Assert
        column.Should().BeAssignableTo<BaseEntity>();
        column.Id.Should().NotBe(Guid.Empty);
        column.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        column.UpdatedAt.Should().BeNull();
        column.IsDeleted.Should().BeFalse();
    }
}
