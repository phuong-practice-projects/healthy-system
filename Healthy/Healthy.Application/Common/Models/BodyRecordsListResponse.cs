namespace Healthy.Application.Common.Models
{
    public class BodyRecordsListResponse
    {
        public List<BodyRecordDto> BodyRecords { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
