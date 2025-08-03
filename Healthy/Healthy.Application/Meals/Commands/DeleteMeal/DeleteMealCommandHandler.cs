using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Models;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Application.Meals.Commands.DeleteMeal;

public class DeleteMealCommandHandler : IRequestHandler<DeleteMealCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteMealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteMealCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var meal = await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == request.Id && m.UserId == request.UserId, cancellationToken);

            if (meal == null)
            {
                return Result<bool>.Failure("Meal not found or access denied.");
            }

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error deleting meal: {ex.Message}");
        }
    }
}
