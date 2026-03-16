// Features/PurchaseOrders/Queries/GetAllPurchaseOrders/GetAllPurchaseOrdersQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.PurchaseOrders.Repository;
namespace InventoryManagementSystem.Features.PurchaseOrders.Queries.GetAllPurchaseOrders
{
    public sealed class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, Result<List<PurchaseOrder>>>
    {
        private readonly IPurchaseOrderRepository _repo;
        public GetAllPurchaseOrdersQueryHandler(IPurchaseOrderRepository repo) => _repo = repo;
        public async Task<Result<List<PurchaseOrder>>> Handle(GetAllPurchaseOrdersQuery req, CancellationToken ct)
            => Result<List<PurchaseOrder>>.Success(await _repo.GetAllAsync(ct));
    }
}
