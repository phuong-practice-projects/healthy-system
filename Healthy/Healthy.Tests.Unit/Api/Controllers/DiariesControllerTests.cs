using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Healthy.Api.Controllers;
using Healthy.Application.Common.Models;
using Healthy.Application.UseCases.Diaries.Commands.CreateDiary;
using Healthy.Application.UseCases.Diaries.Commands.DeleteDiary;
using Healthy.Application.UseCases.Diaries.Commands.UpdateDiary;
using Healthy.Application.UseCases.Diaries.Queries.GetDiaries;
using Healthy.Application.UseCases.Diaries.Queries.GetDiary;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class DiariesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly DiariesController _controller;

    public DiariesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new DiariesController(_mediatorMock.Object);
        
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
    public async Task GetDiaries_WithDefaultParameters_ShouldReturnOkResult()
    {
        // Arrange
        var diaryListResponse = new DiaryListResponse
        {
            Items = new List<DiaryDto>(),
            TotalItems = 0,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDiariesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(diaryListResponse);

        // Act
        var result = await _controller.GetDiaries();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as DiaryListResponse;
        response.Should().NotBeNull();
        response!.Items.Should().NotBeNull();
        response.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetDiaries_WithCustomParameters_ShouldPassCorrectQueryToMediator()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today;
        var isPrivate = true;
        var searchTerm = "workout";
        var mood = "Happy";
        var tags = "fitness,health";
        var sortBy = "Mood";
        var sortDirection = "Asc";
        var page = 2;
        var pageSize = 5;

        var diaryListResponse = new DiaryListResponse
        {
            Items = new List<DiaryDto>(),
            TotalItems = 0,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDiariesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(diaryListResponse);

        // Act
        await _controller.GetDiaries(startDate, endDate, isPrivate, searchTerm, mood, tags, sortBy, sortDirection, page, pageSize);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<GetDiariesQuery>(q => 
                    q.StartDate == startDate &&
                    q.EndDate == endDate &&
                    q.IsPrivate == isPrivate &&
                    q.SearchTerm == searchTerm &&
                    q.Mood == mood &&
                    q.Tags == tags &&
                    q.SortBy == sortBy &&
                    q.SortDirection == sortDirection &&
                    q.Page == page &&
                    q.PageSize == pageSize),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDiaries_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        var result = await _controller.GetDiaries();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetDiaries_WithInvalidGuidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns("invalid-guid");

        // Act
        var result = await _controller.GetDiaries();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetDiaries_WithValidDiaries_ShouldReturnCorrectResponse()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var diaryDto = new DiaryDto
        {
            Id = diaryId,
            UserId = Guid.NewGuid(),
            Title = "Today's Workout",
            Content = "Had a great workout session today",
            Mood = "Happy",
            Tags = "fitness,workout",
            IsPrivate = false,
            DiaryDate = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };

        var diaryListResponse = new DiaryListResponse
        {
            Items = new List<DiaryDto> { diaryDto },
            TotalItems = 1,
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDiariesQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(diaryListResponse);

        // Act
        var result = await _controller.GetDiaries();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as DiaryListResponse;
        response.Should().NotBeNull();
        
        response!.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        
        var firstDiary = response.Items.First();
        firstDiary.Id.Should().Be(diaryId);
        firstDiary.Title.Should().Be("Today's Workout");
        firstDiary.Content.Should().Be("Had a great workout session today");
        firstDiary.Mood.Should().Be("Happy");
        firstDiary.Tags.Should().Be("fitness,workout");
        firstDiary.IsPrivate.Should().BeFalse();
        firstDiary.DiaryDate.Should().Be(DateTime.Today);

        response.TotalItems.Should().Be(1);
        response.CurrentPage.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetDiary_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var diaryDto = new DiaryDto
        {
            Id = diaryId,
            UserId = Guid.NewGuid(),
            Title = "Today's Workout",
            Content = "Had a great workout session today",
            Mood = "Happy",
            Tags = "fitness,workout",
            IsPrivate = false,
            DiaryDate = DateTime.Today,
            CreatedAt = DateTime.UtcNow
        };

        var result = Result<DiaryDto>.Success(diaryDto);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDiaryQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetDiary(diaryId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetDiary_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var result = Result<DiaryDto>.Failure("Diary not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDiaryQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.GetDiary(diaryId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetDiary_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var diaryId = Guid.NewGuid();

        // Act
        var response = await _controller.GetDiary(diaryId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task CreateDiary_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var request = new CreateDiaryRequest
        {
            Title = "Today's Workout",
            Content = "Had a great workout session today",
            Mood = "Happy",
            Tags = "fitness,workout",
            IsPrivate = false,
            DiaryDate = DateTime.Today
        };

        var result = Result<Guid>.Success(diaryId);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDiaryCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateDiary(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(DiariesController.GetDiary));
        createdResult.RouteValues!["id"].Should().Be(diaryId);
        createdResult.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateDiary_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateDiaryRequest
        {
            Title = "", // Invalid empty title
            Content = "Had a great workout session today",
            Mood = "Happy",
            Tags = "fitness,workout",
            IsPrivate = false,
            DiaryDate = DateTime.Today
        };

        var result = Result<Guid>.Failure("Title is required");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDiaryCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.CreateDiary(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreateDiary_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var request = new CreateDiaryRequest
        {
            Title = "Today's Workout",
            Content = "Had a great workout session today",
            Mood = "Happy",
            Tags = "fitness,workout",
            IsPrivate = false,
            DiaryDate = DateTime.Today
        };

        // Act
        var response = await _controller.CreateDiary(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateDiary_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var request = new UpdateDiaryRequest
        {
            Title = "Updated Workout",
            Content = "Updated workout session",
            Mood = "Excited",
            Tags = "fitness,workout,updated",
            IsPrivate = true,
            DiaryDate = DateTime.Today
        };

        var result = Result<bool>.Success(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDiaryCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateDiary(diaryId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateDiary_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var request = new UpdateDiaryRequest
        {
            Title = "Updated Workout",
            Content = "Updated workout session",
            Mood = "Excited",
            Tags = "fitness,workout,updated",
            IsPrivate = true,
            DiaryDate = DateTime.Today
        };

        var result = Result<bool>.Failure("Diary not found");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDiaryCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.UpdateDiary(diaryId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task UpdateDiary_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var diaryId = Guid.NewGuid();
        var request = new UpdateDiaryRequest
        {
            Title = "Updated Workout",
            Content = "Updated workout session",
            Mood = "Excited",
            Tags = "fitness,workout,updated",
            IsPrivate = true,
            DiaryDate = DateTime.Today
        };

        // Act
        var response = await _controller.UpdateDiary(diaryId, request);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task DeleteDiary_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var result = Result<bool>.Success(true);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDiaryCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteDiary(diaryId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<OkObjectResult>();
        var okResult = response.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteDiary_WithNotFoundId_ShouldReturnNotFound()
    {
        // Arrange
        var diaryId = Guid.NewGuid();
        var result = Result<bool>.Failure("Diary not found");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDiaryCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteDiary(diaryId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = response.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task DeleteDiary_WithInvalidUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var diaryId = Guid.NewGuid();

        // Act
        var response = await _controller.DeleteDiary(diaryId);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().BeOfType<UnauthorizedResult>();
    }
} 