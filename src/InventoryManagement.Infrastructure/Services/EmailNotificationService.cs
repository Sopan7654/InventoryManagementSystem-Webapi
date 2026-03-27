// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Services/EmailNotificationService.cs
// ============================================================
using InventoryManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Infrastructure.Services
{
    /// <summary>
    /// Stub email service — logs emails instead of sending.
    /// Replace with SendGrid/SMTP in production.
    /// </summary>
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendLowStockAlertAsync(string productId, string warehouseId,
            decimal currentQuantity, decimal reorderLevel, CancellationToken ct = default)
        {
            _logger.LogWarning(
                "📧 [STUB EMAIL] Low Stock Alert — Product: {ProductId}, Warehouse: {WarehouseId}, " +
                "Current: {CurrentQty}, Reorder Level: {ReorderLevel}",
                productId, warehouseId, currentQuantity, reorderLevel);
            return Task.CompletedTask;
        }

        public Task SendBatchExpiryAlertAsync(string batchId, string productName,
            DateTime expiryDate, CancellationToken ct = default)
        {
            _logger.LogWarning(
                "📧 [STUB EMAIL] Batch Expiry Alert — Batch: {BatchId}, Product: {ProductName}, " +
                "Expiry: {ExpiryDate:yyyy-MM-dd}",
                batchId, productName, expiryDate);
            return Task.CompletedTask;
        }
    }
}
