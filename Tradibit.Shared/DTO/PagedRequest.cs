using Tradibit.Shared.DTO.Primitives;

namespace Tradibit.Shared.DTO;

public abstract class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public abstract class PagedSortedRequest : PagedRequest
{
    public string FieldName { get; set; } = null!;
    public SortDirection SortDirection { get; set; }
}