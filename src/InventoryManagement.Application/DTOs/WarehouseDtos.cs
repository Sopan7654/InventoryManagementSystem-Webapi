// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/WarehouseDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for creating a warehouse.</summary>
    public class CreateWarehouseDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string WarehouseName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Capacity { get; set; }
    }

    /// <summary>DTO for updating a warehouse.</summary>
    public class UpdateWarehouseDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string WarehouseName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Capacity { get; set; }
    }

    /// <summary>Warehouse response DTO.</summary>
    public class WarehouseResponseDto
    {
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public decimal? Capacity { get; set; }
        public int StockLevelCount { get; set; }
    }
}
