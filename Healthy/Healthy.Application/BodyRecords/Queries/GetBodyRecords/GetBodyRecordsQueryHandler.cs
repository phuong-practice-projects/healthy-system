using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Healthy.Application.BodyRecords.Queries.GetBodyRecord;

namespace Healthy.Application.BodyRecords.Queries.GetBodyRecords;

public class GetBodyRecordsQueryHandler : IRequestHandler<GetBodyRecordsQuery, BodyRecordsListResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetBodyRecordsQueryHandler> _logger;

    public GetBodyRecordsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetBodyRecordsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BodyRecordsListResponse> Handle(GetBodyRecordsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.BodyRecords
                .Where(br => br.UserId == request.UserId);

            if (request.StartDate.HasValue)
            {
                query = query.Where(br => br.RecordDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(br => br.RecordDate <= request.EndDate.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

            var bodyRecords = await query
                .OrderByDescending(br => br.RecordDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(br => new BodyRecordDto
                {
                    Id = br.Id,
                    UserId = br.UserId,
                    Weight = br.Weight,
                    BodyFatPercentage = br.BodyFatPercentage,
                    RecordDate = br.RecordDate,
                    Notes = br.Notes,
                    CreatedAt = br.CreatedAt,
                    UpdatedAt = br.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return new BodyRecordsListResponse
            {
                BodyRecords = bodyRecords,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving body records for user {UserId}", request.UserId);
            return new BodyRecordsListResponse
            {
                BodyRecords = new List<BodyRecordDto>(),
                TotalItems = 0,
                TotalPages = 0,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
