namespace Healthy.Application.Common.Models;

public class CreateColumnRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; } = true;
}
