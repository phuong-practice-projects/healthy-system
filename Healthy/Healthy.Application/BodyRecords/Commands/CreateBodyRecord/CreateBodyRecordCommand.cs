using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Commands.CreateBodyRecord;

public record CreateBodyRecordCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public decimal Weight { get; init; }
    public decimal? BodyFatPercentage { get; init; }
    public DateTime RecordDate { get; init; }
    public string? Notes { get; init; }
}
