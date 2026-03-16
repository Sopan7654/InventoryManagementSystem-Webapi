// Data/InventoryDbContext.cs
using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Data
{
    /// <summary>
    /// EF Core DbContext for the Inventory Management System.
    /// Configured for MySQL via Pomelo provider.
    /// </summary>
    public class InventoryDbContext : DbContext
    {
        // ── DbSets ──────────────────────────────────────────────────────────────
        public DbSet<ProductCategory>   ProductCategories   { get; set; } = null!;
        public DbSet<Supplier>          Suppliers           { get; set; } = null!;
        public DbSet<Warehouse>         Warehouses          { get; set; } = null!;
        public DbSet<Product>           Products            { get; set; } = null!;
        public DbSet<Batch>             Batches             { get; set; } = null!;
        public DbSet<StockLevel>        StockLevels         { get; set; } = null!;
        public DbSet<StockTransaction>  StockTransactions   { get; set; } = null!;
        public DbSet<PurchaseOrder>     PurchaseOrders      { get; set; } = null!;
        public DbSet<PurchaseOrderItem> PurchaseOrderItems  { get; set; } = null!;

        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── ProductCategory ─────────────────────────────────────────────────
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory");
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.CategoryName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasMaxLength(255);

                entity.Property(e => e.ParentCategoryId)
                      .HasMaxLength(36);

                // Display-only field — not stored in DB
                entity.Ignore(e => e.ParentCategoryName);

                // Self-referencing FK: ParentCategoryId → CategoryId
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.SubCategories)
                      .HasForeignKey(e => e.ParentCategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.CategoryName).HasDatabaseName("idx_category_name");
            });

            // ── Supplier ────────────────────────────────────────────────────────
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");
                entity.HasKey(e => e.SupplierId);

                entity.Property(e => e.SupplierId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.SupplierName)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasMaxLength(100);

                entity.Property(e => e.Phone)
                      .HasMaxLength(20);

                entity.Property(e => e.Website)
                      .HasMaxLength(200);
            });

            // ── Warehouse ───────────────────────────────────────────────────────
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("Warehouse");
                entity.HasKey(e => e.WarehouseId);

                entity.Property(e => e.WarehouseId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.WarehouseName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Location)
                      .HasMaxLength(200);

                entity.Property(e => e.Capacity)
                      .HasColumnType("decimal(18,2)");
            });

            // ── Product ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.SKU)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ProductName)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.CategoryId)
                      .HasMaxLength(36);

                entity.Property(e => e.UnitOfMeasure)
                      .HasMaxLength(20)
                      .HasDefaultValue("PCS");

                entity.Property(e => e.Cost)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ListPrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                // Display-only — not a DB column
                entity.Ignore(e => e.CategoryName);

                // FK: Product.CategoryId → ProductCategory.CategoryId
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.SKU)
                      .IsUnique()
                      .HasDatabaseName("idx_product_sku");

                entity.HasIndex(e => e.ProductName)
                      .HasDatabaseName("idx_product_name");
            });

            // ── Batch ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Batch>(entity =>
            {
                entity.ToTable("Batch");
                entity.HasKey(e => e.BatchId);

                entity.Property(e => e.BatchId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.ProductId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.WarehouseId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.BatchNumber)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Quantity)
                      .HasColumnType("decimal(18,2)");

                // Display-only / computed — not DB columns
                entity.Ignore(e => e.ProductName);
                entity.Ignore(e => e.WarehouseName);
                entity.Ignore(e => e.ExpiryStatus);

                // FK: Batch.ProductId → Product.ProductId
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: Batch.WarehouseId → Warehouse.WarehouseId
                entity.HasOne(e => e.Warehouse)
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.BatchNumber)
                      .IsUnique()
                      .HasDatabaseName("idx_batch_number");

                entity.HasIndex(e => e.ExpiryDate)
                      .HasDatabaseName("idx_batch_expiry");
            });

            // ── StockLevel ──────────────────────────────────────────────────────
            modelBuilder.Entity<StockLevel>(entity =>
            {
                entity.ToTable("StockLevel");
                entity.HasKey(e => e.StockLevelId);

                entity.Property(e => e.StockLevelId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.ProductId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.WarehouseId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.QuantityOnHand)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ReorderLevel)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.SafetyStock)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ReservedQuantity)
                      .HasColumnType("decimal(18,2)");

                // Computed / display-only — not DB columns
                entity.Ignore(e => e.ProductName);
                entity.Ignore(e => e.WarehouseName);
                entity.Ignore(e => e.AvailableQuantity);
                entity.Ignore(e => e.IsLowStock);

                // Unique constraint: one StockLevel row per Product+Warehouse
                entity.HasIndex(e => new { e.ProductId, e.WarehouseId })
                      .IsUnique()
                      .HasDatabaseName("idx_stocklevel_product_warehouse");

                // FK: StockLevel.ProductId → Product.ProductId
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // FK: StockLevel.WarehouseId → Warehouse.WarehouseId
                entity.HasOne(e => e.Warehouse)
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── StockTransaction ────────────────────────────────────────────────
            modelBuilder.Entity<StockTransaction>(entity =>
            {
                entity.ToTable("StockTransaction");
                entity.HasKey(e => e.TransactionId);

                entity.Property(e => e.TransactionId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.ProductId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.WarehouseId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.TransactionType)
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.Quantity)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Reference)
                      .HasMaxLength(100);

                // Display-only — not DB columns
                entity.Ignore(e => e.ProductName);
                entity.Ignore(e => e.WarehouseName);

                // FK: StockTransaction.ProductId → Product.ProductId
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: StockTransaction.WarehouseId → Warehouse.WarehouseId
                entity.HasOne(e => e.Warehouse)
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.TransactionDate)
                      .HasDatabaseName("idx_transaction_date");

                entity.HasIndex(e => e.TransactionType)
                      .HasDatabaseName("idx_transaction_type");
            });

            // ── PurchaseOrder ───────────────────────────────────────────────────
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("PurchaseOrder");
                entity.HasKey(e => e.PurchaseOrderId);

                entity.Property(e => e.PurchaseOrderId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.SupplierId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasMaxLength(20)
                      .HasDefaultValue("PENDING");

                // Display-only
                entity.Ignore(e => e.SupplierName);

                // FK: PurchaseOrder.SupplierId → Supplier.SupplierId
                entity.HasOne(e => e.Supplier)
                      .WithMany(s => s.PurchaseOrders)
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One PurchaseOrder has many PurchaseOrderItems
                entity.HasMany(e => e.Items)
                      .WithOne(i => i.PurchaseOrder)
                      .HasForeignKey(i => i.PurchaseOrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Status)
                      .HasDatabaseName("idx_po_status");
            });

            // ── PurchaseOrderItem ───────────────────────────────────────────────
            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.ToTable("PurchaseOrderItem");
                entity.HasKey(e => e.POItemId);

                entity.Property(e => e.POItemId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.PurchaseOrderId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.ProductId)
                      .HasMaxLength(36)
                      .IsRequired();

                entity.Property(e => e.QuantityOrdered)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.UnitPrice)
                      .HasColumnType("decimal(18,2)");

                // Computed — not a DB column
                entity.Ignore(e => e.LineTotal);
                entity.Ignore(e => e.ProductName);

                // FK: PurchaseOrderItem.ProductId → Product.ProductId
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
