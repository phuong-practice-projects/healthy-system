using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.BodyRecords.Commands.DeleteBodyRecord;

public class DeleteBodyRecordCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
