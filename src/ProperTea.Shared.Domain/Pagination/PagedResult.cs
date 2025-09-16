namespace ProperTea.Shared.Domain.Pagination;

public class PagedResult<T>
{
    public PagedResult(IReadOnlyList<T> items, int totalCount, PageRequest pageRequest)
    {
        Items = items;
        TotalCount = totalCount;
        PageSize = pageRequest.PageSize;
        CurrentPage = pageRequest.PageNumber;
        TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        HasNextPage = CurrentPage < TotalPages;
        HasPreviousPage = CurrentPage > 1;
    }

    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageSize { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
}