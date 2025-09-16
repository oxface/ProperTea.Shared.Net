using ProperTea.Shared.Domain.Events;

namespace ProperTea.Shared.Domain;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}

public abstract class AggregateRoot(Guid id) : Entity(id)
{
    private readonly List<IDomainEvent> domainEvents = [];

    private AggregateRoot() : this(Guid.Empty)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}