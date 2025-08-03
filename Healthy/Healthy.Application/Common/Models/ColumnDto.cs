namespace Healthy.Application.Common.Models;

public class ColumnDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
