using Tradibit.SharedUI.DTO.Primitives;

namespace Tradibit.SharedUI.DTO;

/// <summary> </summary>
public abstract class PagedRequest
{
    /// <summary></summary>
    public int PageNumber { get; set; } = 1;

    /// <summary></summary>
    public int PageSize { get; set; } = 10;
}

public abstract class PagedSortedRequest : PagedRequest
{
    /// <summary> </summary>
    public string? FieldName { get; set; }

    /// <summary> </summary>
    public SortDirection
        SortDirection { get; set; }
}