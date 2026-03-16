// Features/Products/Commands/UpdateProduct/UpdateProductCommandValidator.cs
using FluentValidation;

namespace InventoryManagementSystem.Features.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ProductName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ListPrice).GreaterThanOrEqualTo(0);
        }
    }
}
