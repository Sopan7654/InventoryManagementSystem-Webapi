// ============================================================
// FILE: src/InventoryManagement.Domain/Enums/PurchaseOrderStatus.cs
// ============================================================
namespace InventoryManagement.Domain.Enums
{
    /// <summary>
    /// Represents the lifecycle status of a purchase order.
    /// </summary>
    public enum PurchaseOrderStatus
    {
        /// <summary>Order has been created but not yet approved.</summary>
        PENDING,

        /// <summary>Order has been approved by management.</summary>
        APPROVED,

        /// <summary>Order has been fully received into inventory.</summary>
        RECEIVED,

        /// <summary>Order has been cancelled.</summary>
        CANCELLED
    }
}
