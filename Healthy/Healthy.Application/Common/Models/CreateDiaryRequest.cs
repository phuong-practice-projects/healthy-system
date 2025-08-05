namespace Healthy.Application.Common.Models;

public class CreateDiaryRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public string? Mood { get; set; }
    public bool IsPrivate { get; set; } = true;
    public DateTime DiaryDate { get; set; }
}
