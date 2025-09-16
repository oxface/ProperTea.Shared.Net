namespace ProperTea.Shared.Domain.Pagination;

public record SortField(string PropertyName, bool IsDescending = false);

public record SortRequest
{
    private SortRequest(IEnumerable<SortField> fields)
    {
        Fields = fields.ToList();
    }

    public IReadOnlyList<SortField> Fields { get; }

    public static SortRequest By(string propertyName, bool isDescending = false)
    {
        return new SortRequest([new SortField(propertyName, isDescending)]);
    }

    public static SortRequest By(params SortField[] fields)
    {
        return new SortRequest(fields);
    }

    public static SortRequest By(IEnumerable<SortField> fields)
    {
        return new SortRequest(fields);
    }
}