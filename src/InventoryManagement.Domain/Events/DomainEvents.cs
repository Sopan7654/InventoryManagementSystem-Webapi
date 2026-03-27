// ============================================================
// FILE: src/InventoryManagement.Domain/Events/DomainEvents.cs
// ============================================================
namespace InventoryManagement.Domain.Events
{
    /// <summary>
    /// Raised when a new product is created in the system.
    /// </summary>
    public class ProductCreatedEvent : IDomainEvent
    {
        public string ProductId { get; }
        public string SKU { get; }
        public string ProductName { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public ProductCreatedEvent(string productId, string sku, string productName)
        {
            ProductId = productId;
            SKU = sku;
            ProductName = productName;
        }
    }

    /// <summary>
    /// Raised when stock level changes for a product at a warehouse.
    /// </summary>
    public class StockLevelChangedEvent : IDomainEvent
    {
        public string ProductId { get; }
        public string WarehouseId { get; }
        public decimal PreviousQuantity { get; }
        public decimal NewQuantity { get; }
        public decimal ReorderLevel { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public StockLevelChangedEvent(string productId, string warehouseId,
            decimal previousQuantity, decimal newQuantity, decimal reorderLevel)
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            PreviousQuantity = previousQuantity;
            NewQuantity = newQuantity;
            ReorderLevel = reorderLevel;
        }
    }

    /// <summary>
    /// Raised when stock drops to or below the reorder level.
    /// </summary>
    public class LowStockDetectedEvent : IDomainEvent
    {
        public string ProductId { get; }
        public string WarehouseId { get; }
        public decimal CurrentQuantity { get; }
        public decimal ReorderLevel { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public LowStockDetectedEvent(string productId, string warehouseId,
            decimal currentQuantity, decimal reorderLevel)
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            CurrentQuantity = currentQuantity;
            ReorderLevel = reorderLevel;
        }
    }

    /// <summary>
    /// Raised when a purchase order is approved by management.
    /// </summary>
    public class PurchaseOrderApprovedEvent : IDomainEvent
    {
        public string PurchaseOrderId { get; }
        public string SupplierId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public PurchaseOrderApprovedEvent(string purchaseOrderId, string supplierId)
        {
            PurchaseOrderId = purchaseOrderId;
            SupplierId = supplierId;
        }
    }

    /// <summary>
    /// Raised when a batch is approaching its expiry date.
    /// </summary>
    public class BatchExpiringEvent : IDomainEvent
    {
        public string BatchId { get; }
        public string ProductId { get; }
        public string WarehouseId { get; }
        public DateTime ExpiryDate { get; }
        public int DaysUntilExpiry { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public BatchExpiringEvent(string batchId, string productId, string warehouseId,
            DateTime expiryDate, int daysUntilExpiry)
        {
            BatchId = batchId;
            ProductId = productId;
            WarehouseId = warehouseId;
            ExpiryDate = expiryDate;
            DaysUntilExpiry = daysUntilExpiry;
        }
    }
}
