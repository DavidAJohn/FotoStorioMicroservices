using FluentValidation;
using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Validators;

public class RegistrationValidator : AbstractValidator<RegisterModel>
{
    public RegistrationValidator()
    {
        RuleFor(r => r.DisplayName)
                .NotEmpty().WithMessage("Your Name is required")
                .MaximumLength(50).WithMessage("Your name cannot be longer than 50 characters");

        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email Address is required")
            .EmailAddress().WithMessage("This Email Address is invalid");

        RuleFor(r => r.Password).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("A password is required")
            .Length(8, 32).WithMessage("Password must be between 8 and 32 characters")
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,}$")
                .WithMessage("Password must contain a number, upper and lower case letters and at least one special character");

        RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("Passwords do not match");
    }
}
