using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Diaries.Commands.CreateDiary;

public record CreateDiaryCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Tags { get; init; }
    public string? Mood { get; init; }
    public bool IsPrivate { get; init; } = true;
    public DateTime DiaryDate { get; init; }
}
