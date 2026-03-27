// ============================================================
// FILE: src/InventoryManagement.Domain/Models/Batch.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a batch of a product with manufacturing and expiry tracking.
    /// </summary>
    public class Batch
    {
        /// <summary>Unique identifier for the batch.</summary>
        [Key]
        [MaxLength(36)]
        public string BatchId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>FK to the product in this batch.</summary>
        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>FK to the warehouse storing this batch.</summary>
        [Required]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        /// <summary>Unique batch number/code.</summary>
        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        /// <summary>Date the batch was manufactured.</summary>
        public DateTime? ManufacturingDate { get; set; }

        /// <summary>Date the batch expires.</summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>Quantity of items in this batch.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        // ─── Computed Properties (not persisted) ────────────────────
        /// <summary>Returns the expiry status: "No Expiry", "EXPIRED", "EXPIRING SOON", or "OK".</summary>
        [NotMapped]
        public string ExpiryStatus
        {
            get
            {
                if (ExpiryDate == null) return "No Expiry";
                int days = (ExpiryDate.Value - DateTime.Today).Days;
                if (days < 0) return "EXPIRED";
                if (days <= 30) return "EXPIRING SOON";
                return "OK";
            }
        }

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The product this batch belongs to.</summary>
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        /// <summary>The warehouse storing this batch.</summary>
        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
    }
}
