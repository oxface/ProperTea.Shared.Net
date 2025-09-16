namespace ProperTea.Shared.Domain.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAllAsync(CancellationToken cancellationToken = default);
    void Enqueue(IDomainEvent domainEvent);
}