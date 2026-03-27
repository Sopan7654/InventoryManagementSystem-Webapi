// ============================================================
// FILE: src/InventoryManagement.Domain/Events/IDomainEvent.cs
// ============================================================
namespace InventoryManagement.Domain.Events
{
    /// <summary>
    /// Marker interface for domain events.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>When the event occurred.</summary>
        DateTime OccurredOn { get; }
    }
}
