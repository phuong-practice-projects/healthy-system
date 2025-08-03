namespace Healthy.Application.Common.Models;

public class UpdateDiaryRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public string? Mood { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime DiaryDate { get; set; }
}
