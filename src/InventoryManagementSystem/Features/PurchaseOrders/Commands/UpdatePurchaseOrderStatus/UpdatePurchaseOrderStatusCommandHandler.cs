// Features/PurchaseOrders/Commands/UpdatePurchaseOrderStatus/UpdatePurchaseOrderStatusCommandHandler.cs
using MediatR; using InventoryManagementSystem.Common.Exceptions; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.PurchaseOrders.Repository;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.UpdatePurchaseOrderStatus
{
    public sealed class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, Result<bool>>
    {
        private readonly IPurchaseOrderRepository _repo;
        public UpdatePurchaseOrderStatusCommandHandler(IPurchaseOrderRepository repo) => _repo = repo;
        public async Task<Result<bool>> Handle(UpdatePurchaseOrderStatusCommand cmd, CancellationToken ct)
        {
            var updated = await _repo.UpdateStatusAsync(cmd.PurchaseOrderId, cmd.Status, ct);
            if (!updated) throw new NotFoundException(nameof(PurchaseOrder), cmd.PurchaseOrderId);
            return Result<bool>.Success(true);
        }
    }
}
