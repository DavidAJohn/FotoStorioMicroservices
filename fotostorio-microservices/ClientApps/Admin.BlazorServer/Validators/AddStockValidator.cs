using Admin.BlazorServer.Models;
using FluentValidation;

namespace Admin.BlazorServer.Validators;

public class AddStockValidator : AbstractValidator<AddStockValidationModel>
{
    public AddStockValidator()
    {
        RuleFor(update => update.Added)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Cannot be zero");
    }
}
