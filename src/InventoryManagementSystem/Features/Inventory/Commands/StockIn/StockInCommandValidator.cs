// Features/Inventory/Commands/StockIn/StockInCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Inventory.Commands.StockIn
{
    public sealed class StockInCommandValidator : AbstractValidator<StockInCommand>
    {
        public StockInCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
