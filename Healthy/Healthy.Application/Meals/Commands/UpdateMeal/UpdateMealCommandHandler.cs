using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Models;
using Healthy.Application.Common.Interfaces;

namespace Healthy.Application.Meals.Commands.UpdateMeal;

public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateMealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var meal = await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == request.Id && m.UserId == request.UserId, cancellationToken);

            if (meal == null)
            {
                return Result<bool>.Failure("Meal not found or access denied.");
            }

            // Update meal properties
            meal.ImageUrl = request.ImageUrl;
            meal.Type = request.Type;
            meal.Date = request.Date;
            meal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error updating meal: {ex.Message}");
        }
    }
}
