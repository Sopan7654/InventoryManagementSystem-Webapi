// Features/Inventory/Commands/Adjustment/AdjustmentCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Inventory.Commands.Adjustment
{
    public sealed class AdjustmentCommandValidator : AbstractValidator<AdjustmentCommand>
    {
        public AdjustmentCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.Quantity).NotEqual(0).WithMessage("Adjustment quantity cannot be zero.");
        }
    }
}
