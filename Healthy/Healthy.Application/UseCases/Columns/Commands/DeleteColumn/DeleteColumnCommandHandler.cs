using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.UseCases.Columns.Commands.DeleteColumn;

public class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteColumnCommandHandler> _logger;

    public DeleteColumnCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteColumnCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var column = await _context.Columns.FirstOrDefaultAsync(c => c.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (column == null)
            {
                return Result<bool>.Failure("Column not found");
            }

            if (column.DeletedAt.HasValue)
            {
                return Result<bool>.Failure("Column is already deleted");
            }

            // Soft delete the column
            column.Delete(); // This will set DeletedAt and DeletedBy
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Column {Id} soft deleted successfully", request.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting column {Id}", request.Id);
            return Result<bool>.Failure("An error occurred while deleting the column");
        }
    }
}
