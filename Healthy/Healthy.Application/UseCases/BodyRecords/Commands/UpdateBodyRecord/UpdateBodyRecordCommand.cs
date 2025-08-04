using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.BodyRecords.Commands.UpdateBodyRecord;

public class UpdateBodyRecordCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Weight { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public DateTime RecordDate { get; set; }
    public string? Notes { get; set; }
}
