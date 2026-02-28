// Models/PurchaseOrder.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class PurchaseOrder
    {
        [Key]
        [MaxLength(36)]
        public string PurchaseOrderId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string SupplierId { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "PENDING";

        // ─── Display-only (populated from JOIN in ADO.NET repos) ────
        [NotMapped]
        public string SupplierName { get; set; } = string.Empty;

        [NotMapped]
        public int ItemCount { get; set; }

        [NotMapped]
        public decimal TotalAmount { get; set; }

        // ─── EF Core navigation properties ──────────────────────────
        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }

        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }

    public class PurchaseOrderItem
    {
        [Key]
        [MaxLength(36)]
        public string POItemId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string PurchaseOrderId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOrdered { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // ─── Computed / display-only (not stored in DB) ──────────────
        [NotMapped]
        public decimal LineTotal => QuantityOrdered * UnitPrice;

        [NotMapped]
        public string ProductName { get; set; } = string.Empty;

        // ─── EF Core navigation properties ──────────────────────────
        [ForeignKey(nameof(PurchaseOrderId))]
        public PurchaseOrder? PurchaseOrder { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
