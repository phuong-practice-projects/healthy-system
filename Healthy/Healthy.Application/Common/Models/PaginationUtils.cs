namespace Healthy.Application.Common.Models;

/// <summary>
/// Interface for paginated responses
/// </summary>
public interface IPaginatedResponse
{
    int TotalItems { get; set; }
    int TotalPages { get; set; }
    int CurrentPage { get; set; }
    int PageSize { get; set; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}

/// <summary>
/// Static utility class for pagination operations
/// </summary>
public static class PaginationUtils
{
    /// <summary>
    /// Calculate total pages from total items and page size
    /// </summary>
    public static int CalculateTotalPages(int totalItems, int pageSize)
    {
        return totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);
    }

    /// <summary>
    /// Validate and normalize page number
    /// </summary>
    public static int ValidatePage(int page)
    {
        return Math.Max(1, page);
    }

    /// <summary>
    /// Validate and normalize page size
    /// </summary>
    public static int ValidatePageSize(int pageSize)
    {
        return Math.Max(1, Math.Min(100, pageSize));
    }

    /// <summary>
    /// Get validated pagination parameters
    /// </summary>
    public static (int ValidPage, int ValidPageSize) GetValidatedPagination(int page, int pageSize)
    {
        return (ValidatePage(page), ValidatePageSize(pageSize));
    }

    /// <summary>
    /// Calculate skip count for database queries
    /// </summary>
    public static int CalculateSkip(int page, int pageSize)
    {
        return (ValidatePage(page) - 1) * ValidatePageSize(pageSize);
    }

    /// <summary>
    /// Create pagination info object
    /// </summary>
    public static PaginationInfo CreatePaginationInfo(int totalItems, int currentPage, int pageSize)
    {
        return new PaginationInfo
        {
            TotalItems = totalItems,
            CurrentPage = ValidatePage(currentPage),
            PageSize = ValidatePageSize(pageSize),
            TotalPages = CalculateTotalPages(totalItems, pageSize)
        };
    }
}

/// <summary>
/// Pagination information container
/// </summary>
public class PaginationInfo
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int FirstItemIndex => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
    public int LastItemIndex => Math.Min(FirstItemIndex + PageSize - 1, TotalItems);
}
