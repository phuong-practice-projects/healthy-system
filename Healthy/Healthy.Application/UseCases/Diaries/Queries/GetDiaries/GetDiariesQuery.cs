using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Diaries.Queries.GetDiaries;

/// <summary>
/// Query to get paginated list of diaries for a user with optional filters
/// </summary>
public record GetDiariesQuery : IRequest<DiaryListResponse>
{
    /// <summary>
    /// ID of the user whose diaries to retrieve
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Optional start date filter (inclusive)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Optional end date filter (inclusive)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Optional privacy filter
    /// </summary>
    public bool? IsPrivate { get; init; }

    /// <summary>
    /// Optional search term for diary title or content
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Optional mood filter
    /// </summary>
    public string? Mood { get; init; }

    /// <summary>
    /// Optional tags filter (comma-separated)
    /// </summary>
    public string? Tags { get; init; }

    /// <summary>
    /// Optional sorting field: "Date", "Title", "Mood"
    /// </summary>
    public string SortBy { get; init; } = "Date";

    /// <summary>
    /// Sort direction: "Asc" or "Desc"
    /// </summary>
    public string SortDirection { get; init; } = "Desc";

    /// <summary>
    /// Page number for pagination (starts from 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page (max 100)
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Validate and normalize pagination parameters
    /// </summary>
    public (int ValidPage, int ValidPageSize) GetValidatedPagination()
    {
        var validPage = Math.Max(1, Page);
        var validPageSize = Math.Max(1, Math.Min(100, PageSize));
        return (validPage, validPageSize);
    }
}
