// ============================================================
// FILE: src/InventoryManagement.Domain/Interfaces/IUnitOfWork.cs
// ============================================================
namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern — coordinates writing to multiple repositories
    /// within a single database transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>Product repository.</summary>
        IProductRepository Products { get; }

        /// <summary>Product category repository.</summary>
        ICategoryRepository Categories { get; }

        /// <summary>Supplier repository.</summary>
        ISupplierRepository Suppliers { get; }

        /// <summary>Warehouse repository.</summary>
        IWarehouseRepository Warehouses { get; }

        /// <summary>Stock level repository.</summary>
        IStockLevelRepository StockLevels { get; }

        /// <summary>Stock transaction repository.</summary>
        IStockTransactionRepository StockTransactions { get; }

        /// <summary>Batch repository.</summary>
        IBatchRepository Batches { get; }

        /// <summary>Purchase order repository.</summary>
        IPurchaseOrderRepository PurchaseOrders { get; }

        /// <summary>User repository.</summary>
        IUserRepository Users { get; }

        /// <summary>Saves all changes made in the current unit of work to the database.</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>Begins a new database transaction.</summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>Commits the current transaction.</summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>Rolls back the current transaction.</summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
