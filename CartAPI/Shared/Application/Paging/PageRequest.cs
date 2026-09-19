namespace CartAPI.Shared.Application.Paging;

public sealed record PageRequest
{
    public const int MaxPageSize = 100;

    public PageRequest(int? page, int? pageSize)
    {
        Page = Math.Max(page ?? 1, 1);
        PageSize = Math.Clamp(pageSize ?? 10, 1, MaxPageSize);
    }

    public int Page { get; }

    public int PageSize { get; }

    public int Skip => (Page - 1) * PageSize;
}
