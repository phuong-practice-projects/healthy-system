using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Queries.GetBodyRecords;

public class GetBodyRecordsQuery : IRequest<BodyRecordsListResponse>
{
    public Guid UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
