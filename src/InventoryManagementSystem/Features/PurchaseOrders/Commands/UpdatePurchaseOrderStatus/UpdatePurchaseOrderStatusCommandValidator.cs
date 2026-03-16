// Features/PurchaseOrders/Commands/UpdatePurchaseOrderStatus/UpdatePurchaseOrderStatusCommandValidator.cs
using FluentValidation;
using InventoryManagementSystem.Features.PurchaseOrders.Commands.UpdatePurchaseOrderStatus;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.UpdatePurchaseOrderStatus
{
    public sealed class UpdatePurchaseOrderStatusCommandValidator : AbstractValidator<UpdatePurchaseOrderStatusCommand>
    {
        private static readonly string[] ValidStatuses = ["PENDING", "APPROVED", "RECEIVED", "CANCELLED"];
        public UpdatePurchaseOrderStatusCommandValidator()
        {
            RuleFor(x => x.PurchaseOrderId).NotEmpty();
            RuleFor(x => x.Status).NotEmpty().Must(s => ValidStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");
        }
    }
}
