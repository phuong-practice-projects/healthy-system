namespace Healthy.Application.Common.Models
{
    public class UpdateExerciseRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public DateTime ExerciseDate { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
    }
}
