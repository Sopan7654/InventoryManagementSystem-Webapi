// ============================================================
// FILE: src/InventoryManagement.Domain/Models/Warehouse.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a physical warehouse location where inventory is stored.
    /// </summary>
    public class Warehouse
    {
        /// <summary>Unique identifier for the warehouse.</summary>
        [Key]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Display name of the warehouse.</summary>
        [Required]
        [MaxLength(100)]
        public string WarehouseName { get; set; } = string.Empty;

        /// <summary>Physical address or location description.</summary>
        [MaxLength(200)]
        public string? Location { get; set; }

        /// <summary>Maximum storage capacity of the warehouse.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Capacity { get; set; }

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>Stock levels for all products in this warehouse.</summary>
        public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();

        /// <summary>All stock transactions at this warehouse.</summary>
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
