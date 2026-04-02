// Features/Inventory/Repository/IStockTransactionRepository.cs
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem.Features.Inventory.Repository
{
    public interface IStockTransactionRepository : IRepository<StockTransaction>
    {
        Task<List<StockTransaction>> GetAllAsync(int limit = 50, CancellationToken ct = default);
        Task<List<StockTransaction>> GetByProductAsync(string productId, CancellationToken ct = default);

        /// <summary>Returns transactions for a product, optionally filtered by warehouse, up to a limit.</summary>
        Task<List<StockTransaction>> GetByProductAsync(string productId, string? warehouseId, int limit, CancellationToken ct = default);

        // UoW overload — inserts within an existing transaction
        Task InsertAsync(StockTransaction txn,
            MySqlConnection conn, MySqlTransaction dbTxn, CancellationToken ct = default);
    }
}
