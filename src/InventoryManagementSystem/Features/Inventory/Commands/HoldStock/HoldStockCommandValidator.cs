// Features/Inventory/Commands/HoldStock/HoldStockCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Inventory.Commands.HoldStock
{
    public sealed class HoldStockCommandValidator : AbstractValidator<HoldStockCommand>
    {
        public HoldStockCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
