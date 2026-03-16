// Features/Inventory/Commands/Transfer/TransferCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Inventory.Commands.Transfer
{
    public sealed class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.FromWarehouseId).NotEmpty();
            RuleFor(x => x.ToWarehouseId).NotEmpty()
                .NotEqual(x => x.FromWarehouseId).WithMessage("Source and destination warehouses must be different.");
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
