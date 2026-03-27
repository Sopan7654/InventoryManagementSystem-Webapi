// ============================================================
// FILE: src/InventoryManagement.Domain/Models/Supplier.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a supplier who provides products for purchase orders.
    /// </summary>
    public class Supplier
    {
        /// <summary>Unique identifier for the supplier.</summary>
        [Key]
        [MaxLength(36)]
        public string SupplierId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Display name of the supplier.</summary>
        [Required]
        [MaxLength(150)]
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>Contact email address.</summary>
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>Contact phone number.</summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>Supplier's website URL.</summary>
        [MaxLength(200)]
        public string? Website { get; set; }

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>Purchase orders placed with this supplier.</summary>
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
