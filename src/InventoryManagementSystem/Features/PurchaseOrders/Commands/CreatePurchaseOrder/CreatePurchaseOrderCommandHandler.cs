// Features/PurchaseOrders/Commands/CreatePurchaseOrder/CreatePurchaseOrderCommandHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.PurchaseOrders.Repository;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.CreatePurchaseOrder
{
    public sealed class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<string>>
    {
        private readonly IPurchaseOrderRepository _repo;
        public CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository repo) => _repo = repo;
        public async Task<Result<string>> Handle(CreatePurchaseOrderCommand cmd, CancellationToken ct)
        {
            var poId = Guid.NewGuid().ToString();
            var po = new PurchaseOrder { PurchaseOrderId = poId, SupplierId = cmd.SupplierId, OrderDate = cmd.OrderDate ?? DateTime.Today, Status = "PENDING" };
            await _repo.InsertPOAsync(po, ct);
            foreach (var item in cmd.Items)
                await _repo.InsertItemAsync(new PurchaseOrderItem { POItemId = Guid.NewGuid().ToString(), PurchaseOrderId = poId, ProductId = item.ProductId, QuantityOrdered = item.QuantityOrdered, UnitPrice = item.UnitPrice }, ct);
            return Result<string>.Success(poId);
        }
    }
}
