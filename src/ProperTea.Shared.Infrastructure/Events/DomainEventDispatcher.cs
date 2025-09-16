using Microsoft.Extensions.DependencyInjection;
using ProperTea.Domain.Shared.Events;

namespace ProperTea.Infrastructure.Shared.Events;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    private readonly PriorityQueue<IDomainEvent, int> events = new();

    public void Enqueue(IDomainEvent domainEvent)
    {
        var priority = (domainEvent as IPrioritizedDomainEvent)?.Priority ?? 0;
        events.Enqueue(domainEvent, priority);
    }

    public async Task DispatchAllAsync(CancellationToken cancellationToken = default)
    {
        while (events.TryDequeue(out var domainEvent, out _))
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
                await (Task)handlerType
                    .GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!
                    .Invoke(handler, [domainEvent, cancellationToken])!;
        }
    }
}