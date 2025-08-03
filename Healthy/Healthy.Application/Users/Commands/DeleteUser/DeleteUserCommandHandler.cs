using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;

namespace Healthy.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            if (user.DeletedAt.HasValue)
            {
                return Result<bool>.Failure("User is already deleted");
            }

            // Soft delete the user
            user.Delete(); // This will set DeletedAt and DeletedBy
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Id} soft deleted successfully", request.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", request.Id);
            return Result<bool>.Failure("An error occurred while deleting the user");
        }
    }
}
