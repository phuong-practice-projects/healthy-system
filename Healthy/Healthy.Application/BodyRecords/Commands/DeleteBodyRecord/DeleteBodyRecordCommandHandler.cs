using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Commands.DeleteBodyRecord;

public class DeleteBodyRecordCommandHandler : IRequestHandler<DeleteBodyRecordCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteBodyRecordCommandHandler> _logger;

    public DeleteBodyRecordCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteBodyRecordCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteBodyRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bodyRecord = await _context.BodyRecords
                .FirstOrDefaultAsync(br => br.Id == request.Id && br.UserId == request.UserId, cancellationToken);

            if (bodyRecord == null)
            {
                return Result<bool>.Failure("Body record not found or you don't have permission to delete it");
            }

            _context.BodyRecords.Remove(bodyRecord);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Body record {Id} deleted successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting body record {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while deleting the body record");
        }
    }
}
