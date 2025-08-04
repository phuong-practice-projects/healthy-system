using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? CategoryType { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; } = true;
}
