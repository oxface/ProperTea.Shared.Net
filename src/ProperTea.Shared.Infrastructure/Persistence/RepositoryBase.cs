using Microsoft.EntityFrameworkCore;
using ProperTea.Domain.Shared;
using ProperTea.Domain.Shared.Pagination;

namespace ProperTea.Infrastructure.Shared.Persistence;

public abstract class RepositoryBase<T, TFilter>(DbContext context)
    : IRepository<T, TFilter>
    where T : AggregateRoot
{
    protected virtual IQueryable<T> BaseQuery => IncludeRelatedData(context.Set<T>());

    protected virtual IAggregateConfiguration<T>? AggregateConfiguration => null;

    protected virtual Dictionary<string, Func<IQueryable<T>, IQueryable<T>>> CustomSortFields { get; } = new();

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await BaseQuery.SingleOrDefaultAsync(e => e.Id == id, ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await context.Set<T>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        context.Set<T>().Update(entity);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        await Task.FromResult(context.Set<T>().Remove(entity));
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        TFilter filter,
        PageRequest pageRequest,
        SortRequest? sortRequest = null,
        CancellationToken ct = default)
    {
        var query = BuildQuery(filter, sortRequest);
        return await query.ToPagedResultAsync(pageRequest, ct);
    }

    public virtual async Task<IReadOnlyList<T>> GetFilteredAsync(
        TFilter filter,
        SortRequest? sortRequest = null,
        CancellationToken ct = default)
    {
        var query = BuildQuery(filter, sortRequest);
        return await query.ToListAsync(ct);
    }

    protected virtual IQueryable<T> IncludeRelatedData(IQueryable<T> query)
    {
        AggregateConfiguration?.ConfigureIncludes(query);
        return query;
    }

    protected abstract IQueryable<T> ApplyFilter(IQueryable<T> query, TFilter filter);

    private IQueryable<T> BuildQuery(
        TFilter filter,
        SortRequest? sortRequest = null)
    {
        var query = BaseQuery;
        query = ApplyFilter(query, filter);
        if (sortRequest != null)
            query = query.ApplySort(sortRequest, CustomSortFields);
        return query;
    }
}