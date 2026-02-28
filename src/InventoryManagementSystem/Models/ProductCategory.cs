// Models/ProductCategory.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class ProductCategory
    {
        [Key]
        [MaxLength(36)]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(36)]
        public string? ParentCategoryId { get; set; }

        // ─── Display-only (populated from JOIN in ADO.NET repos) ───
        [NotMapped]
        public string? ParentCategoryName { get; set; }

        // ─── EF Core navigation properties ─────────────────────────
        [ForeignKey(nameof(ParentCategoryId))]
        public ProductCategory? ParentCategory { get; set; }

        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
