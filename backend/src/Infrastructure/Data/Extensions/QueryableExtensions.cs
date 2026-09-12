using System.Linq.Expressions;

namespace Infrastructure.Data.Extensions;

public static class QueryableExtensions
{
    private static readonly MethodInfo _containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;

    public static IOrderedQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return source.OrderBy(x => 0);
        }

        var parts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var propertyName = parts[0];
        var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = BuildPropertyAccess(parameter, propertyName);

        if (propertyAccess is null)
        {
            return source.OrderBy(x => 0);
        }

        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        var methodName = descending ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), propertyAccess.Type],
            source.Expression,
            Expression.Quote(orderByExpression));

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<T> WhereContainsText<T>(
        this IQueryable<T> source,
        string searchTerm,
        params string[] propertyNames)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || propertyNames.Length == 0)
        {
            return source;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var searchConstant = Expression.Constant(searchTerm);

        Expression? combinedExpression = null;

        foreach (var propertyName in propertyNames)
        {
            var propertyAccess = BuildPropertyAccess(parameter, propertyName);

            if (propertyAccess is null || propertyAccess.Type != typeof(string))
            {
                continue;
            }

            var containsCall = Expression.Call(propertyAccess, _containsMethod, searchConstant);

            combinedExpression = combinedExpression is null
                ? containsCall
                : Expression.OrElse(combinedExpression, containsCall);
        }

        if (combinedExpression is null)
        {
            return source;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

        return source.Where(lambda);
    }

    /// <summary>
    /// Resolves a (possibly dotted, e.g. "Company.Name") property path into a member-access
    /// expression, so entities can be sorted/searched on navigation properties.
    /// </summary>
    private static Expression? BuildPropertyAccess(Expression parameter, string propertyPath)
    {
        Expression current = parameter;

        foreach (var segment in propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            var property = current.Type.GetProperty(segment,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property is null)
            {
                return null;
            }

            current = Expression.MakeMemberAccess(current, property);
        }

        return current == parameter ? null : current;
    }
}
