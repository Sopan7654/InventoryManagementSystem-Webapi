// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/BatchDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for creating a batch.</summary>
    public class CreateBatchDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string WarehouseId { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string BatchNumber { get; set; } = string.Empty;

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
    }

    /// <summary>DTO for updating a batch.</summary>
    public class UpdateBatchDto
    {
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }
    }

    /// <summary>Batch response DTO.</summary>
    public class BatchResponseDto
    {
        public string BatchId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public string ExpiryStatus { get; set; } = string.Empty;
    }
}
