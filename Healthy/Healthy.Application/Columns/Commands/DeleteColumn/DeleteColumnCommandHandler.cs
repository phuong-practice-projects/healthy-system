using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Columns.Commands.DeleteColumn;

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
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (column == null)
            {
                return Result<bool>.Failure("Column not found");
            }

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Column {Id} deleted successfully", request.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting column {Id}", request.Id);
            return Result<bool>.Failure("An error occurred while deleting the column");
        }
    }
}
