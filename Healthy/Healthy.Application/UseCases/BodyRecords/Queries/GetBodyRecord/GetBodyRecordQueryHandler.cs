using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecord;

public class GetBodyRecordQueryHandler : IRequestHandler<GetBodyRecordQuery, Result<BodyRecordDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetBodyRecordQueryHandler> _logger;

    public GetBodyRecordQueryHandler(
        IApplicationDbContext context,
        ILogger<GetBodyRecordQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<BodyRecordDto>> Handle(GetBodyRecordQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var bodyRecord = await _context.BodyRecords
                .FirstOrDefaultAsync(br => br.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (bodyRecord == null)
            {
                return Result<BodyRecordDto>.Failure("Body record not found or you don't have permission to view it");
            }

            var dto = new BodyRecordDto
            {
                Id = bodyRecord.Id,
                UserId = bodyRecord.UserId,
                Weight = bodyRecord.Weight,
                BodyFatPercentage = bodyRecord.BodyFatPercentage,
                RecordDate = bodyRecord.RecordDate,
                Notes = bodyRecord.Notes,
                CreatedAt = bodyRecord.CreatedAt,
                UpdatedAt = bodyRecord.UpdatedAt
            };

            return Result<BodyRecordDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving body record {Id} for user {UserId}", request.Id, request.UserId);
            return Result<BodyRecordDto>.Failure("An error occurred while retrieving the body record");
        }
    }
}
