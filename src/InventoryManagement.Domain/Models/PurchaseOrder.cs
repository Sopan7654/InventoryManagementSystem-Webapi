// ============================================================
// FILE: src/InventoryManagement.Domain/Models/PurchaseOrder.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a purchase order placed with a supplier.
    /// </summary>
    public class PurchaseOrder
    {
        /// <summary>Unique identifier for the purchase order.</summary>
        [Key]
        [MaxLength(36)]
        public string PurchaseOrderId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>FK to the supplier.</summary>
        [Required]
        [MaxLength(36)]
        public string SupplierId { get; set; } = string.Empty;

        /// <summary>Date the order was placed.</summary>
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        /// <summary>Current lifecycle status of the order.</summary>
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.PENDING;

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The supplier for this order.</summary>
        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }

        /// <summary>Line items in this purchase order.</summary>
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }

    /// <summary>
    /// Represents a line item within a purchase order.
    /// </summary>
    public class PurchaseOrderItem
    {
        /// <summary>Unique identifier for the line item.</summary>
        [Key]
        [MaxLength(36)]
        public string POItemId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>FK to the parent purchase order.</summary>
        [Required]
        [MaxLength(36)]
        public string PurchaseOrderId { get; set; } = string.Empty;

        /// <summary>FK to the product being ordered.</summary>
        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>Quantity ordered.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOrdered { get; set; }

        /// <summary>Unit price at time of order.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // ─── Computed Properties (not persisted) ────────────────────
        /// <summary>Line total (Quantity × Unit Price).</summary>
        [NotMapped]
        public decimal LineTotal => QuantityOrdered * UnitPrice;

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The parent purchase order.</summary>
        [ForeignKey(nameof(PurchaseOrderId))]
        public PurchaseOrder? PurchaseOrder { get; set; }

        /// <summary>The product being ordered.</summary>
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
