namespace ProperTea.Shared.Domain.Events;

public abstract record IntegrationEvent(Guid Id, DateTime OccurredAt);