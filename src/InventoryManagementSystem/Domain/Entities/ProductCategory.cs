// Domain/Entities/ProductCategory.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class ProductCategory
    {
        [Key][MaxLength(36)]
        public string CategoryId { get; set; } = string.Empty;

        [Required][MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(36)]
        public string? ParentCategoryId { get; set; }

        // ─── Display-only (from JOIN) ─────────────────────────────────────
        [NotMapped] public string? ParentCategoryName { get; set; }

        // ─── EF Core navigation ──────────────────────────────────────────
        [ForeignKey(nameof(ParentCategoryId))]
        public ProductCategory? ParentCategory { get; set; }
        public List<ProductCategory> SubCategories { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}
