// ============================================================
// FILE: src/InventoryManagement.Domain/Models/Product.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a product in the inventory system.
    /// </summary>
    public class Product
    {
        /// <summary>Unique identifier for the product.</summary>
        [Key]
        [MaxLength(36)]
        public string ProductId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Stock Keeping Unit — unique product code.</summary>
        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        /// <summary>Display name of the product.</summary>
        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>Detailed product description.</summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>Foreign key to the product category.</summary>
        [MaxLength(36)]
        public string? CategoryId { get; set; }

        /// <summary>Unit of measure (e.g., PCS, KG, LTR).</summary>
        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = "PCS";

        /// <summary>Cost price per unit.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        /// <summary>List/selling price per unit.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ListPrice { get; set; }

        /// <summary>Whether the product is currently active.</summary>
        public bool IsActive { get; set; } = true;

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The category this product belongs to.</summary>
        [ForeignKey(nameof(CategoryId))]
        public ProductCategory? Category { get; set; }

        /// <summary>Stock levels across all warehouses.</summary>
        public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();

        /// <summary>All stock transactions for this product.</summary>
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

        /// <summary>Batches associated with this product.</summary>
        public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    }
}
