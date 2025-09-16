using ProperTea.Shared.Domain;

namespace ProperTea.Shared.Infrastructure.Persistence;

public interface IAggregateConfiguration<in T> where T : AggregateRoot
{
    void ConfigureIncludes(IQueryable<T> query);
}