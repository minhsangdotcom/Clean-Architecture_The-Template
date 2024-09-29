using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Contracts.Dtos.Models;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Contracts.Extensions.QueryExtensions;

public static class SortExtension
{
    /// <summary>
    /// Dynamic sort but do not do nested sort for array of poprety
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="sortBy"></param>
    /// <param name="thenby"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public static IQueryable<T> Sort<T>(
        this IQueryable<T> entities,
        string sortBy,
        bool thenby = false,
        bool isNullCheck = false
    )
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return entities;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        string[] sorts = sortBy.Trim().Split(",", StringSplitOptions.TrimEntries);

        Expression expression = entities.Expression;
        foreach (string sort in sorts)
        {
            if (typeof(T).IsNestedPropertyValid(sort))
            {
                throw new NotFoundException(nameof(sort), sort);
            }

            string[] orderField = sort.Split(OrderTerm.DELIMITER);
            string field = orderField[0];
            string order = orderField[1];

            string command =
                order == OrderTerm.DESC
                    ? (thenby ? OrderType.ThenByDescending : OrderType.Descending)
                    : (thenby ? SortType.ThenBy : SortType.OrderBy);

            var member = ExpressionExtension.GetExpressionMember<T>(field, parameter, isNullCheck);
            UnaryExpression converted = Expression.Convert(member, typeof(object));
            Expression<Func<T, object>> lamda = Expression.Lambda<Func<T, object>>(
                converted,
                parameter
            );

            expression = Expression.Call(
                typeof(Queryable),
                command,
                [typeof(T), lamda.ReturnType],
                expression,
                Expression.Quote(lamda)
            );
        }

        return entities.Provider.CreateQuery<T>(expression);
    }

    public static IEnumerable<T> Sort<T>(this IEnumerable<T> entities, string sortBy) =>
        entities.AsQueryable().Sort(sortBy, isNullCheck: true);
}
