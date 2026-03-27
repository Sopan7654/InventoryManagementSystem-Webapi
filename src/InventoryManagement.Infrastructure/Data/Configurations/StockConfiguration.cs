// ============================================================
// EF Configurations for StockLevel, StockTransaction, Batch, PO, User
// ============================================================
using InventoryManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations
{
    public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
    {
        public void Configure(EntityTypeBuilder<StockLevel> builder)
        {
            builder.ToTable("StockLevels");
            builder.HasKey(s => s.StockLevelId);
            builder.HasIndex(s => new { s.ProductId, s.WarehouseId }).IsUnique();
            builder.Property(s => s.QuantityOnHand).HasColumnType("decimal(18,2)");
            builder.Property(s => s.ReorderLevel).HasColumnType("decimal(18,2)");
            builder.Property(s => s.SafetyStock).HasColumnType("decimal(18,2)");
            builder.Property(s => s.ReservedQuantity).HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.Product).WithMany(p => p.StockLevels)
                .HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.Warehouse).WithMany(w => w.StockLevels)
                .HasForeignKey(s => s.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.ToTable("StockTransactions");
            builder.HasKey(t => t.TransactionId);
            builder.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(30);
            builder.Property(t => t.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Reference).HasMaxLength(100);
            builder.HasIndex(t => t.TransactionDate);

            builder.HasOne(t => t.Product).WithMany(p => p.StockTransactions)
                .HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(t => t.Warehouse).WithMany(w => w.StockTransactions)
                .HasForeignKey(t => t.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class BatchConfiguration : IEntityTypeConfiguration<Batch>
    {
        public void Configure(EntityTypeBuilder<Batch> builder)
        {
            builder.ToTable("Batches");
            builder.HasKey(b => b.BatchId);
            builder.HasIndex(b => b.BatchNumber).IsUnique();
            builder.Property(b => b.BatchNumber).IsRequired().HasMaxLength(50);
            builder.Property(b => b.Quantity).HasColumnType("decimal(18,2)");

            builder.HasOne(b => b.Product).WithMany(p => p.Batches)
                .HasForeignKey(b => b.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Warehouse).WithMany()
                .HasForeignKey(b => b.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("PurchaseOrders");
            builder.HasKey(po => po.PurchaseOrderId);
            builder.Property(po => po.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(po => po.Supplier).WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
        {
            builder.ToTable("PurchaseOrderItems");
            builder.HasKey(i => i.POItemId);
            builder.Property(i => i.QuantityOrdered).HasColumnType("decimal(18,2)");
            builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");

            builder.HasOne(i => i.PurchaseOrder).WithMany(po => po.Items)
                .HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(i => i.Product).WithMany()
                .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
            builder.Property(u => u.RefreshToken).HasMaxLength(500);
            builder.Property(u => u.Department).HasMaxLength(50);
            builder.Property(u => u.Experience).HasMaxLength(20);
            builder.Property(u => u.WarehouseAccess).HasMaxLength(500);
        }
    }
}
