// ============================================================
// FILE: src/InventoryManagement.Domain/Enums/TransactionType.cs
// ============================================================
namespace InventoryManagement.Domain.Enums
{
    /// <summary>
    /// Represents the type of stock transaction.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>Stock received from a purchase order.</summary>
        PURCHASE,

        /// <summary>Stock sold to a customer.</summary>
        SALE,

        /// <summary>Manual stock adjustment (positive or negative).</summary>
        ADJUSTMENT,

        /// <summary>Stock transferred into a warehouse.</summary>
        TRANSFER_IN,

        /// <summary>Stock transferred out of a warehouse.</summary>
        TRANSFER_OUT,

        /// <summary>Stock returned by a customer.</summary>
        RETURN,

        /// <summary>Stock placed on hold/reserved.</summary>
        HOLD,

        /// <summary>Stock released from hold.</summary>
        HOLD_RELEASE,

        /// <summary>Stock written off due to damage, expiry, or loss.</summary>
        WRITE_OFF
    }
}
