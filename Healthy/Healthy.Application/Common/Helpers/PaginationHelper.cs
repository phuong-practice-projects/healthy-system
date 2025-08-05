namespace Healthy.Application.Common.Helpers;

/// <summary>
/// Helper class for pagination calculations
/// </summary>
public static class PaginationHelper
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
    /// Check if there is a previous page
    /// </summary>
    public static bool HasPreviousPage(int currentPage)
    {
        return currentPage > 1;
    }

    /// <summary>
    /// Check if there is a next page
    /// </summary>
    public static bool HasNextPage(int currentPage, int totalPages)
    {
        return currentPage < totalPages;
    }

    /// <summary>
    /// Get first item index (1-based for display)
    /// </summary>
    public static int GetFirstItemIndex(int currentPage, int pageSize, int totalItems)
    {
        return totalItems == 0 ? 0 : (currentPage - 1) * pageSize + 1;
    }

    /// <summary>
    /// Get last item index (1-based for display)
    /// </summary>
    public static int GetLastItemIndex(int currentPage, int pageSize, int totalItems)
    {
        var firstIndex = GetFirstItemIndex(currentPage, pageSize, totalItems);
        return Math.Min(firstIndex + pageSize - 1, totalItems);
    }
}
