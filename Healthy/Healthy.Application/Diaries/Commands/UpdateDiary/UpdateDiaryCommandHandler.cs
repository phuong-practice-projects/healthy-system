using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Commands.UpdateDiary;

public class UpdateDiaryCommandHandler : IRequestHandler<UpdateDiaryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateDiaryCommandHandler> _logger;

    public UpdateDiaryCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateDiaryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateDiaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (diary == null)
            {
                return Result<bool>.Failure("Diary not found or you don't have permission to update it");
            }

            diary.Title = request.Title;
            diary.Content = request.Content;
            diary.Tags = request.Tags;
            diary.Mood = request.Mood;
            diary.IsPrivate = request.IsPrivate;
            diary.DiaryDate = request.DiaryDate;
            diary.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Diary {Id} updated successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating diary {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while updating the diary");
        }
    }
}
