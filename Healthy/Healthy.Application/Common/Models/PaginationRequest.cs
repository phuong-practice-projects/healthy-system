namespace Healthy.Application.Common.Models;

/// <summary>
/// Base pagination request parameters
/// </summary>
public class PaginationRequest
{
    private int _page = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (minimum: 1, default: 1)
    /// </summary>
    public int Page 
    { 
        get => _page; 
        set => _page = value < 1 ? 1 : value; 
    }

    /// <summary>
    /// Page size (minimum: 1, maximum: 100, default: 10)
    /// </summary>
    public int PageSize 
    { 
        get => _pageSize; 
        set => _pageSize = value switch
        {
            < 1 => 1,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Calculate skip count for database queries
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Take count for database queries
    /// </summary>
    public int Take => PageSize;
}
