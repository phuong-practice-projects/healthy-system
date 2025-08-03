using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Columns.Commands.UpdateColumn;

public class UpdateColumnCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; }
}
