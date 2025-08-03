using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Queries.GetDiary;

public class GetDiaryQueryHandler : IRequestHandler<GetDiaryQuery, Result<DiaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDiaryQueryHandler> _logger;

    public GetDiaryQueryHandler(
        IApplicationDbContext context,
        ILogger<GetDiaryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<DiaryDto>> Handle(GetDiaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == request.UserId, cancellationToken);

            if (diary == null)
            {
                return Result<DiaryDto>.Failure("Diary not found or you don't have permission to view it");
            }

            var dto = new DiaryDto
            {
                Id = diary.Id,
                UserId = diary.UserId,
                Title = diary.Title,
                Content = diary.Content,
                Tags = diary.Tags,
                Mood = diary.Mood,
                IsPrivate = diary.IsPrivate,
                DiaryDate = diary.DiaryDate,
                CreatedAt = diary.CreatedAt,
                UpdatedAt = diary.UpdatedAt
            };

            return Result<DiaryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving diary {Id} for user {UserId}", request.Id, request.UserId);
            return Result<DiaryDto>.Failure("An error occurred while retrieving the diary");
        }
    }
}
