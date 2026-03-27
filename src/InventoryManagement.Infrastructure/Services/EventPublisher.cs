// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Services/EventPublisher.cs
// ============================================================
using InventoryManagement.Domain.Events;
using InventoryManagement.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Infrastructure.Services
{
    /// <summary>
    /// In-memory event publisher that resolves handlers from DI.
    /// For production, replace with RabbitMQ/Azure Service Bus.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IServiceProvider serviceProvider, ILogger<EventPublisher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
            where TEvent : IDomainEvent
        {
            _logger.LogInformation("Publishing domain event: {EventType} at {OccurredOn}",
                typeof(TEvent).Name, domainEvent.OccurredOn);

            var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(domainEvent, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling event {EventType} in {HandlerType}",
                        typeof(TEvent).Name, handler.GetType().Name);
                }
            }
        }
    }
}
