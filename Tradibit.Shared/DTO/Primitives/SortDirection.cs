using System.ComponentModel;

namespace Tradibit.Shared.DTO.Primitives;

public enum SortDirection
{
    [Description("fa-sort-none")]
    Default = 0,

    [Description("fa-sort-up")]
    Ascending = 1,
    
    [Description("fa-sort-down")]
    Descending = 2
}

public static class SortDirectionExtension
{
    public static bool? ToBooleanSorting(this SortDirection sortDirection) =>
        sortDirection switch
        {
            SortDirection.Default => null,
            SortDirection.Ascending => true,
            SortDirection.Descending => false,
            _ => null
        };
}