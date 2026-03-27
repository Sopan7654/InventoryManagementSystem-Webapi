// Models/StockLevel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class StockLevel
    {
        [Key]
        [MaxLength(36)]
        public string StockLevelId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOnHand { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReorderLevel { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SafetyStock { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReservedQuantity { get; set; }

        // ─── Computed / display-only (not stored in DB) ──────────────
        [NotMapped]
        public string ProductName { get; set; } = string.Empty;

        [NotMapped]
        public string WarehouseName { get; set; } = string.Empty;

        [NotMapped]
        public decimal AvailableQuantity => QuantityOnHand - ReservedQuantity;

        [NotMapped]
        public bool IsLowStock => QuantityOnHand <= ReorderLevel;

        // ─── EF Core navigation properties ──────────────────────────
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
    }
}
