namespace Healthy.Application.Common.Models
{
    public class ExerciseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int? CaloriesBurned { get; set; }
        public DateTime ExerciseDate { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Duration => $"{DurationMinutes} min";
        public string Calories => $"{CaloriesBurned ?? 0}kcal";
    }
}
