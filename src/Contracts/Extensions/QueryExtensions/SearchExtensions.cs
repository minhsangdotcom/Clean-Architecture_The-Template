using System.Linq.Expressions;
using System.Reflection;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Contracts.Extensions.QueryExtensions;

public static class SearchExtensions
{
    public static IQueryable<T> Search<T>(
        this IQueryable<T> query,
        string? keyword,
        List<string>? fields = null,
        int level = 0
    )
    {
        if (level < 0)
        {
            throw new ArgumentException("Level is invalid", nameof(level));
        }

        ParameterExpression x = Expression.Parameter(typeof(T), "x");

        Expression body = SearchBodyExpression<T>(x, keyword, fields, level, false);

        if (body == null)
        {
            return query;
        }

        return query.Where(Expression.Lambda<Func<T, bool>>(body, x));
    }

    public static IEnumerable<T> Search<T>(
        this IEnumerable<T> query,
        string? keyword,
        List<string>? fields = null,
        int level = 0
    )
    {
        if (level < 0)
        {
            throw new ArgumentException("Level is invalid", nameof(level));
        }

        ParameterExpression x = Expression.Parameter(typeof(T), "x");

        Expression body = SearchBodyExpression<T>(x, keyword, fields, level, true);

        if (body == null)
        {
            return query;
        }

        return query.Where(Expression.Lambda<Func<T, bool>>(body, x).Compile());
    }

    private static Expression SearchBodyExpression<T>(
        ParameterExpression parameter,
        string? keyword,
        List<string>? fields = null,
        int level = 0,
        bool isNullCheck = false
    )
    {
        Type type = typeof(T);

        ParameterExpression x = parameter;

        var constant = Expression.Call(
            Expression.Constant(string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword),
            "ToLower",
            Type.EmptyTypes
        );

        Expression? body = null!;

        IEnumerable<string> searchTargets =
            fields?.Count > 0
                ? GetTargetSearchFields(new Queue<string>(fields), type, [])
                : GetStringProperties(type.GetProperties(), [], level);

        if (!searchTargets.Any())
        {
            return body;
        }

        foreach (string propName in searchTargets)
        {
            Expression member = ExpressionExtension.GetExpressionMember<T>(
                propName,
                x,
                isNullCheck
            );

            Expression lower = Expression.Call(member, "ToLower", Type.EmptyTypes);

            Expression nullCheck = Expression.Equal(member, Expression.Constant(null));

            Expression expression = isNullCheck
                ? Expression.Condition(
                    nullCheck,
                    Expression.Constant(false),
                    Expression.Call(lower, "Contains", Type.EmptyTypes, constant)
                )
                : Expression.Call(lower, "Contains", Type.EmptyTypes, constant);

            body = body == null ? expression : Expression.OrElse(body, expression);
        }

        return body;
    }

    private static IEnumerable<string> GetTargetSearchFields(
        Queue<string> propertyInfos,
        Type type,
        List<string> properties
    )
    {
        int length = propertyInfos.Count;

        string property = propertyInfos.Dequeue();

        string[] propertyNames = property.Trim().Split('.');

        PropertyInfo? propertyInfo = type.GetProperty(
            propertyNames[0].Trim(),
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
        );

        if (
            (
                propertyInfo.IsUserDefineType()
                && type.GetNestedPropertyInfo(property).PropertyType == typeof(string)
            )
            || propertyInfo?.PropertyType == typeof(string)
        )
        {
            properties.Add(property);
        }

        if (length == 1)
        {
            return properties;
        }

        return GetTargetSearchFields(propertyInfos, type, properties);
    }

    private static IEnumerable<string> GetStringProperties(
        PropertyInfo[] propertyInfos,
        List<string> properties,
        int level = 0,
        int currentLevel = 0,
        string propertyName = ""
    )
    {
        if (
            !propertyInfos.Any(x => x.PropertyType == typeof(string) || x.IsUserDefineType())
            || (level > 0 && currentLevel >= level)
        )
        {
            return properties;
        }

        if (level > 0)
        {
            currentLevel++;
        }

        foreach (var propertyInfo in propertyInfos)
        {
            if (propertyInfo.IsUserDefineType())
            {
                return GetStringProperties(
                    propertyInfo.PropertyType.GetProperties(),
                    properties,
                    level,
                    currentLevel,
                    propertyName += propertyInfo.Name
                );
            }

            if (propertyInfo.PropertyType == typeof(string))
            {
                properties.Add(
                    string.IsNullOrWhiteSpace(propertyName)
                        ? propertyInfo.Name
                        : $"{propertyName}.{propertyInfo.Name}"
                );
            }
        }

        return properties;
    }
}
