namespace Healthy.Application.Common.Models;

/// <summary>
/// Generic base paginated response model that can be inherited by all list responses
/// </summary>
/// <typeparam name="T">Type of the data items</typeparam>
public class BasePaginatedResponse<T>
{
    /// <summary>
    /// List of data items for current page
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

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
    /// Set pagination information with validation
    /// </summary>
    /// <param name="items">Data items</param>
    /// <param name="totalItems">Total number of items</param>
    /// <param name="currentPage">Current page number</param>
    /// <param name="pageSize">Page size</param>
    protected void SetPaginationData(List<T> items, int totalItems, int currentPage, int pageSize)
    {
        // Validate parameters
        var validPage = Math.Max(1, currentPage);
        var validPageSize = Math.Max(1, Math.Min(100, pageSize));
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / validPageSize);

        Items = items ?? new List<T>();
        TotalItems = Math.Max(0, totalItems);
        TotalPages = totalPages;
        CurrentPage = validPage;
        PageSize = validPageSize;
    }
}
