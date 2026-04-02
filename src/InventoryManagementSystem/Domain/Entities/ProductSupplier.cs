// Domain/Entities/ProductSupplier.cs
// Link entity: maps a Product to a Supplier with supplier-specific metadata.
// Required by project spec: ProductSupplier (SupplierProductId, ProductId, SupplierId, SupplierSKU, LeadTime)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    /// <summary>
    /// DDD Entity — represents the relationship between a Product and a Supplier.
    /// One product can have multiple suppliers; one supplier can supply multiple products.
    /// </summary>
    public class ProductSupplier
    {
        [Key][MaxLength(36)]
        public string SupplierProductId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string SupplierId { get; set; } = string.Empty;

        /// <summary>The SKU code used by this specific supplier for this product.</summary>
        [MaxLength(100)]
        public string? SupplierSKU { get; set; }

        /// <summary>Lead time in days — how long this supplier takes to deliver.</summary>
        public int LeadTimeDays { get; set; }

        /// <summary>Cost price from this supplier (may differ across suppliers).</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SupplierCost { get; set; }

        /// <summary>Marks the default/preferred supplier for this product.</summary>
        public bool IsPreferred { get; set; } = false;

        // ─── Display-only (from JOIN) ─────────────────────────────────────
        [NotMapped] public string? ProductName { get; set; }
        [NotMapped] public string? SupplierName { get; set; }

        // ─── EF Core navigation ──────────────────────────────────────────
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }
    }
}
