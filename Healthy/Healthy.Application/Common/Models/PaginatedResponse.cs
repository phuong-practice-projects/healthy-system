namespace Healthy.Application.Common.Models;

/// <summary>
/// Base paginated response with common pagination properties
/// </summary>
public abstract class BasePaginatedResponse
{
    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Index of first item on current page (1-based for display)
    /// </summary>
    public int FirstItemIndex => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;

    /// <summary>
    /// Index of last item on current page (1-based for display)
    /// </summary>
    public int LastItemIndex => Math.Min(FirstItemIndex + PageSize - 1, TotalItems);

    /// <summary>
    /// Set pagination info
    /// </summary>
    protected void SetPaginationInfo(int totalItems, int currentPage, int pageSize)
    {
        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);
    }
}

/// <summary>
/// Generic paginated response model that can be used for any list response
/// </summary>
/// <typeparam name="T">Type of data items</typeparam>
public class PaginatedResponse<T> : BasePaginatedResponse
{
    /// <summary>
    /// List of data items for current page
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Create a paginated response
    /// </summary>
    /// <param name="items">Data items</param>
    /// <param name="totalItems">Total number of items</param>
    /// <param name="currentPage">Current page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated response</returns>
    public static PaginatedResponse<T> Create(List<T> items, int totalItems, int currentPage, int pageSize)
    {
        var response = new PaginatedResponse<T> { Items = items };
        response.SetPaginationInfo(totalItems, currentPage, pageSize);
        return response;
    }

    /// <summary>
    /// Create an empty paginated response
    /// </summary>
    /// <param name="currentPage">Current page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Empty paginated response</returns>
    public static PaginatedResponse<T> Empty(int currentPage = 1, int pageSize = 10)
    {
        var response = new PaginatedResponse<T> { Items = new List<T>() };
        response.SetPaginationInfo(0, currentPage, pageSize);
        return response;
    }
}
