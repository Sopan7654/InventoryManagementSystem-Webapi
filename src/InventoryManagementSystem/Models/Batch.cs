// Models/Batch.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Batch
    {
        [Key]
        [MaxLength(36)]
        public string BatchId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        // ─── Display-only / computed (not stored in DB) ─────────────
        [NotMapped]
        public string ProductName { get; set; } = string.Empty;

        [NotMapped]
        public string WarehouseName { get; set; } = string.Empty;

        [NotMapped]
        public string ExpiryStatus
        {
            get
            {
                if (ExpiryDate == null) return "No Expiry";
                var days = (ExpiryDate.Value - DateTime.Today).Days;
                if (days < 0) return "EXPIRED";
                if (days <= 30) return "EXPIRING SOON";
                return "OK";
            }
        }

        // ─── EF Core navigation properties ─────────────────────────
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }
    }
}
