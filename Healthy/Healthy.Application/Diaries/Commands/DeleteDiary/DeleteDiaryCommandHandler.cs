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
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (diary == null)
            {
                return Result<bool>.Failure("Diary not found or you don't have permission to delete it");
            }

            if (diary.DeletedAt.HasValue)
            {
                return Result<bool>.Failure("Diary is already deleted");
            }

            // Soft delete the diary
            diary.Delete(); // This will set DeletedAt and DeletedBy
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Diary {Id} soft deleted successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting diary {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while deleting the diary");
        }
    }
}
