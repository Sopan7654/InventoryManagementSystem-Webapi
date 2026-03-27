// ============================================================
// FILE: src/InventoryManagement.Domain/Models/ProductCategory.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a hierarchical product category.
    /// </summary>
    public class ProductCategory
    {
        /// <summary>Unique identifier for the category.</summary>
        [Key]
        [MaxLength(36)]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Display name of the category.</summary>
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>Description of the category.</summary>
        [MaxLength(255)]
        public string? Description { get; set; }

        /// <summary>FK to parent category for hierarchy support.</summary>
        [MaxLength(36)]
        public string? ParentCategoryId { get; set; }

        // ─── Navigation Properties ──────────────────────────────────
        /// <summary>The parent category (null if root).</summary>
        [ForeignKey(nameof(ParentCategoryId))]
        public ProductCategory? ParentCategory { get; set; }

        /// <summary>Child sub-categories.</summary>
        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        /// <summary>Products in this category.</summary>
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
