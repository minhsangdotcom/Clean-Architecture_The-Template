using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Contracts.Dtos.Models;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Contracts.Extensions.QueryExtensions;

public static class SortExtension
{
    /// <summary>
    /// Dynamic sort but do not do nested sort for array propreties
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
        bool hasThenBy = false;
        foreach (string sort in sorts)
        {
            string[] orderField = sort.Split(OrderTerm.DELIMITER);
            string field = orderField[0];

            if (!typeof(T).IsNestedPropertyValid(field))
            {
                throw new NotFoundException(nameof(field), field);
            }

            string order = orderField.Length == 1 ? OrderTerm.ASC : orderField[1];

            string command =
                order == OrderTerm.DESC
                    ? (hasThenBy ? OrderType.ThenByDescending : OrderType.Descending)
                    : (hasThenBy ? SortType.ThenBy : SortType.OrderBy);

            var member = parameter.MemberExpression<T>(field, isNullCheck);
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

            hasThenBy = true;
        }

        return entities.Provider.CreateQuery<T>(expression);
    }

    public static IEnumerable<T> Sort<T>(this IEnumerable<T> entities, string sortBy) =>
        entities.AsQueryable().Sort(sortBy, isNullCheck: true);
}
