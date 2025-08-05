using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Columns.Queries.GetColumns;

public class GetColumnsQueryHandler : IRequestHandler<GetColumnsQuery, PaginatedList<ColumnDto>>
{
    private readonly IApplicationDbContext _context;

    public GetColumnsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ColumnDto>> Handle(GetColumnsQuery request, CancellationToken cancellationToken)
    {
        // Start with all columns - global query filter handles IsPublished and IsDeleted filtering
        var query = _context.Columns.AsQueryable();

        // Apply category filter if provided
        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(c => c.Category == request.Category);
        }

        // Apply sorting by CreatedAt descending (most recent first)
        query = query.OrderByDescending(c => c.CreatedAt);

        // Convert to ColumnDto query
        var columnDtoQuery = query.Select(c => new ColumnDto
        {
            Id = c.Id,
            Title = c.Title,
            Content = c.Content,
            Category = c.Category,
            ImageUrl = c.ImageUrl,
            Tags = c.Tags,
            IsPublished = c.IsPublished,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        // Use the existing PaginatedList.CreateAsync method
        return await PaginatedList<ColumnDto>.CreateAsync(
            columnDtoQuery, 
            request.Page, 
            request.Limit);
    }
}
