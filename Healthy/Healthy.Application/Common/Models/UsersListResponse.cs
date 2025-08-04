using Healthy.Application.Common.Models;

namespace Healthy.Application.Common.Models
{
    /// <summary>
    /// Response model for users list with pagination
    /// </summary>
    public class UsersListResponse : BasePaginatedResponse<UserDto>
    {
        /// <summary>
        /// List of users for the current page (alias for Items property)
        /// </summary>
        public List<UserDto> Users 
        { 
            get => Items; 
            set => Items = value; 
        }

        /// <summary>
        /// Create a users list response with pagination info
        /// </summary>
        public static UsersListResponse Create(List<UserDto> users, int totalItems, int currentPage, int pageSize)
        {
            var response = new UsersListResponse();
            response.SetPaginationData(users, totalItems, currentPage, pageSize);
            return response;
        }
    }
}
