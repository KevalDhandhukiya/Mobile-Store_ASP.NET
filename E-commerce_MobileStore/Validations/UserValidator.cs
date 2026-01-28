// Validation/UserValidator.cs
using FluentValidation;
using E_commerce_MobileStore.Models;

namespace E_commerce_MobileStore.Validations
{
    public class UserValidator : AbstractValidator<User>
    {          
        public UserValidator()
        {
            RuleFor(u => u.UserName)
                .NotNull().WithMessage("Username is required.")
                .NotEmpty().WithMessage("Username cannot be empty.")
                .Length(3, 100).WithMessage("Username must be between 3 and 100 characters.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");

            RuleFor(u => u.Password)
                .NotNull().WithMessage("Password is required.")
                .NotEmpty().WithMessage("Password cannot be empty.")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@$!%*?&]{6,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");

            RuleFor(u => u.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.")
                .When(u => !string.IsNullOrEmpty(u.Email));

            RuleFor(u => u.Contact)
                .Matches(@"^[\+]?[1-9]?\d{1,4}?[\s\-]?\(?\d{1,4}?\)?[\s\-]?\d{1,4}[\s\-]?\d{1,9}$")
                .WithMessage("Invalid contact number format.")
                .MaximumLength(20).WithMessage("Contact number cannot exceed 20 characters.")
                .When(u => !string.IsNullOrEmpty(u.Contact));

            RuleFor(u => u.IsActive)
                .NotNull().WithMessage("IsActive status is required.");

            RuleFor(u => u.IsAdmin)
                .NotNull().WithMessage("IsAdmin status is required.");

            RuleFor(u => u.Created)
                .NotEmpty().WithMessage("Created date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Created date cannot be in the future.");

            RuleFor(u => u.Modified)
                .NotEmpty().WithMessage("Modified date is required.")
                .GreaterThanOrEqualTo(u => u.Created).WithMessage("Modified date must be after or equal to created date.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Modified date cannot be in the future.");
        }
    }
}
