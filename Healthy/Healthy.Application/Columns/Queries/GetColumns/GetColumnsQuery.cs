using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Columns.Queries.GetColumns;

public record GetColumnsQuery : IRequest<PaginatedList<ColumnDto>>
{
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = 10;
    public string? Category { get; init; }
}
