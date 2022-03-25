using Admin.BlazorServer.Models;
using FluentValidation;

namespace Admin.BlazorServer.Validators;

public class AddNewProductValidator : AbstractValidator<AddNewProductModel>
{
    public AddNewProductValidator()
    {
        RuleFor(product => product.BrandId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Please choose a manufacturer");

        RuleFor(product => product.CategoryId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Please choose a category");

        RuleFor(product => product.MountId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Please choose a mount");

        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(50).WithMessage("The product name cannot be longer than 50 characters");

        RuleFor(product => product.Sku)
            .NotEmpty().WithMessage("Product SKU is required")
            .MaximumLength(12).WithMessage("The product SKU cannot be longer than 12 characters");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(500).WithMessage("The product description cannot be longer than 500 characters");

        RuleFor(product => product.Price)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Please add a price");
    }
}
