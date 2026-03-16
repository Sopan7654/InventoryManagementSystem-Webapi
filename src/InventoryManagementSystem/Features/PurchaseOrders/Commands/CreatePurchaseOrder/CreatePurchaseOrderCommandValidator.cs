// Features/PurchaseOrders/Commands/CreatePurchaseOrder/CreatePurchaseOrderCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.CreatePurchaseOrder
{
    public sealed class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
    {
        public CreatePurchaseOrderCommandValidator()
        {
            RuleFor(x => x.SupplierId).NotEmpty();
            RuleFor(x => x.Items).NotEmpty().WithMessage("A purchase order must have at least one line item.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.QuantityOrdered).GreaterThan(0);
                item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
            });
        }
    }
}
