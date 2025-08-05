using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Healthy.Application.UseCases.BodyRecords.Queries.GetBodyRecords;

public class GetBodyRecordsQueryHandler : 
    IRequestHandler<GetBodyRecordsQuery, BodyRecordsListResponse>,
    IRequestHandler<GetBodyRecordsQueryRequest, BodyRecordsListResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetBodyRecordsQueryHandler> _logger;

    public GetBodyRecordsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetBodyRecordsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BodyRecordsListResponse> Handle(GetBodyRecordsQuery request, CancellationToken cancellationToken)
    {
        return await HandleInternal(
            request.UserId,
            request.StartDate,
            request.EndDate,
            request.MinWeight,
            request.MaxWeight,
            request.MinBodyFat,
            request.MaxBodyFat,
            request.SortBy,
            request.SortDirection,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    public async Task<BodyRecordsListResponse> Handle(GetBodyRecordsQueryRequest request, CancellationToken cancellationToken)
    {
        return await HandleInternal(
            request.UserId,
            request.StartDate,
            request.EndDate,
            request.MinWeight,
            request.MaxWeight,
            request.MinBodyFat,
            request.MaxBodyFat,
            request.SortBy,
            request.SortDirection,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private async Task<BodyRecordsListResponse> HandleInternal(
        Guid userId,
        DateTime? startDate,
        DateTime? endDate,
        decimal? minWeight,
        decimal? maxWeight,
        decimal? minBodyFat,
        decimal? maxBodyFat,
        string sortBy,
        string sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.BodyRecords
                .Where(br => br.UserId.ToString().ToLower() == userId.ToString().ToLower());

            if (startDate.HasValue)
            {
                query = query.Where(br => br.RecordDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(br => br.RecordDate <= endDate.Value);
            }

            if (minWeight.HasValue)
            {
                query = query.Where(br => br.Weight >= minWeight.Value);
            }

            if (maxWeight.HasValue)
            {
                query = query.Where(br => br.Weight <= maxWeight.Value);
            }

            if (minBodyFat.HasValue)
            {
                query = query.Where(br => br.BodyFatPercentage >= minBodyFat.Value);
            }

            if (maxBodyFat.HasValue)
            {
                query = query.Where(br => br.BodyFatPercentage <= maxBodyFat.Value);
            }

            // Apply sorting (only available fields)
            query = sortBy.ToUpper() switch
            {
                "WEIGHT" => sortDirection.ToUpper() == "ASC" 
                    ? query.OrderBy(br => br.Weight) 
                    : query.OrderByDescending(br => br.Weight),
                "BODYFAT" => sortDirection.ToUpper() == "ASC" 
                    ? query.OrderBy(br => br.BodyFatPercentage) 
                    : query.OrderByDescending(br => br.BodyFatPercentage),
                _ => sortDirection.ToUpper() == "ASC" 
                    ? query.OrderBy(br => br.RecordDate) 
                    : query.OrderByDescending(br => br.RecordDate)
            };

            var totalItems = await query.CountAsync(cancellationToken);
            var validPage = Math.Max(1, page);
            var validPageSize = Math.Max(1, Math.Min(100, pageSize));

            var bodyRecords = await query
                .Skip((validPage - 1) * validPageSize)
                .Take(validPageSize)
                .Select(br => new BodyRecordDto
                {
                    Id = br.Id,
                    UserId = br.UserId,
                    Weight = br.Weight,
                    BodyFatPercentage = br.BodyFatPercentage,
                    RecordDate = br.RecordDate,
                    Notes = br.Notes,
                    CreatedAt = br.CreatedAt,
                    UpdatedAt = br.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return BodyRecordsListResponse.Create(bodyRecords, totalItems, validPage, validPageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving body records for user {UserId}", userId);
            return BodyRecordsListResponse.Create(new List<BodyRecordDto>(), 0, 1, pageSize);
        }
    }
}
