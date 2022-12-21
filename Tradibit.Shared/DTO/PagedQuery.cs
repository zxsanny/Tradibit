namespace Tradibit.Common.DTO;

/// <summary> </summary>
public abstract class PagedQuery
{
    /// <summary></summary>
    public int PageNumber { get; set; } = 1;

    /// <summary></summary>
    public int PageSize { get; set; } = 10;
}

public abstract class PagedSortedQuery : PagedQuery
{
    /// <summary> </summary>
    public string FieldName { get; set; }

    /// <summary> </summary>
    public SortDirection SortDirection { get; set; }
}