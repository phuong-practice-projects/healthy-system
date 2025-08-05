using System.ComponentModel.DataAnnotations;

namespace Healthy.Application.Common.Models;

public class DiaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public string? Mood { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime DiaryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Timestamp => DiaryDate.ToString("yyyy.MM.dd HH:mm");
}
