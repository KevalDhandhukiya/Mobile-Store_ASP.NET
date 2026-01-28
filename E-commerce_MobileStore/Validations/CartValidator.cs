using FluentValidation;
using E_commerce_MobileStore.Models;

namespace E_commerce_MobileStore.Validations
{
    public class CartValidator : AbstractValidator<Cart>
    {
        public CartValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThan(0).WithMessage("User ID must be a positive number");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required")
                .GreaterThan(0).WithMessage("Product ID must be a positive number");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity is required")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0")
                .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");

            RuleFor(x => x.AddedDate)
                .NotEmpty().WithMessage("Added date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Added date cannot be in the future");

            RuleFor(x => x.Total)
                .NotEmpty().WithMessage("Total is required")
                .GreaterThan(0.01m).WithMessage("Total must be greater than 0.01")
                .LessThanOrEqualTo(9999999.99m).WithMessage("Total cannot exceed 9999999.99");

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
