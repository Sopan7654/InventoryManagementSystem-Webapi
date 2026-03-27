// ============================================================
// Specific Repository Implementations
// ============================================================
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<Product?> GetBySKUAsync(string sku, CancellationToken ct = default)
            => await _dbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.SKU == sku, ct);

        public async Task<bool> SKUExistsAsync(string sku, CancellationToken ct = default)
            => await _dbSet.AnyAsync(p => p.SKU == sku, ct);
    }

    public class CategoryRepository : Repository<ProductCategory>, ICategoryRepository
    {
        public CategoryRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync(CancellationToken ct = default)
            => await _dbSet.Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories).ToListAsync(ct);
    }

    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<Supplier?> GetWithPurchaseOrdersAsync(string supplierId, CancellationToken ct = default)
            => await _dbSet.Include(s => s.PurchaseOrders).FirstOrDefaultAsync(s => s.SupplierId == supplierId, ct);
    }

    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<Warehouse?> GetWithStockLevelsAsync(string warehouseId, CancellationToken ct = default)
            => await _dbSet.Include(w => w.StockLevels).ThenInclude(sl => sl.Product)
                .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId, ct);
    }

    public class StockLevelRepository : Repository<StockLevel>, IStockLevelRepository
    {
        public StockLevelRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<StockLevel?> GetByProductAndWarehouseAsync(string productId, string warehouseId, CancellationToken ct = default)
            => await _dbSet.Include(sl => sl.Product).Include(sl => sl.Warehouse)
                .FirstOrDefaultAsync(sl => sl.ProductId == productId && sl.WarehouseId == warehouseId, ct);

        public async Task<IEnumerable<StockLevel>> GetLowStockAsync(CancellationToken ct = default)
            => await _dbSet.Include(sl => sl.Product).Include(sl => sl.Warehouse)
                .Where(sl => sl.QuantityOnHand <= sl.ReorderLevel && sl.ReorderLevel > 0)
                .ToListAsync(ct);

        public async Task<IEnumerable<StockLevel>> GetByWarehouseAsync(string warehouseId, CancellationToken ct = default)
            => await _dbSet.Include(sl => sl.Product)
                .Where(sl => sl.WarehouseId == warehouseId).ToListAsync(ct);
    }

    public class StockTransactionRepository : Repository<StockTransaction>, IStockTransactionRepository
    {
        public StockTransactionRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<StockTransaction>> GetByProductAsync(string productId, CancellationToken ct = default)
            => await _dbSet.Include(t => t.Product).Include(t => t.Warehouse)
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.TransactionDate).ToListAsync(ct);

        public async Task<IEnumerable<StockTransaction>> GetRecentAsync(int count, CancellationToken ct = default)
            => await _dbSet.Include(t => t.Product).Include(t => t.Warehouse)
                .OrderByDescending(t => t.TransactionDate).Take(count).ToListAsync(ct);
    }

    public class BatchRepository : Repository<Batch>, IBatchRepository
    {
        public BatchRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Batch>> GetByProductAsync(string productId, CancellationToken ct = default)
            => await _dbSet.Include(b => b.Product).Include(b => b.Warehouse)
                .Where(b => b.ProductId == productId).ToListAsync(ct);

        public async Task<IEnumerable<Batch>> GetExpiringSoonAsync(int daysAhead, CancellationToken ct = default)
        {
            DateTime cutoff = DateTime.Today.AddDays(daysAhead);
            return await _dbSet.Include(b => b.Product).Include(b => b.Warehouse)
                .Where(b => b.ExpiryDate != null && b.ExpiryDate <= cutoff && b.ExpiryDate >= DateTime.Today)
                .OrderBy(b => b.ExpiryDate).ToListAsync(ct);
        }
    }

    public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<PurchaseOrder?> GetWithItemsAsync(string purchaseOrderId, CancellationToken ct = default)
            => await _dbSet.Include(po => po.Supplier)
                .Include(po => po.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == purchaseOrderId, ct);

        public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(string supplierId, CancellationToken ct = default)
            => await _dbSet.Include(po => po.Items)
                .Where(po => po.SupplierId == supplierId).ToListAsync(ct);
    }

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(u => u.Username == username, ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
            => await _dbSet.AnyAsync(u => u.Username == username, ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => await _dbSet.AnyAsync(u => u.Email == email, ct);
    }
}
