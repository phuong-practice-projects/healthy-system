using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Columns.Commands.CreateColumn;
using Healthy.Application.UseCases.Columns.Commands.DeleteColumn;
using Healthy.Application.UseCases.Columns.Commands.UpdateColumn;
using Healthy.Application.UseCases.Columns.Queries.GetColumn;
using Healthy.Application.UseCases.Columns.Queries.GetColumns;

namespace Healthy.Tests.Unit.Api.Controllers;

public class ColumnsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ColumnsController _controller;

    public ColumnsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ColumnsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetColumns_WithDefaultParameters_ShouldReturnOkResultWithCorrectStructure()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var paginatedResult = new PaginatedList<ColumnDto>(
            new List<ColumnDto>
            {
                new ColumnDto
                {
                    Id = columnId,
                    Title = "Test Column",
                    Content = "Test content for column",
                    Category = "diet",
                    ImageUrl = "/images/column-01.jpg",
                    Tags = "#test,#unit",
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                }
            },
            1, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetColumns();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as PaginatedList<ColumnDto>;
        response.Should().NotBeNull();
        response!.Items.Should().HaveCount(1);
        response.TotalCount.Should().Be(1);
        response.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetColumns_WithCustomParameters_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var expectedPage = 2;
        var expectedLimit = 5;
        var expectedCategory = "diet";
        
        var paginatedResult = new PaginatedList<ColumnDto>(new List<ColumnDto>(), 0, expectedPage, expectedLimit);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paginatedResult);

        // Act
        await _controller.GetColumns(expectedPage, expectedLimit, expectedCategory);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetColumnsQuery>(q => 
                    q.Page == expectedPage && 
                    q.Limit == expectedLimit && 
                    q.Category == expectedCategory),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 10, null, 1, 10)] // Invalid page should default to 1
    [InlineData(-1, 10, null, 1, 10)] // Negative page should default to 1
    [InlineData(1, 0, null, 1, 10)] // Invalid limit should default to 10
    [InlineData(1, -1, null, 1, 10)] // Negative limit should default to 10
    [InlineData(1, 150, null, 1, 10)] // Limit > 100 should default to 10
    public async Task GetColumns_WithInvalidParameters_ShouldUseDefaultValues(
        int inputPage, int inputLimit, string? inputCategory,
        int expectedPage, int expectedLimit)
    {
        // Arrange
        var paginatedResult = new PaginatedList<ColumnDto>(new List<ColumnDto>(), 0, expectedPage, expectedLimit);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paginatedResult);

        // Act
        await _controller.GetColumns(inputPage, inputLimit, inputCategory);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetColumnsQuery>(q => 
                    q.Page == expectedPage && 
                    q.Limit == expectedLimit),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetColumns_WithValidCategory_ShouldPassCategoryToQuery()
    {
        // Arrange
        var categories = new[] { "diet", "recommended", "beauty" };
        var paginatedResult = new PaginatedList<ColumnDto>(new List<ColumnDto>(), 0, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paginatedResult);

        foreach (var category in categories)
        {
            // Act
            await _controller.GetColumns(1, 10, category);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<GetColumnsQuery>(q => q.Category == category),
                    It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
        }
    }

    [Fact]
    public async Task GetColumns_ShouldReturnCorrectResponseFormat()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var columnDto = new ColumnDto
        {
            Id = columnId,
            Title = "Health Tips Column",
            Content = "Detailed content about health tips",
            Category = "diet",
            ImageUrl = "/images/column-01.jpg",
            Tags = "#health,#tips,#diet",
            IsPublished = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt.AddHours(1)
        };

        var paginatedResult = new PaginatedList<ColumnDto>(
            new List<ColumnDto> { columnDto },
            5, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetColumns();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as PaginatedList<ColumnDto>;
        response.Should().NotBeNull();
        
        // Check columns data
        response!.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        
        var firstColumn = response.Items.First();
        firstColumn.Id.Should().Be(columnId);
        firstColumn.Title.Should().Be("Health Tips Column");
        firstColumn.Content.Should().Be("Detailed content about health tips");
        firstColumn.Category.Should().Be("diet");
        firstColumn.ImageUrl.Should().Be("/images/column-01.jpg");
        firstColumn.Tags.Should().Be("#health,#tips,#diet");
        firstColumn.IsPublished.Should().BeTrue();
        firstColumn.CreatedAt.Should().Be(createdAt);
        firstColumn.UpdatedAt.Should().Be(createdAt.AddHours(1));

        // Check pagination data
        response.PageNumber.Should().Be(1);
        response.TotalPages.Should().Be(1);
        response.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetColumns_WithEmptyResult_ShouldReturnEmptyArrayAndZeroPagination()
    {
        // Arrange
        var paginatedResult = new PaginatedList<ColumnDto>(new List<ColumnDto>(), 0, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetColumns();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as PaginatedList<ColumnDto>;
        response.Should().NotBeNull();
        
        response!.Items.Should().NotBeNull();
        response.Items.Should().BeEmpty();
        response.TotalCount.Should().Be(0);
        response.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetColumn_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var columnDto = new ColumnDto
        {
            Id = columnId,
            Title = "Test Column",
            Content = "Test content",
            Category = "diet",
            ImageUrl = "/images/test.jpg",
            Tags = "#test",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = Result<ColumnDto>.Success(columnDto);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetColumn(columnId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetColumn_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var result = Result<ColumnDto>.Failure("Column not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetColumnQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetColumn(columnId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateColumn_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var request = new CreateColumnRequest
        {
            Title = "New Column",
            Content = "New content",
            Category = "diet",
            ImageUrl = "/images/new.jpg",
            Tags = "#new,#test",
            IsPublished = true
        };

        var result = Result<Guid>.Success(columnId);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateColumnCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateColumn(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(ColumnsController.GetColumn));
        createdResult.RouteValues!["id"].Should().Be(columnId);
        createdResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateColumn_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateColumnRequest
        {
            Title = "", // Invalid empty title
            Content = "Content",
            Category = "diet"
        };

        var result = Result<Guid>.Failure("Title cannot be empty");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateColumnCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateColumn(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateColumn_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var request = new UpdateColumnRequest
        {
            Title = "Updated Column",
            Content = "Updated content",
            Category = "beauty",
            ImageUrl = "/images/updated.jpg",
            Tags = "#updated,#test",
            IsPublished = false
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateColumnCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateColumn(columnId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateColumn_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var request = new UpdateColumnRequest
        {
            Title = "Updated Column",
            Content = "Updated content"
        };

        var result = Result<bool>.Failure("Column not found");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateColumnCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateColumn(columnId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteColumn_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteColumnCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteColumn(columnId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteColumn_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var result = Result<bool>.Failure("Column not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteColumnCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteColumn(columnId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }
}
