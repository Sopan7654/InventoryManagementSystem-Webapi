// Features/PurchaseOrders/Queries/GetPurchaseOrderItems/GetPurchaseOrderItemsQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Exceptions; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.PurchaseOrders.Repository;
namespace InventoryManagementSystem.Features.PurchaseOrders.Queries.GetPurchaseOrderItems
{
    public sealed class GetPurchaseOrderItemsQueryHandler : IRequestHandler<GetPurchaseOrderItemsQuery, Result<List<PurchaseOrderItem>>>
    {
        private readonly IPurchaseOrderRepository _repo;
        public GetPurchaseOrderItemsQueryHandler(IPurchaseOrderRepository repo) => _repo = repo;
        public async Task<Result<List<PurchaseOrderItem>>> Handle(GetPurchaseOrderItemsQuery req, CancellationToken ct)
        {
            var po = await _repo.GetByIdAsync(req.PurchaseOrderId, ct)
                ?? throw new NotFoundException(nameof(PurchaseOrder), req.PurchaseOrderId);
            return Result<List<PurchaseOrderItem>>.Success(await _repo.GetItemsByPOAsync(req.PurchaseOrderId, ct));
        }
    }
}
