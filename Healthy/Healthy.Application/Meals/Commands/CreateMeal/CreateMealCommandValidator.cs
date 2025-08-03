using FluentValidation;

namespace Healthy.Application.Meals.Commands.CreateMeal;

public class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
{
    public CreateMealCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(v => v.ImageUrl)
            .NotEmpty()
            .WithMessage("Image URL is required.")
            .MaximumLength(500)
            .WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(v => v.Type)
            .NotEmpty()
            .WithMessage("Meal type is required.")
            .Must(BeValidMealType)
            .WithMessage("Meal type must be one of: Morning, Lunch, Dinner, Snack.");

        RuleFor(v => v.Date)
            .NotEmpty()
            .WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.Today.AddDays(1))
            .WithMessage("Date cannot be in the future beyond tomorrow.");
    }

    private static bool BeValidMealType(string type)
    {
        var validTypes = new[] { "Morning", "Lunch", "Dinner", "Snack" };
        return validTypes.Contains(type);
    }
}
