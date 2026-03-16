// Domain/Entities/Supplier.cs
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Supplier
    {
        [Key][MaxLength(36)]
        public string SupplierId { get; set; } = string.Empty;

        [Required][MaxLength(150)]
        public string SupplierName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        // ─── EF Core navigation ──────────────────────────────────────────
        public List<PurchaseOrder> PurchaseOrders { get; set; } = new();
    }
}
