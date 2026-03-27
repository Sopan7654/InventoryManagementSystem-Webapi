// ============================================================
// FILE: src/InventoryManagement.Domain/Models/StockTransaction.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents an immutable record of a stock movement or adjustment.
    /// </summary>
    public class StockTransaction
    {
        /// <summary>Unique identifier for the transaction.</summary>
        [Key]
        [MaxLength(36)]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>FK to the product involved in the transaction.</summary>
        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>FK to the warehouse where the transaction occurred.</summary>
        [Required]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        /// <summary>Type of stock transaction performed.</summary>
        [Required]
        public TransactionType TransactionType { get; set; }

        /// <summary>Quantity involved in the transaction.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        /// <summary>Date and time the transaction occurred.</summary>
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        /// <summary>Optional reference (e.g., transfer reference, PO number).</summary>
        [MaxLength(100)]
        public string? Reference { get; set; }

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The product involved in this transaction.</summary>
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        /// <summary>The warehouse involved in this transaction.</summary>
        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
    }
}
