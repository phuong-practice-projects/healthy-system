using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Columns.Queries.GetColumn;

public class GetColumnQuery : IRequest<Result<ColumnDto>>
{
    public Guid Id { get; set; }
}
