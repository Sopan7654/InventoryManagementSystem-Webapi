// ============================================================
// FILE: src/InventoryManagement.Domain/Models/StockLevel.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents the current stock level of a product at a specific warehouse.
    /// </summary>
    public class StockLevel
    {
        /// <summary>Unique identifier for the stock level record.</summary>
        [Key]
        [MaxLength(36)]
        public string StockLevelId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>FK to the product.</summary>
        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>FK to the warehouse.</summary>
        [Required]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        /// <summary>Current quantity physically on hand.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOnHand { get; set; }

        /// <summary>Threshold below which a low-stock alert is triggered.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ReorderLevel { get; set; }

        /// <summary>Minimum safety stock to maintain.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SafetyStock { get; set; }

        /// <summary>Quantity currently reserved/held.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ReservedQuantity { get; set; }

        // ─── Computed Properties (not persisted) ────────────────────
        /// <summary>Quantity available for sale (on-hand minus reserved).</summary>
        [NotMapped]
        public decimal AvailableQuantity => QuantityOnHand - ReservedQuantity;

        /// <summary>Whether current stock is at or below the reorder level.</summary>
        [NotMapped]
        public bool IsLowStock => QuantityOnHand <= ReorderLevel;

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The product for this stock level.</summary>
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        /// <summary>The warehouse for this stock level.</summary>
        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
    }
}
