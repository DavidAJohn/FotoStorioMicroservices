using Admin.BlazorServer.Models;
using FluentValidation;

namespace Admin.BlazorServer.Validators;

public class RemoveStockValidator : AbstractValidator<RemoveStockValidationModel>
{
    public RemoveStockValidator()
    {
        RuleFor(update => update.Removed)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Cannot be zero");
    }
}
