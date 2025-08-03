using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Columns.Queries.GetColumns;
using Healthy.Tests.Unit.Common;
using Healthy.Infrastructure.Persistence;

namespace Healthy.Tests.Unit.Application.Columns.Queries;

public class GetColumnsQueryHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetColumnsQueryHandler _handler;

    public GetColumnsQueryHandlerTests()
    {
        _context = TestHelper.GetInMemoryDbContext();
        _handler = new GetColumnsQueryHandler(_context);

        // Seed test data only once per test
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Clear any existing data
        _context.Columns.RemoveRange(_context.Columns);
        
        // Add specific test data
        var columns = TestHelper.GetSampleColumns();
        _context.Columns.AddRange(columns);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_WithDefaultQuery_ShouldReturnAllActiveColumns()
    {
        // Arrange
        var query = new GetColumnsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3); // Only active and non-deleted columns
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ShouldReturnFilteredColumns()
    {
        // Arrange
        var query = new GetColumnsQuery { Category = "diet" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1); // Only 1 active diet column (not deleted)
        result.Items.First().Tags.Should().Contain("#魚料理");
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithRecommendedCategory_ShouldReturnRecommendedColumns()
    {
        // Arrange
        var query = new GetColumnsQuery { Category = "recommended" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1); // Only 1 active recommended column
        result.Items.First().Tags.Should().Contain("#ダイエット");
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithBeautyCategory_ShouldReturnBeautyColumns()
    {
        // Arrange
        var query = new GetColumnsQuery { Category = "beauty" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Tags.Should().Contain("#美容");
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetColumnsQuery { Page = 1, Limit = 2 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithSecondPage_ShouldReturnRemainingItems()
    {
        // Arrange
        var query = new GetColumnsQuery { Page = 2, Limit = 2 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1); // Only 1 item on second page
        result.PageNumber.Should().Be(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldReturnEmptyResult()
    {
        // Arrange
        var query = new GetColumnsQuery { Category = "nonexistent" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnColumnsOrderedByPublishedAtDescending()
    {
        // Arrange
        var query = new GetColumnsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        
        // Check if ordered by CreatedAt descending (most recent first)
        var timestamps = result.Items.Select(c => c.CreatedAt).ToList();
        timestamps.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task Handle_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var query = new GetColumnsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        var firstItem = result.Items.First();
        firstItem.Id.Should().NotBe(Guid.Empty);
        firstItem.ImageUrl.Should().NotBeNullOrEmpty();
        firstItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(30));
        firstItem.Content.Should().NotBeNullOrEmpty();
        firstItem.Tags.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_ShouldConvertTagsCorrectly()
    {
        // Arrange
        var query = new GetColumnsQuery { Category = "diet" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);

        var column = result.Items.First();
        column.Tags.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_ShouldNotReturnInactiveColumns()
    {
        // Arrange
        var query = new GetColumnsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        // Should not contain the unpublished column
        result.Items.Should().NotContain(c => c.Content.Contains("運動習慣"));
    }

    [Fact]
    public async Task Handle_ShouldNotReturnDeletedColumns()
    {
        // Arrange
        var query = new GetColumnsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        // Should not contain the soft deleted column
        result.Items.Should().NotContain(c => c.Content.Contains("バランスの良い食事"));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
