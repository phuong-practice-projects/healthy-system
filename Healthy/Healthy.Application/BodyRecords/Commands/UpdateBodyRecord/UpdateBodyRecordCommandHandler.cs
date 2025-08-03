using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Commands.UpdateBodyRecord;

public class UpdateBodyRecordCommandHandler : IRequestHandler<UpdateBodyRecordCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateBodyRecordCommandHandler> _logger;

    public UpdateBodyRecordCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateBodyRecordCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateBodyRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bodyRecord = await _context.BodyRecords
                .FirstOrDefaultAsync(br => br.Id == request.Id && br.UserId == request.UserId, cancellationToken);

            if (bodyRecord == null)
            {
                return Result<bool>.Failure("Body record not found or you don't have permission to update it");
            }

            bodyRecord.Weight = request.Weight;
            bodyRecord.BodyFatPercentage = request.BodyFatPercentage;
            bodyRecord.RecordDate = request.RecordDate;
            bodyRecord.Notes = request.Notes;
            bodyRecord.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Body record {Id} updated successfully for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating body record {Id} for user {UserId}", request.Id, request.UserId);
            return Result<bool>.Failure("An error occurred while updating the body record");
        }
    }
}
