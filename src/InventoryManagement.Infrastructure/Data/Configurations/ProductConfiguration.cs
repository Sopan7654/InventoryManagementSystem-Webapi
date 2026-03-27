// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Data/Configurations/ProductConfiguration.cs
// ============================================================
using InventoryManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.ProductId);
            builder.HasIndex(p => p.SKU).IsUnique();
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.UnitOfMeasure).HasMaxLength(20).HasDefaultValue("PCS");
            builder.Property(p => p.Cost).HasColumnType("decimal(18,2)");
            builder.Property(p => p.ListPrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.IsActive).HasDefaultValue(true);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories");
            builder.HasKey(c => c.CategoryId);
            builder.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(255);

            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");
            builder.HasKey(s => s.SupplierId);
            builder.Property(s => s.SupplierName).IsRequired().HasMaxLength(150);
            builder.Property(s => s.Email).HasMaxLength(100);
            builder.Property(s => s.Phone).HasMaxLength(20);
            builder.Property(s => s.Website).HasMaxLength(200);
        }
    }

    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");
            builder.HasKey(w => w.WarehouseId);
            builder.Property(w => w.WarehouseName).IsRequired().HasMaxLength(100);
            builder.Property(w => w.Location).HasMaxLength(200);
            builder.Property(w => w.Capacity).HasColumnType("decimal(18,2)");
        }
    }
}
