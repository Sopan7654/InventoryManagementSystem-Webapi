// Domain/Entities/PurchaseOrder.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class PurchaseOrder
    {
        [Key][MaxLength(36)]
        public string PurchaseOrderId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string SupplierId { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Today;

        [MaxLength(20)]
        public string Status { get; set; } = "PENDING";

        // ─── Display-only (from JOIN/aggregate) ──────────────────────────
        [NotMapped] public string? SupplierName { get; set; }
        [NotMapped] public int ItemCount { get; set; }
        [NotMapped] public decimal TotalAmount { get; set; }

        // ─── EF Core navigation ──────────────────────────────────────────
        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }
        public List<PurchaseOrderItem> Items { get; set; } = new();
    }

    public class PurchaseOrderItem
    {
        [Key][MaxLength(36)]
        public string POItemId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string PurchaseOrderId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOrdered { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // ─── Display-only ─────────────────────────────────────────────────
        [NotMapped] public string? ProductName { get; set; }
        [NotMapped] public decimal LineTotal => QuantityOrdered * UnitPrice;

        // ─── EF Core navigation ──────────────────────────────────────────
        [ForeignKey(nameof(PurchaseOrderId))] public PurchaseOrder? PurchaseOrder { get; set; }
        [ForeignKey(nameof(ProductId))]       public Product? Product { get; set; }
    }
}
