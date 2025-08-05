using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Healthy.Api;
using Healthy.Infrastructure.Persistence;
using Healthy.Tests.Unit.Common;

namespace Healthy.Tests.Unit.Integration;

[Trait("Category", "Integration")]
public class ColumnsIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>, IDisposable
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    public ColumnsIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        
        // Keep the scope alive for the test lifetime
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _context.Database.EnsureCreated();

        // Seed test data using the API's context
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Clear existing data to ensure clean state for each test
        _context.Columns.RemoveRange(_context.Columns);
        _context.SaveChanges();
        
        // Seed fresh test data
        var columns = TestHelper.GetSampleColumns();
        _context.Columns.AddRange(columns);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GET_Columns_ShouldReturnSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/columns");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        
        // Parse response and verify structure
        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        // PaginatedList structure
        root.TryGetProperty("items", out var itemsProperty).Should().BeTrue();
        root.TryGetProperty("pageNumber", out var pageNumberProperty).Should().BeTrue();
        root.TryGetProperty("totalPages", out var totalPagesProperty).Should().BeTrue();
        root.TryGetProperty("totalCount", out var totalCountProperty).Should().BeTrue();

        itemsProperty.ValueKind.Should().Be(JsonValueKind.Array);
        
        // Verify we have some data
        itemsProperty.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GET_Columns_ShouldReturnExpectedJsonStructure()
    {
        // Act
        var response = await _client.GetAsync("/api/columns");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrEmpty();

        var jsonDocument = JsonDocument.Parse(content);
        var root = jsonDocument.RootElement;

        root.TryGetProperty("items", out var itemsProperty).Should().BeTrue();
        root.TryGetProperty("pageNumber", out var pageNumberProperty).Should().BeTrue();
        root.TryGetProperty("totalPages", out var totalPagesProperty).Should().BeTrue();
        root.TryGetProperty("totalCount", out var totalCountProperty).Should().BeTrue();

        itemsProperty.ValueKind.Should().Be(JsonValueKind.Array);
        pageNumberProperty.ValueKind.Should().Be(JsonValueKind.Number);
        totalPagesProperty.ValueKind.Should().Be(JsonValueKind.Number);
        totalCountProperty.ValueKind.Should().Be(JsonValueKind.Number);
    }

    [Fact]
    public async Task GET_Columns_ShouldReturnOnlyActiveColumns()
    {
        // Act
        var response = await _client.GetAsync("/api/columns");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonDocument = JsonDocument.Parse(content);
        var items = jsonDocument.RootElement.GetProperty("items");
        var totalCount = jsonDocument.RootElement.GetProperty("totalCount");

        items.GetArrayLength().Should().Be(3); // Only active and non-deleted columns
        totalCount.GetInt32().Should().Be(3);
    }

    [Theory]
    [InlineData("diet", 1)]
    [InlineData("recommended", 1)]
    [InlineData("beauty", 1)]
    [InlineData("nonexistent", 0)]
    public async Task GET_Columns_WithCategoryFilter_ShouldReturnFilteredResults(string category, int expectedCount)
    {
        // Act
        var response = await _client.GetAsync($"/api/columns?category={category}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonDocument = JsonDocument.Parse(content);
        var items = jsonDocument.RootElement.GetProperty("items");
        var totalCount = jsonDocument.RootElement.GetProperty("totalCount");

        items.GetArrayLength().Should().Be(expectedCount);
        totalCount.GetInt32().Should().Be(expectedCount);
    }

    [Fact]
    public async Task GET_Columns_WithPagination_ShouldReturnCorrectPage()
    {
        // Act
        var response = await _client.GetAsync("/api/columns?page=1&limit=2");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonDocument = JsonDocument.Parse(content);
        var items = jsonDocument.RootElement.GetProperty("items");
        var pageNumber = jsonDocument.RootElement.GetProperty("pageNumber");
        var totalPages = jsonDocument.RootElement.GetProperty("totalPages");
        var totalCount = jsonDocument.RootElement.GetProperty("totalCount");

        items.GetArrayLength().Should().Be(2);
        pageNumber.GetInt32().Should().Be(1);
        totalPages.GetInt32().Should().Be(2);
        totalCount.GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task GET_Columns_WithSecondPage_ShouldReturnRemainingItems()
    {
        // Act
        var response = await _client.GetAsync("/api/columns?page=2&limit=2");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonDocument = JsonDocument.Parse(content);
        var items = jsonDocument.RootElement.GetProperty("items");
        var pageNumber = jsonDocument.RootElement.GetProperty("pageNumber");
        var totalPages = jsonDocument.RootElement.GetProperty("totalPages");
        var totalCount = jsonDocument.RootElement.GetProperty("totalCount");

        items.GetArrayLength().Should().Be(1); // Only 1 item on second page
        pageNumber.GetInt32().Should().Be(2);
        totalPages.GetInt32().Should().Be(2);
        totalCount.GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task GET_Columns_ShouldReturnColumnsWithCorrectFormat()
    {
        // Act
        var response = await _client.GetAsync("/api/columns");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonDocument = JsonDocument.Parse(content);
        var items = jsonDocument.RootElement.GetProperty("items");

        if (items.GetArrayLength() > 0)
        {
            var firstColumn = items[0];

            firstColumn.TryGetProperty("id", out var idProperty).Should().BeTrue();
            firstColumn.TryGetProperty("title", out var titleProperty).Should().BeTrue();
            firstColumn.TryGetProperty("content", out var contentProperty).Should().BeTrue();
            firstColumn.TryGetProperty("imageUrl", out var imageUrlProperty).Should().BeTrue();
            firstColumn.TryGetProperty("category", out var categoryProperty).Should().BeTrue();
            firstColumn.TryGetProperty("tags", out var tagsProperty).Should().BeTrue();
            firstColumn.TryGetProperty("isPublished", out var isPublishedProperty).Should().BeTrue();
            firstColumn.TryGetProperty("createdAt", out var createdAtProperty).Should().BeTrue();

            idProperty.ValueKind.Should().Be(JsonValueKind.String);
            titleProperty.ValueKind.Should().Be(JsonValueKind.String);
            contentProperty.ValueKind.Should().Be(JsonValueKind.String);
            imageUrlProperty.ValueKind.Should().Be(JsonValueKind.String);
            isPublishedProperty.ValueKind.Should().Be(JsonValueKind.True);

            // Check tags format (if it's a string containing comma-separated tags)
            if (tagsProperty.ValueKind == JsonValueKind.String)
            {
                var tagsValue = tagsProperty.GetString();
                if (!string.IsNullOrEmpty(tagsValue))
                {
                    tagsValue.Should().Contain("#");
                }
            }
        }
    }

    [Theory]
    [InlineData("page=-1&limit=10", 1, 10)] // Invalid page
    [InlineData("page=0&limit=10", 1, 10)] // Invalid page
    [InlineData("page=1&limit=-1", 1, 10)] // Invalid limit
    [InlineData("page=1&limit=0", 1, 10)] // Invalid limit
    [InlineData("page=1&limit=150", 1, 10)] // Limit too high
    public async Task GET_Columns_WithInvalidParameters_ShouldUseDefaults(string queryString, int expectedPage, int expectedLimit)
    {
        // Act
        var response = await _client.GetAsync($"/api/columns?{queryString}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonDocument = JsonDocument.Parse(content);
        var pageNumber = jsonDocument.RootElement.GetProperty("pageNumber");

        pageNumber.GetInt32().Should().Be(expectedPage);
        
        // Verify the defaults are applied
        var items = jsonDocument.RootElement.GetProperty("items");
        var itemCount = items.GetArrayLength();
        
        // Should use default limit (10) when invalid limit is provided
        itemCount.Should().BeLessOrEqualTo(expectedLimit);
    }

    public void Dispose()
    {
        _client.Dispose();
        _context.Dispose();
        _scope.Dispose();
    }
}
