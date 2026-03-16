// Features/Products/Commands/CreateProduct/CreateProductCommandValidator.cs
using FluentValidation;

namespace InventoryManagementSystem.Features.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(150).WithMessage("Product name must not exceed 150 characters.");

            RuleFor(x => x.UnitOfMeasure)
                .NotEmpty().WithMessage("Unit of measure is required.")
                .MaximumLength(20);

            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0).WithMessage("Cost must be 0 or greater.");

            RuleFor(x => x.ListPrice)
                .GreaterThanOrEqualTo(0).WithMessage("List price must be 0 or greater.");
        }
    }
}
