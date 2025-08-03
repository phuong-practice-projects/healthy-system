using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Queries.GetBodyRecordGraph;

public class GetBodyRecordGraphQueryHandler : IRequestHandler<GetBodyRecordGraphQuery, BodyRecordGraphResponse>
{
    private readonly IApplicationDbContext _context;

    public GetBodyRecordGraphQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BodyRecordGraphResponse> Handle(GetBodyRecordGraphQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BodyRecords
            .Where(br => br.UserId == request.UserId && !br.IsDeleted);

        if (request.StartDate.HasValue)
        {
            query = query.Where(br => br.RecordDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(br => br.RecordDate <= request.EndDate.Value);
        }

        var bodyRecords = await query
            .OrderBy(br => br.RecordDate)
            .Select(br => new BodyRecordGraphData
            {
                Date = br.RecordDate,
                Weight = br.Weight,
                BodyFat = br.BodyFatPercentage
            })
            .ToListAsync(cancellationToken);

        return new BodyRecordGraphResponse
        {
            GraphData = bodyRecords
        };
    }
}
