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
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id.ToString().ToLower() == request.Id.ToString().ToLower(), cancellationToken);

            if (meal == null)
            {
                return Result<bool>.Failure("Meal not found or access denied.");
            }

            if (meal.DeletedAt.HasValue)
            {
                return Result<bool>.Failure("Meal is already deleted");
            }

            // Soft delete the meal
            meal.Delete(); // This will set DeletedAt and DeletedBy
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error deleting meal: {ex.Message}");
        }
    }
}
