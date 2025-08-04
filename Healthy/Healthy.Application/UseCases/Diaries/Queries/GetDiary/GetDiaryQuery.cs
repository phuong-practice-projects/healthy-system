using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Diaries.Queries.GetDiary;

public class GetDiaryQuery : IRequest<Result<DiaryDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
