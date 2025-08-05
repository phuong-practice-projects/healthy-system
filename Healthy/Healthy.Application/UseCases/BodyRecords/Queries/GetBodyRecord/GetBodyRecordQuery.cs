using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecord;

public class GetBodyRecordQuery : IRequest<Result<BodyRecordDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
