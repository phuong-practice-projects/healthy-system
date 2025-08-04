using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Columns.Commands.DeleteColumn;

public class DeleteColumnCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
