// Models/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        [Key]
        [MaxLength(36)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(36)]
        public string? CategoryId { get; set; }

        [MaxLength(20)]
        public string UnitOfMeasure { get; set; } = "PCS";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ListPrice { get; set; }

        public bool IsActive { get; set; } = true;

        // ─── Display-only (populated from JOIN in ADO.NET repos) ───
        [NotMapped]
        public string? CategoryName { get; set; }

        // ─── EF Core navigation properties ─────────────────────────
        [ForeignKey(nameof(CategoryId))]
        public ProductCategory? Category { get; set; }
    }
}
