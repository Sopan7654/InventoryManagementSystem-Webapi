// Features/Inventory/Repository/IStockLevelRepository.cs
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem.Features.Inventory.Repository
{
    public interface IStockLevelRepository : IRepository<StockLevel>
    {
        Task<List<StockLevel>> GetAllAsync(CancellationToken ct = default);
        Task<StockLevel?> GetByProductAndWarehouseAsync(string productId, string warehouseId, CancellationToken ct = default);
        Task<List<StockLevel>> GetLowStockAsync(CancellationToken ct = default);

        // These overloads accept an existing connection+transaction for Unit of Work usage
        Task UpsertAsync(string productId, string warehouseId, decimal qtyDelta,
            MySqlConnection conn, MySqlTransaction txn, CancellationToken ct = default);
        Task UpdateQuantityAsync(string productId, string warehouseId, decimal delta,
            MySqlConnection conn, MySqlTransaction txn, CancellationToken ct = default);
        Task UpdateReservedAsync(string productId, string warehouseId, decimal delta,
            MySqlConnection conn, MySqlTransaction txn, CancellationToken ct = default);
    }
}
