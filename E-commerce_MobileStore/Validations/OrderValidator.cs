using FluentValidation;
using E_commerce_MobileStore.Models;

namespace E_commerce_MobileStore.Validations
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThan(0).WithMessage("User ID must be a positive number");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Order date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Order date cannot be in the future");

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
