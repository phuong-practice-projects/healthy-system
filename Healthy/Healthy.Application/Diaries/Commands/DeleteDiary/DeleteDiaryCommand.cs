using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Diaries.Commands.DeleteDiary;

public class DeleteDiaryCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
