namespace Healthy.Application.Common.Models
{
    /// <summary>
    /// Response model for exercise list with pagination
    /// </summary>
    public class ExerciseListResponse : BasePaginatedResponse<ExerciseDto>
    {
        /// <summary>
        /// List of exercises for the current page (alias for Items property)
        /// </summary>
        public List<ExerciseDto> Exercises 
        { 
            get => Items; 
            set => Items = value; 
        }

        /// <summary>
        /// Create an exercise list response with pagination info
        /// </summary>
        public static ExerciseListResponse Create(List<ExerciseDto> exercises, int totalItems, int currentPage, int pageSize)
        {
            var response = new ExerciseListResponse();
            response.SetPaginationData(exercises, totalItems, currentPage, pageSize);
            return response;
        }
    }
}
