using Microsoft.EntityFrameworkCore;
using ProperTea.Domain.Shared;
using ProperTea.Domain.Shared.Events;
using ProperTea.Shared.Application;

namespace ProperTea.Infrastructure.Shared.Persistence;

public class UnitOfWork(DbContext dbContext, IDomainEventDispatcher dispatcher)
    : IUnitOfWork
{
    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Save data so the events have access to the latest state.
            var result = await dbContext.SaveChangesAsync(cancellationToken);

            const int iterationsLimit = 20;
            var currentIteration = 0;
            bool hasMoreEvents;
            do
            {
                var domainEvents = CollectDomainEvents();
                ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                    dispatcher.Enqueue(domainEvent);

                await dispatcher.DispatchAllAsync(cancellationToken);

                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync(cancellationToken);

                hasMoreEvents = CollectDomainEvents().Count != 0;
                currentIteration++;
            } while (hasMoreEvents && currentIteration < iterationsLimit);

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        var domainEvents = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents);
        return domainEvents.ToList();
    }

    private void ClearDomainEvents()
    {
        foreach (var entity in dbContext.ChangeTracker.Entries<IAggregateRoot>())
            entity.Entity.ClearDomainEvents();
    }
}