// ============================================================
// FILE: src/InventoryManagement.Domain/Interfaces/IEventPublisher.cs
// ============================================================
using InventoryManagement.Domain.Events;

namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>
    /// Publishes domain events to registered handlers.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>Publishes a domain event to all registered handlers.</summary>
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;
    }

    /// <summary>
    /// Handles a specific type of domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to handle.</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>Handles the domain event.</summary>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
