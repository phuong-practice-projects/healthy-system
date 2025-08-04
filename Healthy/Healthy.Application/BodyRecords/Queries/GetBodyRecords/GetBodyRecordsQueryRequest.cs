using MediatR;
using Healthy.Application.Common.Models;

namespace Healthy.Application.BodyRecords.Queries.GetBodyRecords;

/// <summary>
/// Base query model for body records with specific filters
/// </summary>
public record GetBodyRecordsQueryRequest : BasePagedQuery, IRequest<BodyRecordsListResponse>
{
    /// <summary>
    /// Optional weight range filter - minimum weight
    /// </summary>
    public decimal? MinWeight { get; init; }

    /// <summary>
    /// Optional weight range filter - maximum weight
    /// </summary>
    public decimal? MaxWeight { get; init; }

    /// <summary>
    /// Optional body fat percentage range - minimum
    /// </summary>
    public decimal? MinBodyFat { get; init; }

    /// <summary>
    /// Optional body fat percentage range - maximum
    /// </summary>
    public decimal? MaxBodyFat { get; init; }

    // Computed properties specific to body records
    /// <summary>
    /// Gets whether there is a weight range filter
    /// </summary>
    public bool HasWeightFilter => MinWeight.HasValue || MaxWeight.HasValue;

    /// <summary>
    /// Gets whether there is a body fat filter
    /// </summary>
    public bool HasBodyFatFilter => MinBodyFat.HasValue || MaxBodyFat.HasValue;

    /// <summary>
    /// Gets whether any numeric filters are applied
    /// </summary>
    public bool HasNumericFilters => HasWeightFilter || HasBodyFatFilter;

    /// <summary>
    /// Create from individual parameters for easy controller binding
    /// </summary>
    public static GetBodyRecordsQueryRequest Create(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minWeight = null,
        decimal? maxWeight = null,
        decimal? minBodyFat = null,
        decimal? maxBodyFat = null,
        string? searchTerm = null,
        string sortBy = "Date",
        string sortDirection = "Desc",
        int page = 1,
        int pageSize = 10)
    {
        return new GetBodyRecordsQueryRequest
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            MinWeight = minWeight,
            MaxWeight = maxWeight,
            MinBodyFat = minBodyFat,
            MaxBodyFat = maxBodyFat,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        };
    }
}
