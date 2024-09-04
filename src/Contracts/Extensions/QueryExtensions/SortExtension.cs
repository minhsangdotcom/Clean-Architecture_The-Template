using System.Linq.Expressions;
using Contracts.Dtos.Models;
using Contracts.Extensions.Expressions;

namespace Contracts.Extensions.QueryExtensions;

public static class SortExtension
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> entities, string orderByStringValues, bool thenby = false)
    {
        if (string.IsNullOrWhiteSpace(orderByStringValues))
        {
            return entities;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        string[] orderValues = orderByStringValues.Trim().Split(",");

        string orderValue = orderValues[0];

        string[] orderBy = orderValue.Trim().Split(" ");

        string command = orderValue.ToLower().EndsWith(RequestType.DescOrderType) ?
        (thenby ? OrderType.ThenByDescending : OrderType.Descending) :
        (thenby ? SortType.ThenBy : SortType.OrderBy);

        var member = ExpressionExtension.GetExpressionMember<T>(orderBy[0], parameter, false);

        var converted = Expression.Convert(member, typeof(object));

        var lamda = Expression.Lambda<Func<T, object>>(converted, parameter);

        var queryExpression = Expression.Call(
            typeof(Queryable),
            command,
            [typeof(T), lamda.ReturnType],
            entities.Expression,
            Expression.Quote(lamda));

        return Sort(entities.Provider.CreateQuery<T>(queryExpression),
                        orderValues.Length == 1
                            ? string.Empty
                                : orderValues.Skip(1).Aggregate((current, next) => current + "," + next),
                    true);
    }

    public static IEnumerable<T> Sort<T>(this IEnumerable<T> entities, string orderByStringValues) =>
        entities.AsQueryable().Sort<T>(orderByStringValues);
}