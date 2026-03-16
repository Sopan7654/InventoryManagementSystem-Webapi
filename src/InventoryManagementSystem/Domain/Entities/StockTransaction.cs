// Domain/Entities/StockTransaction.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class StockTransaction
    {
        [Key][MaxLength(36)]
        public string TransactionId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Required][MaxLength(30)]
        public string TransactionType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? Reference { get; set; }

        // ─── Display-only (from JOIN) ─────────────────────────────────────
        [NotMapped] public string? ProductName { get; set; }
        [NotMapped] public string? WarehouseName { get; set; }

        // ─── EF Core navigation ──────────────────────────────────────────
        [ForeignKey(nameof(ProductId))]  public Product? Product { get; set; }
        [ForeignKey(nameof(WarehouseId))] public Warehouse? Warehouse { get; set; }
    }
}
