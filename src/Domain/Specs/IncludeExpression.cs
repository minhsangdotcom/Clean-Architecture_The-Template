using System.Linq.Expressions;
using Domain.Specs.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Specs;

public static class IncludeExpression
{
    public static IQueryable<T> Include<T>(this IQueryable<T> query, List<IncludeInfo> includes)
    {
        Expression queryExpression = query.Expression!;

        foreach (var include in includes)
        {
            ParameterExpression parameter = Expression.Parameter(include.EntityType!, "x");

            string command =
                include.InCludeType == InCludeType.Include
                    ? nameof(EntityFrameworkQueryableExtensions.Include)
                    : nameof(EntityFrameworkQueryableExtensions.ThenInclude);

            List<Type> types = [include.EntityType!];

            if (include.InCludeType == InCludeType.Include)
            {
                types.Add(include.PropertyType!);
            }
            else
            {
                types.AddRange([include.PreviousPropertyType!, include.PropertyType!]);
            }

            queryExpression = Expression.Call(
                typeof(EntityFrameworkQueryableExtensions),
                command,
                [.. types],
                queryExpression,
                Expression.Quote(include.LamdaExpression!)
            );
        }

        return query.Provider.CreateQuery<T>(queryExpression);
    }
}
