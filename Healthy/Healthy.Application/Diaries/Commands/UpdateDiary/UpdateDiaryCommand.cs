using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Commands.UpdateDiary;

public class UpdateDiaryCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public string? Mood { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime DiaryDate { get; set; }
}
