using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Columns.Commands.UpdateColumn;

public class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateColumnCommandHandler> _logger;

    public UpdateColumnCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateColumnCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (column == null)
            {
                return Result<bool>.Failure("Column not found");
            }

            column.Title = request.Title;
            column.Content = request.Content;
            column.Category = request.Category;
            column.ImageUrl = request.ImageUrl;
            column.Tags = request.Tags;
            column.IsPublished = request.IsPublished;
            column.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Column {Id} updated successfully", request.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating column {Id}", request.Id);
            return Result<bool>.Failure("An error occurred while updating the column");
        }
    }
}
