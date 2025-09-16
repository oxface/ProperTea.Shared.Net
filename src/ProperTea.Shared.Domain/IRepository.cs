using ProperTea.Shared.Domain.Pagination;

namespace ProperTea.Shared.Domain;

public interface IRepository<T, in TFilter> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);

    Task<PagedResult<T>> GetPagedAsync(
        TFilter filter,
        PageRequest pageRequest,
        SortRequest? sortRequest = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<T>> GetFilteredAsync(
        TFilter filter,
        SortRequest? sortRequest = null,
        CancellationToken ct = default);
}