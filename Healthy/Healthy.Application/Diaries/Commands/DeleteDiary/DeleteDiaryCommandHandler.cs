using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Commands.DeleteDiary;

public class DeleteDiaryCommandHandler : IRequestHandler<DeleteDiaryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteDiaryCommandHandler> _logger;

    public DeleteDiaryCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteDiaryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteDiaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == request.UserId, cancellationToken);

            if (diary == null)
            {
                return Result<bool>.Failure("Diary not found or you don't have permission to delete it");
            }

            _context.Diaries.Remove(diary);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Diary {Id} deleted successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting diary {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while deleting the diary");
        }
    }
}
