// Common/Interfaces/IUnitOfWork.cs  — Unit of Work Pattern
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem.Common.Interfaces
{
    /// <summary>
    /// Unit of Work Pattern: manages a single database transaction across multiple
    /// repository operations in the Inventory feature (StockIn, Transfer, etc.).
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        MySqlConnection Connection { get; }
        MySqlTransaction? Transaction { get; }

        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
