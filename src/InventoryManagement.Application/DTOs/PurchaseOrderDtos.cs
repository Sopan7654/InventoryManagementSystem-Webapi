// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/PurchaseOrderDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for creating a purchase order.</summary>
    public class CreatePurchaseOrderDto
    {
        [Required]
        public string SupplierId { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<AddPurchaseOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>DTO for updating a purchase order.</summary>
    public class UpdatePurchaseOrderDto
    {
        [Required]
        public string SupplierId { get; set; } = string.Empty;
    }

    /// <summary>DTO for adding a line item to a purchase order.</summary>
    public class AddPurchaseOrderItemDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal QuantityOrdered { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    /// <summary>Purchase order response DTO.</summary>
    public class PurchaseOrderResponseDto
    {
        public string PurchaseOrderId { get; set; } = string.Empty;
        public string SupplierId { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderItemResponseDto> Items { get; set; } = new();
    }

    /// <summary>Purchase order item response DTO.</summary>
    public class PurchaseOrderItemResponseDto
    {
        public string POItemId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
