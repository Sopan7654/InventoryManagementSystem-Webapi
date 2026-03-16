// Domain/Entities/Batch.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Batch
    {
        [Key][MaxLength(36)]
        public string BatchId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required][MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Required][MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        // ─── Display-only (from JOIN) ─────────────────────────────────────
        [NotMapped] public string? ProductName { get; set; }
        [NotMapped] public string? WarehouseName { get; set; }
        [NotMapped] public string ExpiryStatus => ExpiryDate.HasValue
            ? (ExpiryDate.Value < DateTime.Today ? "EXPIRED"
               : ExpiryDate.Value <= DateTime.Today.AddDays(30) ? "EXPIRING_SOON" : "OK")
            : "NO_EXPIRY";

        // ─── EF Core navigation ──────────────────────────────────────────
        [ForeignKey(nameof(ProductId))]  public Product? Product { get; set; }
        [ForeignKey(nameof(WarehouseId))] public Warehouse? Warehouse { get; set; }
    }
}
