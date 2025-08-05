namespace Healthy.Application.Common.Models;

/// <summary>
/// Response model for diary list with pagination
/// </summary>
public class DiaryListResponse : BasePaginatedResponse<DiaryDto>
{
    /// <summary>
    /// List of diaries for the current page (alias for Items property)
    /// </summary>
    public List<DiaryDto> Diaries 
    { 
        get => Items; 
        set => Items = value; 
    }

    /// <summary>
    /// Create a diary list response with pagination info
    /// </summary>
    public static DiaryListResponse Create(List<DiaryDto> diaries, int totalItems, int currentPage, int pageSize)
    {
        var response = new DiaryListResponse();
        response.SetPaginationData(diaries, totalItems, currentPage, pageSize);
        return response;
    }
}
