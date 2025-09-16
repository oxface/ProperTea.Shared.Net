using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProperTea.Domain.Shared.Pagination;

namespace ProperTea.Infrastructure.Shared.Persistence;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct);

        return new PagedResult<T>(items, totalCount, pageRequest);
    }

    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        SortRequest sort,
        Dictionary<string, Func<IQueryable<T>, IQueryable<T>>>? customSortFields = null)
    {
        if (sort.Fields.Count == 0)
            return query;

        for (var i = 0; i < sort.Fields.Count; i++)
            if (customSortFields != null && customSortFields.TryGetValue(sort.Fields[i].PropertyName, out var value))
                query = value(query);
            else
                query = ApplySortField(query, sort.Fields[i], i == 0);

        return query;
    }

    private static IQueryable<T> ApplySortField<T>(
        IQueryable<T> query,
        SortField field,
        bool isFirstField)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, field.PropertyName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = field.IsDescending
            ? isFirstField ? "OrderByDescending" : "ThenByDescending"
            : isFirstField
                ? "OrderBy"
                : "ThenBy";

        var resultExp = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.Type],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExp);
    }
}