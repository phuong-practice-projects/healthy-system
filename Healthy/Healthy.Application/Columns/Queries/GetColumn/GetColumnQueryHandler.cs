using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Columns.Queries.GetColumn;

public class GetColumnQueryHandler : IRequestHandler<GetColumnQuery, Result<ColumnDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetColumnQueryHandler> _logger;

    public GetColumnQueryHandler(
        IApplicationDbContext context,
        ILogger<GetColumnQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<ColumnDto>> Handle(GetColumnQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (column == null)
            {
                return Result<ColumnDto>.Failure("Column not found");
            }

            var dto = new ColumnDto
            {
                Id = column.Id,
                Title = column.Title,
                Content = column.Content,
                Category = column.Category,
                ImageUrl = column.ImageUrl,
                Tags = column.Tags,
                IsPublished = column.IsPublished,
                CreatedAt = column.CreatedAt,
                UpdatedAt = column.UpdatedAt
            };

            return Result<ColumnDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving column {Id}", request.Id);
            return Result<ColumnDto>.Failure("An error occurred while retrieving the column");
        }
    }
}
