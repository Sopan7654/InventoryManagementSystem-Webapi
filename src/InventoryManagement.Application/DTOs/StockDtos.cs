// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/StockDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for stock-in (purchase/receive) operations.</summary>
    public class StockInDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string WarehouseId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        [StringLength(100)]
        public string? Reference { get; set; }
    }

    /// <summary>DTO for stock-out (sale) operations.</summary>
    public class StockOutDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string WarehouseId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        [StringLength(100)]
        public string? Reference { get; set; }
    }

    /// <summary>DTO for inter-warehouse stock transfer.</summary>
    public class StockTransferDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string FromWarehouseId { get; set; } = string.Empty;

        [Required]
        public string ToWarehouseId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }
    }

    /// <summary>DTO for placing a stock hold/reservation.</summary>
    public class HoldStockDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string WarehouseId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }
    }

    /// <summary>DTO for stock adjustments (positive or negative).</summary>
    public class AdjustmentDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string WarehouseId { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }
    }

    /// <summary>Stock level response DTO.</summary>
    public class StockLevelResponseDto
    {
        public string StockLevelId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal SafetyStock { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public bool IsLowStock { get; set; }
    }

    /// <summary>Stock transaction response DTO.</summary>
    public class StockTransactionResponseDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Reference { get; set; }
    }
}
