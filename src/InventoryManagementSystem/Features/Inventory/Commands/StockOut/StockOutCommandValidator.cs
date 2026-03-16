// Features/Inventory/Commands/StockOut/StockOutCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Inventory.Commands.StockOut
{
    public sealed class StockOutCommandValidator : AbstractValidator<StockOutCommand>
    {
        public StockOutCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
