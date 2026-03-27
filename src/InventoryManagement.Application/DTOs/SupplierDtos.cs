// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/SupplierDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for creating a supplier.</summary>
    public class CreateSupplierDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string SupplierName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }
    }

    /// <summary>DTO for updating a supplier.</summary>
    public class UpdateSupplierDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string SupplierName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }
    }

    /// <summary>Supplier response DTO.</summary>
    public class SupplierResponseDto
    {
        public string SupplierId { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public int PurchaseOrderCount { get; set; }
    }
}
