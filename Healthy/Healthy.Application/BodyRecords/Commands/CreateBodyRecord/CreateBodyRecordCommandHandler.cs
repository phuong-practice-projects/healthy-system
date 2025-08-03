using MediatR;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Domain.Entities;

namespace Healthy.Application.BodyRecords.Commands.CreateBodyRecord;

public class CreateBodyRecordCommandHandler : IRequestHandler<CreateBodyRecordCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly ILogger<CreateBodyRecordCommandHandler> _logger;

    public CreateBodyRecordCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        ILogger<CreateBodyRecordCommandHandler> logger)
    {
        _context = context;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateBodyRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bodyRecord = new BodyRecord
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Weight = request.Weight,
                BodyFatPercentage = request.BodyFatPercentage,
                RecordDate = request.RecordDate,
                Notes = request.Notes,
                CreatedAt = _dateTime.UtcNow,
                IsDeleted = false
            };

            _context.BodyRecords.Add(bodyRecord);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created body record {Id} for user {UserId}", bodyRecord.Id, request.UserId);

            return Result<Guid>.Success(bodyRecord.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating body record for user {UserId}", request.UserId);
            return Result<Guid>.Failure("Failed to create body record");
        }
    }
}
