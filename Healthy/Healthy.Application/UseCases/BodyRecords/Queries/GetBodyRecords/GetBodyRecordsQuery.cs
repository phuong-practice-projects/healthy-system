using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecords;

/// <summary>
/// Query to get paginated list of body records for a user with optional filters
/// </summary>
public record GetBodyRecordsQuery : IRequest<BodyRecordsListResponse>
{
    /// <summary>
    /// ID of the user whose body records to retrieve
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
    /// Optional weight range filter - minimum weight
    /// </summary>
    public decimal? MinWeight { get; init; }

    /// <summary>
    /// Optional weight range filter - maximum weight
    /// </summary>
    public decimal? MaxWeight { get; init; }

    /// <summary>
    /// Optional BMI range filter - minimum BMI
    /// </summary>
    public decimal? MinBMI { get; init; }

    /// <summary>
    /// Optional BMI range filter - maximum BMI
    /// </summary>
    public decimal? MaxBMI { get; init; }

    /// <summary>
    /// Optional body fat percentage range - minimum
    /// </summary>
    public decimal? MinBodyFat { get; init; }

    /// <summary>
    /// Optional body fat percentage range - maximum
    /// </summary>
    public decimal? MaxBodyFat { get; init; }

    /// <summary>
    /// Optional sorting field: "Date", "Weight", "BMI", "Height", "BodyFat"
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
