using FluentValidation;
using E_commerce_MobileStore.Models;

namespace E_commerce_MobileStore.Validations
{
    public class StateValidator : AbstractValidator<State>
    {
        public StateValidator()
        {
            RuleFor(x => x.StateName)
                .NotEmpty().WithMessage("State name is required")
                .Length(2, 100).WithMessage("State name must be between 2 and 100 characters");

            RuleFor(x => x.StateCode)
                .MaximumLength(10).WithMessage("State code cannot exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.StateCode));

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country ID is required")
                .GreaterThan(0).WithMessage("Country ID must be a positive number");

            RuleFor(x => x.CreateDate)
                .NotEmpty().WithMessage("Create date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Create date cannot be in the future");

            RuleFor(x => x.ModifiedDate)
                .NotEmpty().WithMessage("Modified date is required")
                .GreaterThanOrEqualTo(x => x.CreateDate).WithMessage("Modified date must be after or equal to create date")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Modified date cannot be in the future");

            RuleFor(x => x.FilePath)
                .MaximumLength(500).WithMessage("File path cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.FilePath));
        }
    }
}
