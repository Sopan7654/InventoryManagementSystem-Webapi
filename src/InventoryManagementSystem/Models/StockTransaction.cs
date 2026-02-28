// Models/StockTransaction.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class StockTransaction
    {
        [Key]
        [MaxLength(36)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string TransactionType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public DateTime TransactionDate { get; set; }

        [MaxLength(100)]
        public string? Reference { get; set; }

        // ─── Display-only (populated from JOIN in ADO.NET repos) ────
        [NotMapped]
        public string ProductName { get; set; } = string.Empty;

        [NotMapped]
        public string WarehouseName { get; set; } = string.Empty;

        // ─── EF Core navigation properties ──────────────────────────
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
    }

    /// <summary>Constants for all valid transaction type values.</summary>
    public static class TransactionTypes
    {
        public const string Purchase    = "PURCHASE";
        public const string Sale        = "SALE";
        public const string Adjustment  = "ADJUSTMENT";
        public const string TransferIn  = "TRANSFER_IN";
        public const string TransferOut = "TRANSFER_OUT";
        public const string Return      = "RETURN";
        public const string Hold        = "HOLD";
        public const string HoldRelease = "HOLD_RELEASE";
        public const string WriteOff    = "WRITE_OFF";
    }
}
