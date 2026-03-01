namespace FTG12_ReviewsApi.Application.Common.Models;

/// <summary>
/// Generic paginated response wrapper.
/// </summary>
public record PaginatedList<T>
{
    public List<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Creates a paginated list from a full collection by applying skip/take.
    /// </summary>
    public static PaginatedList<T> Create(IEnumerable<T> source, int page, int pageSize, int totalCount)
    {
        return new PaginatedList<T>
        {
            Items = source.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
