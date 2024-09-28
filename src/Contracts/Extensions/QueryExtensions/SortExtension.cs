using System.Linq.Expressions;
using Contracts.Dtos.Models;
using Contracts.Extensions.Expressions;

namespace Contracts.Extensions.QueryExtensions;

public static class SortExtension
{
    public static IQueryable<T> Sort<T>(
        this IQueryable<T> entities,
        string sortBy,
        bool thenby = false
    )
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return entities;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        string[] sortProperties = sortBy.Trim().Split(",", StringSplitOptions.TrimEntries);
        string sortProperty = sortProperties[0];
        string[] orderBy = sortProperty.Split(OrderTerm.DELIMITER);

        string command = sortProperty.ToLower().EndsWith(OrderTerm.DESC)
            ? (thenby ? OrderType.ThenByDescending : OrderType.Descending)
            : (thenby ? SortType.ThenBy : SortType.OrderBy);

        var member = ExpressionExtension.GetExpressionMember<T>(orderBy[0], parameter, false);
        var converted = Expression.Convert(member, typeof(object));
        var lamda = Expression.Lambda<Func<T, object>>(converted, parameter);

        var queryExpression = Expression.Call(
            typeof(Queryable),
            command,
            [typeof(T), lamda.ReturnType],
            entities.Expression,
            Expression.Quote(lamda)
        );

        return Sort(
            entities.Provider.CreateQuery<T>(queryExpression),
            sortProperties.Length == 1 ? string.Empty : string.Join(".", sortProperties.Skip(1)),
            true
        );
    }

    public static IEnumerable<T> Sort<T>(
        this IEnumerable<T> entities,
        string sortBy
    ) => entities.AsQueryable().Sort(sortBy);
}