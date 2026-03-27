// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Repositories/UnitOfWork.cs
// ============================================================
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Coordinates multiple repositories in a single database transaction.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryDbContext _context;
        private IDbContextTransaction? _transaction;

        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private ISupplierRepository? _suppliers;
        private IWarehouseRepository? _warehouses;
        private IStockLevelRepository? _stockLevels;
        private IStockTransactionRepository? _stockTransactions;
        private IBatchRepository? _batches;
        private IPurchaseOrderRepository? _purchaseOrders;
        private IUserRepository? _users;

        public UnitOfWork(InventoryDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products =>
            _products ??= new ProductRepository(_context);
        public ICategoryRepository Categories =>
            _categories ??= new CategoryRepository(_context);
        public ISupplierRepository Suppliers =>
            _suppliers ??= new SupplierRepository(_context);
        public IWarehouseRepository Warehouses =>
            _warehouses ??= new WarehouseRepository(_context);
        public IStockLevelRepository StockLevels =>
            _stockLevels ??= new StockLevelRepository(_context);
        public IStockTransactionRepository StockTransactions =>
            _stockTransactions ??= new StockTransactionRepository(_context);
        public IBatchRepository Batches =>
            _batches ??= new BatchRepository(_context);
        public IPurchaseOrderRepository PurchaseOrders =>
            _purchaseOrders ??= new PurchaseOrderRepository(_context);
        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction ??= await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
