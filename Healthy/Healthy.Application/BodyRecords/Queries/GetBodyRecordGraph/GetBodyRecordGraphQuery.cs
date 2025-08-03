using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Queries.GetBodyRecordGraph;

public record GetBodyRecordGraphQuery : IRequest<BodyRecordGraphResponse>
{
    public Guid UserId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
