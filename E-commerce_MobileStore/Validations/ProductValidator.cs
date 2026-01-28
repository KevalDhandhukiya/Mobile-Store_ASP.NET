using FluentValidation;
using E_commerce_MobileStore.Models;

namespace E_commerce_MobileStore.Validations
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.ModelName)
                .NotEmpty().WithMessage("Model name is required")
                .Length(2, 100).WithMessage("Model name must be between 2 and 100 characters");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required")
                .Length(2, 100).WithMessage("Company name must be between 2 and 100 characters");

            RuleFor(x => x.LaunchDate)
                .NotEmpty().WithMessage("Launch date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Launch date cannot be in the future");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0.01m).WithMessage("Price must be greater than 0.01")
                .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999999.99");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required")
                .GreaterThan(0).WithMessage("Category ID must be a positive number");

            RuleFor(x => x.Stock)
                .NotNull().WithMessage("Stock is required")
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

            RuleFor(x => x.Image1)
                .MaximumLength(500).WithMessage("Image1 path cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Image1));

            RuleFor(x => x.Image2)
                .MaximumLength(500).WithMessage("Image2 path cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Image2));

            RuleFor(x => x.Image3)
                .MaximumLength(500).WithMessage("Image3 path cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Image3));

            RuleFor(x => x.Image4)
                .MaximumLength(500).WithMessage("Image4 path cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Image4));

            RuleFor(x => x.SpecificationId)
                .GreaterThan(0).WithMessage("Specification ID must be a positive number");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThan(0).WithMessage("User ID must be a positive number");

            RuleFor(x => x.IsRemoved)
                .NotNull().WithMessage("IsRemoved status is required");

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
