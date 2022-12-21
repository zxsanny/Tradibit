using System.Linq.Expressions;
using Tradibit.Common.DTO;

namespace Tradibit.SharedUI.Extensions;

/// <summary> </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Adds Where true predicate only if result of condition is true.
    /// If false predicate provided, uses it in case of false result
    /// Useful for filters, when filters should be applied only when it was set (not NULL) 
    /// </summary>
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> query, bool? condition,
        Expression<Func<TSource, bool>> truePredicate,
        Expression<Func<TSource, bool>> falsePredicate = null)
    {
        if (!condition.HasValue)
            return query;

        if (condition.Value)
            return query.Where(truePredicate);
        
        if (falsePredicate != null)
            return query.Where(falsePredicate);
        
        return query;
    }

    /// <summary> If statement for more readable flow </summary>
    public static TRes If<TSource, TRes>(this TSource source, bool? condition,
        Func<TSource, TRes> trueFn, 
        Func<TSource, TRes> falseFn = null) where TSource : TRes =>
        condition.HasValue
            ? condition.Value
                ? trueFn(source)
                : falseFn != null 
                    ? falseFn(source)
                    : source
            : source;

    /// <summary>Generates expression like <param name="source"></param>.WHERE ( typeof(x) is <param name="type"></param>) </summary>
    public static Expression<Func<T,bool>> GetOfTypePredicate<T>(this IQueryable<T> source, Type type)
    {
        var parameter = Expression.Parameter(typeof(T), "e");
        var body = Expression.TypeIs(parameter, type);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
        
    /// <summary> Generates expression like WHERE ( predicate1 OR predicate2 OR ... predicateN ) </summary>
    public static IQueryable<T> Or<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, bool>>> predicates)
    {
        Expression<Func<T, bool>> expr = x => false;
        foreach ( var p in predicates ) 
        {
            var invokedExpr = Expression.Invoke(p, expr.Parameters);
            expr = Expression.Lambda<Func<T, bool>>(Expression.OrElse( expr.Body, invokedExpr ), expr.Parameters);
        }
        return source.Where(expr);
    }
    
    /// <summary>Checks whether object = one of element of the list </summary>
    public static bool IsOneOf<T>(this T obj, params T[] objects) =>
        objects.Contains(obj);

    /// <summary>Apply pagination: Gets selected page by PageNumber and PageSize</summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> source, PagedQuery query) =>
        source.Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

    /// <summary> Apply sorting: query.OrderBy(x => x.FieldName), if sortedQuery.SortDirection = Descending then .OrderByDescending  </summary>
    public static IQueryable<TSource> ApplyOrdering<TSource>(this IQueryable<TSource> query, PagedSortedQuery sortedQuery)
    {
        if (string.IsNullOrEmpty(sortedQuery.FieldName) || sortedQuery.SortDirection == SortDirection.Default)
            return query;

        var lambda = (dynamic)CreateExpression(typeof(TSource), sortedQuery.FieldName);

        return sortedQuery.SortDirection switch
        {
            SortDirection.Ascending => Queryable.OrderBy(query, lambda),
            SortDirection.Descending => Queryable.OrderByDescending(query, lambda),
            _ => query
        };
    }

    private static LambdaExpression CreateExpression(Type type, string propertyName)
    {
        var param = Expression.Parameter(type, "x");

        Expression body = param;
        foreach (var member in propertyName.Split('.'))
        {
            body = Expression.PropertyOrField(body, member);
        }

        return Expression.Lambda(body, param);
    }
}

public enum ComparisonEnum
{
    EqualTo,
    NotEqualTo,
    GreaterThan,
    GreaterThanOrEqualTo,
    LessThan,
    LessThanOrEqualTo
}