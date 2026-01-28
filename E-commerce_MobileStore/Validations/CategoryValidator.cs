using FluentValidation;
using E_commerce_MobileStore.Models;

namespace E_commerce_MobileStore.Validations
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required")
                .Length(2, 100).WithMessage("Category name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-_&]+$").WithMessage("Category name can only contain letters, numbers, spaces, hyphens, underscores, and ampersands");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThan(0).WithMessage("User ID must be a positive number");

            RuleFor(x => x.Created)
                .NotEmpty().WithMessage("Created date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Created date cannot be in the future");

            RuleFor(x => x.Modified)
                .NotEmpty().WithMessage("Modified date is required")
                .GreaterThanOrEqualTo(x => x.Created).WithMessage("Modified date must be after or equal to created date")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Modified date cannot be in the future");
        }
    }
}
