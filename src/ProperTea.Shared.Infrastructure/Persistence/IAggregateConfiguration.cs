using ProperTea.Domain.Shared;

namespace ProperTea.Infrastructure.Shared.Persistence;

public interface IAggregateConfiguration<in T> where T : AggregateRoot
{
    void ConfigureIncludes(IQueryable<T> query);
}