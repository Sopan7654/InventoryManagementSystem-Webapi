// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/ProductDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for creating a new product.</summary>
    public class CreateProductDto
    {
        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Z0-9\-]{3,20}$", ErrorMessage = "SKU must contain only uppercase letters, digits, and hyphens (3-20 chars).")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, MinimumLength = 2)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? CategoryId { get; set; }

        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = "PCS";

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative.")]
        public decimal Cost { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "List price must be non-negative.")]
        public decimal ListPrice { get; set; }
    }

    /// <summary>DTO for updating an existing product.</summary>
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, MinimumLength = 2)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? CategoryId { get; set; }

        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = "PCS";

        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ListPrice { get; set; }
    }

    /// <summary>Full product response DTO.</summary>
    public class ProductResponseDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal ListPrice { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>Lightweight product summary for list views.</summary>
    public class ProductSummaryDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ListPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
