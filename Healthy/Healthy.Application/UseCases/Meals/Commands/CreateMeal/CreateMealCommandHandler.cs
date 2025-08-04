using MediatR;
using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Models;
using Healthy.Application.Common.Interfaces;
using Healthy.Domain.Entities;

namespace Healthy.Application.UseCases.Meals.Commands.CreateMeal;

/// <summary>
/// Handler for creating a new meal record
/// </summary>
public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateMealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the creation of a new meal record
    /// </summary>
    /// <param name="request">Create meal command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with created meal ID</returns>
    public async Task<Result<Guid>> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify that the user exists before creating the meal
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.UserId, cancellationToken);

            if (!userExists)
            {
                return Result<Guid>.Failure("User not found.");
            }

            // Create new meal entity with provided data
            var meal = new Meal
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ImageUrl = request.ImageUrl,
                Type = request.Type,
                Date = request.Date,
                CreatedAt = DateTime.UtcNow
            };

            // Add to database and save changes
            _context.Meals.Add(meal);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(meal.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating meal: {ex.Message}");
        }
    }
}
