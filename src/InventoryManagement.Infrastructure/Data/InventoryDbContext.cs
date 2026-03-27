// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Data/InventoryDbContext.cs
// ============================================================
using InventoryManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Data
{
    /// <summary>
    /// EF Core DbContext for the Inventory Management System.
    /// All entity configurations are applied via Fluent API in OnModelCreating.
    /// </summary>
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> Categories => Set<ProductCategory>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<StockLevel> StockLevels => Set<StockLevel>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration classes from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        }
    }
}
