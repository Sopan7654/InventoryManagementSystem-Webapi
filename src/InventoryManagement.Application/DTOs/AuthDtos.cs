// ============================================================
// FILE: src/InventoryManagement.Application/DTOs/AuthDtos.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>DTO for user registration.</summary>
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Department { get; set; }

        [StringLength(20)]
        public string? Experience { get; set; }

        [StringLength(500)]
        public string? WarehouseAccess { get; set; }
    }

    /// <summary>DTO for user login.</summary>
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>DTO for token refresh.</summary>
    public class RefreshTokenDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>Authentication response with tokens.</summary>
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>Dashboard aggregate response.</summary>
    public class DashboardResponseDto
    {
        public int TotalProducts { get; set; }
        public int TotalWarehouses { get; set; }
        public int LowStockCount { get; set; }
        public int PendingPurchaseOrders { get; set; }
        public int ExpiringBatchesCount { get; set; }
    }

    /// <summary>Report-specific DTOs.</summary>
    public class InventoryValuationDto
    {
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal Cost { get; set; }
        public decimal TotalValue { get; set; }
    }

    /// <summary>Transaction summary for reports.</summary>
    public class TransactionSummaryDto
    {
        public string TransactionType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
