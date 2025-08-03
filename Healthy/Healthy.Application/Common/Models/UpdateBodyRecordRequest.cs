namespace Healthy.Application.Common.Models
{
    public class UpdateBodyRecordRequest
    {
        public decimal Weight { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public DateTime RecordDate { get; set; }
        public string? Notes { get; set; }
    }
}
