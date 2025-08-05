using MediatR;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.UseCases.Diaries.Commands.CreateDiary;

public class CreateDiaryCommandHandler : IRequestHandler<CreateDiaryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly ILogger<CreateDiaryCommandHandler> _logger;

    public CreateDiaryCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        ILogger<CreateDiaryCommandHandler> logger)
    {
        _context = context;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateDiaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var diary = new Diary
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title,
                Content = request.Content,
                Tags = request.Tags,
                Mood = request.Mood,
                IsPrivate = request.IsPrivate,
                DiaryDate = request.DiaryDate,
                CreatedAt = _dateTime.UtcNow,
            };

            _context.Diaries.Add(diary);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created diary {Id} for user {UserId}", diary.Id, request.UserId);

            return Result<Guid>.Success(diary.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating diary for user {UserId}", request.UserId);
            return Result<Guid>.Failure("Failed to create diary");
        }
    }
}
