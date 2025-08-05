using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.BodyRecords.Commands.CreateBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Commands.DeleteBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Commands.UpdateBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecord;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecordGraph;
using Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecords;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class BodyRecordsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly BodyRecordsController _controller;

    public BodyRecordsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new BodyRecordsController(_mediatorMock.Object);
        
        // Setup default current user
        var testUserId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(testUserId.ToString());
        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(true);
        
        // Mock the HttpContext and RequestServices
        var httpContext = new DefaultHttpContext();
        var serviceProvider = new TestServiceProvider(_currentUserServiceMock.Object);
        httpContext.RequestServices = serviceProvider;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetBodyRecords_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var request = new GetBodyRecordsQueryRequest { UserId = Guid.NewGuid() };
        var bodyRecordsListResponse = new BodyRecordsListResponse
        {
            Items = new List<BodyRecordDto>(),
            TotalItems = 0,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordsQueryRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bodyRecordsListResponse);

        // Act
        var result = await _controller.GetBodyRecords(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as BodyRecordsListResponse;
        response.Should().NotBeNull();
        response!.Items.Should().NotBeNull();
        response.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetBodyRecords_WithCustomParameters_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var request = new GetBodyRecordsQueryRequest
        {
            UserId = Guid.NewGuid(),
            Page = 2,
            PageSize = 5,
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today,
            SortBy = "Date",
            SortDirection = "Desc"
        };

        var bodyRecordsListResponse = new BodyRecordsListResponse
        {
            Items = new List<BodyRecordDto>(),
            TotalItems = 0,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(bodyRecordsListResponse);

        // Act
        await _controller.GetBodyRecords(request);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetBodyRecordsQueryRequest>(q => 
                    q.Page == request.Page &&
                    q.PageSize == request.PageSize &&
                    q.StartDate == request.StartDate &&
                    q.EndDate == request.EndDate &&
                    q.SortBy == request.SortBy &&
                    q.SortDirection == request.SortDirection),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBodyRecords_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var request = new GetBodyRecordsQueryRequest { UserId = Guid.NewGuid() };

        // Act
        var result = await _controller.GetBodyRecords(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetBodyRecords_WithInvalidGuidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns("invalid-guid");
        var request = new GetBodyRecordsQueryRequest { UserId = Guid.NewGuid() };

        // Act
        var result = await _controller.GetBodyRecords(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetBodyRecord_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var bodyRecordDto = new BodyRecordDto
        {
            Id = recordId,
            UserId = Guid.NewGuid(),
            Weight = 70.5m,
            BodyFatPercentage = 15.0m,
            RecordDate = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };

        var result = Result<BodyRecordDto>.Success(bodyRecordDto);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetBodyRecord(recordId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetBodyRecord_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var result = Result<BodyRecordDto>.Failure("Body record not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetBodyRecord(recordId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetBodyRecord_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var recordId = Guid.NewGuid();

        // Act
        var response = await _controller.GetBodyRecord(recordId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetBodyRecordGraph_WithValidParameters_ShouldReturnOkResult()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-30);
        var endDate = DateTime.Today;
        var graphResponse = new BodyRecordGraphResponse
        {
            GraphData = new List<BodyRecordGraphData>
            {
                new BodyRecordGraphData
                {
                    Date = DateTime.Today.AddDays(-1),
                    Weight = 70.0m,
                    BodyFat = 15.0m
                },
                new BodyRecordGraphData
                {
                    Date = DateTime.Today,
                    Weight = 70.5m,
                    BodyFat = 14.8m
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBodyRecordGraphQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(graphResponse);

        // Act
        var result = await _controller.GetBodyRecordGraph(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as BodyRecordGraphResponse;
        response.Should().NotBeNull();
        response!.GraphData.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBodyRecordGraph_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var result = await _controller.GetBodyRecordGraph();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task CreateBodyRecord_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var request = new CreateBodyRecordRequest
        {
            Weight = 70.5m,
            BodyFatPercentage = 15.0m,
            RecordDate = DateTime.Today,
            Notes = "Good progress"
        };

        var result = Result<Guid>.Success(recordId);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateBodyRecordCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateBodyRecord(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(BodyRecordsController.GetBodyRecord));
        createdResult.RouteValues!["id"].Should().Be(recordId);
        createdResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateBodyRecord_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateBodyRecordRequest
        {
            Weight = -10m, // Invalid negative weight
            BodyFatPercentage = 15.0m,
            RecordDate = DateTime.Today,
            Notes = "Invalid weight"
        };

        var result = Result<Guid>.Failure("Weight cannot be negative");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateBodyRecordCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateBodyRecord(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateBodyRecord_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var request = new CreateBodyRecordRequest
        {
            Weight = 70.5m,
            BodyFatPercentage = 15.0m,
            RecordDate = DateTime.Today,
            Notes = "Test record"
        };

        // Act
        var response = await _controller.CreateBodyRecord(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateBodyRecord_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var request = new UpdateBodyRecordRequest
        {
            Weight = 71.0m,
            BodyFatPercentage = 14.5m,
            RecordDate = DateTime.Today,
            Notes = "Updated progress"
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBodyRecordCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateBodyRecord(recordId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateBodyRecord_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var request = new UpdateBodyRecordRequest
        {
            Weight = 71.0m,
            BodyFatPercentage = 14.5m,
            RecordDate = DateTime.Today,
            Notes = "Updated progress"
        };

        var result = Result<bool>.Failure("Body record not found");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBodyRecordCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateBodyRecord(recordId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateBodyRecord_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var recordId = Guid.NewGuid();
        var request = new UpdateBodyRecordRequest
        {
            Weight = 71.0m,
            BodyFatPercentage = 14.5m,
            RecordDate = DateTime.Today,
            Notes = "Updated progress"
        };

        // Act
        var response = await _controller.UpdateBodyRecord(recordId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task DeleteBodyRecord_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBodyRecordCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteBodyRecord(recordId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteBodyRecord_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var result = Result<bool>.Failure("Body record not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBodyRecordCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteBodyRecord(recordId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteBodyRecord_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var recordId = Guid.NewGuid();

        // Act
        var response = await _controller.DeleteBodyRecord(recordId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }
} 