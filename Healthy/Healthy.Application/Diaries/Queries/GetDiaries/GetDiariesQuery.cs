using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Queries.GetDiaries;

public record GetDiariesQuery : IRequest<DiaryListResponse>
{
    public Guid UserId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsPrivate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
