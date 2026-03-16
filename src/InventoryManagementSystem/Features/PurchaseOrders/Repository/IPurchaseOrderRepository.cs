// Features/PurchaseOrders/Repository/IPurchaseOrderRepository.cs
using InventoryManagementSystem.Common.Interfaces; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.PurchaseOrders.Repository
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<List<PurchaseOrder>> GetAllAsync(CancellationToken ct = default);
        Task<PurchaseOrder?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<List<PurchaseOrderItem>> GetItemsByPOAsync(string poId, CancellationToken ct = default);
        Task InsertPOAsync(PurchaseOrder po, CancellationToken ct = default);
        Task InsertItemAsync(PurchaseOrderItem item, CancellationToken ct = default);
        Task<bool> UpdateStatusAsync(string poId, string status, CancellationToken ct = default);
    }
}
