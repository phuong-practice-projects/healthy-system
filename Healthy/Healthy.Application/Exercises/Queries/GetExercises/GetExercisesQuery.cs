using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Exercises.Queries.GetExercises;

/// <summary>
/// Query to get paginated list of exercises for a user with optional filters
/// </summary>
public record GetExercisesQuery : IRequest<ExerciseListResponse>
{
    /// <summary>
    /// ID of the user whose exercises to retrieve
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
    /// Optional exercise category filter
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Optional search term for exercise title or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Minimum duration filter (in minutes)
    /// </summary>
    public int? MinDuration { get; init; }

    /// <summary>
    /// Maximum duration filter (in minutes)
    /// </summary>
    public int? MaxDuration { get; init; }

    /// <summary>
    /// Minimum calories burned filter
    /// </summary>
    public int? MinCalories { get; init; }

    /// <summary>
    /// Maximum calories burned filter
    /// </summary>
    public int? MaxCalories { get; init; }

    /// <summary>
    /// Optional sorting field: "Date", "Duration", "Calories", "Title", "Category"
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
