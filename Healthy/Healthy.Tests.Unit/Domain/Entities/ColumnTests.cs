using FluentAssertions;
using Healthy.Domain.Entities;
using Healthy.Domain.Common;

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
        column.DeletedAt.Should().BeNull();
        column.UpdatedAt.Should().BeNull();
        column.CreatedBy.Should().BeNull();
        column.UpdatedBy.Should().BeNull();
        column.DeletedBy.Should().BeNull();
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
    public void Column_InheritsFromEntityAuditableBase_ShouldHaveBaseProperties()
    {
        // Arrange & Act
        var column = new Column();

        // Assert
        column.Should().BeAssignableTo<EntityAuditableBase>();
        column.Id.Should().NotBe(Guid.Empty);
        column.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        column.UpdatedAt.Should().BeNull();
        column.IsDeleted.Should().BeFalse();
        column.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Column_Delete_ShouldMarkAsDeleted()
    {
        // Arrange
        var column = new Column();
        var deletedBy = "test-user";

        // Act
        column.Delete(deletedBy);

        // Assert
        column.IsDeleted.Should().BeTrue();
        column.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        column.DeletedBy.Should().Be(deletedBy);
    }

    [Fact]
    public void Column_Restore_ShouldClearDeletedProperties()
    {
        // Arrange
        var column = new Column();
        column.Delete("test-user");

        // Act
        column.Restore();

        // Assert
        column.IsDeleted.Should().BeFalse();
        column.DeletedAt.Should().BeNull();
        column.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void Column_UpdateAuditInfo_ShouldSetUpdateProperties()
    {
        // Arrange
        var column = new Column();
        var updatedBy = "test-updater";

        // Act
        column.UpdateAuditInfo(updatedBy);

        // Assert
        column.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        column.UpdatedBy.Should().Be(updatedBy);
    }

    [Theory]
    [InlineData("Health & Wellness")]
    [InlineData("Diet & Nutrition")]
    [InlineData("Exercise & Fitness")]
    public void Column_SetValidCategory_ShouldAcceptValue(string category)
    {
        // Arrange
        var column = new Column();

        // Act
        column.Category = category;

        // Assert
        column.Category.Should().Be(category);
    }

    [Theory]
    [InlineData("#health,#wellness,#diet")]
    [InlineData("#fitness,#exercise")]
    [InlineData("")]
    public void Column_SetValidTags_ShouldAcceptValue(string tags)
    {
        // Arrange
        var column = new Column();

        // Act
        column.Tags = tags;

        // Assert
        column.Tags.Should().Be(tags);
    }

    [Fact]
    public void Column_TogglePublishStatus_ShouldUpdateCorrectly()
    {
        // Arrange
        var column = new Column();
        column.IsPublished.Should().BeTrue(); // Default value

        // Act & Assert
        column.IsPublished = false;
        column.IsPublished.Should().BeFalse();

        column.IsPublished = true;
        column.IsPublished.Should().BeTrue();
    }
}
