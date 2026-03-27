// ============================================================
// FILE: src/InventoryManagement.Domain/Interfaces/ISpecificRepositories.cs
// ============================================================
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>Product repository with SKU lookup.</summary>
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetBySKUAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> SKUExistsAsync(string sku, CancellationToken cancellationToken = default);
    }

    /// <summary>Product category repository.</summary>
    public interface ICategoryRepository : IRepository<ProductCategory>
    {
        Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>Supplier repository with purchase order lookup.</summary>
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> GetWithPurchaseOrdersAsync(string supplierId, CancellationToken cancellationToken = default);
    }

    /// <summary>Warehouse repository with stock level lookup.</summary>
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<Warehouse?> GetWithStockLevelsAsync(string warehouseId, CancellationToken cancellationToken = default);
    }

    /// <summary>Stock level repository with specialized queries.</summary>
    public interface IStockLevelRepository : IRepository<StockLevel>
    {
        Task<StockLevel?> GetByProductAndWarehouseAsync(string productId, string warehouseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StockLevel>> GetLowStockAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<StockLevel>> GetByWarehouseAsync(string warehouseId, CancellationToken cancellationToken = default);
    }

    /// <summary>Stock transaction repository with history queries.</summary>
    public interface IStockTransactionRepository : IRepository<StockTransaction>
    {
        Task<IEnumerable<StockTransaction>> GetByProductAsync(string productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StockTransaction>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    }

    /// <summary>Batch repository with expiry queries.</summary>
    public interface IBatchRepository : IRepository<Batch>
    {
        Task<IEnumerable<Batch>> GetByProductAsync(string productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Batch>> GetExpiringSoonAsync(int daysAhead, CancellationToken cancellationToken = default);
    }

    /// <summary>Purchase order repository with line items.</summary>
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<PurchaseOrder?> GetWithItemsAsync(string purchaseOrderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(string supplierId, CancellationToken cancellationToken = default);
    }

    /// <summary>User repository for authentication.</summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    }
}
