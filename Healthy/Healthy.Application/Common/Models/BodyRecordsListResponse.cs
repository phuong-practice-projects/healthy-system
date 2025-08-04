namespace Healthy.Application.Common.Models
{
    /// <summary>
    /// Response model for body records list with pagination - inherits from generic base
    /// </summary>
    public class BodyRecordsListResponse : BasePaginatedResponse<BodyRecordDto>
    {
        /// <summary>
        /// List of body records - provides backward compatibility alias for Items
        /// </summary>
        public List<BodyRecordDto> BodyRecords 
        { 
            get => Items; 
            set => Items = value; 
        }

        /// <summary>
        /// Create a body records list response with pagination info
        /// </summary>
        public static BodyRecordsListResponse Create(List<BodyRecordDto> bodyRecords, int totalItems, int currentPage, int pageSize)
        {
            var response = new BodyRecordsListResponse();
            response.SetPaginationData(bodyRecords, totalItems, currentPage, pageSize);
            return response;
        }
    }
}
