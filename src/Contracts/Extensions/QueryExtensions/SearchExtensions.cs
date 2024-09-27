using System.Linq.Expressions;
using System.Reflection;
using Contracts.Dtos.Models;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Contracts.Extensions.QueryExtensions;

public static class SearchExtensions
{
    /// <summary>
    /// Search for IQueryable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="keyword"></param>
    /// <param name="fields"></param>
    /// <param name="deep"></param>
    /// <returns></returns>
    public static IQueryable<T> Search<T>(
        this IQueryable<T> query,
        string? keyword,
        List<string>? fields = null,
        int deep = 1
    )
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        SearchResult searchResult = Search<T>(fields, keyword, deep, false);

        return query.Where(
            Expression.Lambda<Func<T, bool>>(searchResult.Expression, searchResult.Parameter)
        );
    }

    /// <summary>
    /// search for IEnumrable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="keyword"></param>
    /// <param name="fields"></param>
    /// <param name="deep"></param>
    /// <returns></returns>
    public static IEnumerable<T> Search<T>(
        this IEnumerable<T> query,
        string? keyword,
        List<string>? fields = null,
        int deep = 0
    )
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        SearchResult searchResult = Search<T>(fields, keyword, deep, true);

        return query.Where(
            Expression
                .Lambda<Func<T, bool>>(searchResult.Expression, searchResult.Parameter)
                .Compile()
        );
    }

    private static SearchResult Search<T>(
        IEnumerable<string>? fields,
        string keyword,
        int deep,
        bool isNullCheck = false
    )
    {
        if (deep < 0)
        {
            throw new ArgumentException("Level is invalid", nameof(deep));
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "a");

        return new(
            SearchBodyExpression<T>(parameter, keyword, fields, deep, isNullCheck),
            parameter
        );
    }

    /// <summary>
    /// Create main search expression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="keyword"></param>
    /// <param name="fields"></param>
    /// <param name="deep"></param>
    /// <param name="isNullCheck"> check null if it's IEnumable</param>
    /// <returns></returns>
    private static Expression SearchBodyExpression<T>(
        ParameterExpression parameter,
        string keyword,
        IEnumerable<string>? fields = null,
        int deep = 0,
        bool isNullCheck = false
    )
    {
        Type type = typeof(T);
        ParameterExpression rootParameter = parameter;

        MethodCallExpression constant = Expression.Call(
            Expression.Constant(keyword),
            nameof(string.ToLower),
            Type.EmptyTypes
        );

        Expression? body = null!;
        List<KeyValuePair<PropertyType, string>> searchFields =
            fields?.Any() == true
                ? FilterSearchFields(type, fields)
                : DetectStringProperties(type, deep);

        if (searchFields.Count == 0)
        {
            return body;
        }

        foreach (KeyValuePair<PropertyType, string> field in searchFields)
        {
            Expression expression =
                field.Key == PropertyType.Array
                    ? BuildAnyQuery(type, field.Value, keyword ?? string.Empty, rootParameter, 'a')
                    : BuildContainsQuery(type, field.Value, rootParameter, constant, isNullCheck);

            body = body == null ? expression : Expression.OrElse(body, expression);
        }

        return body;
    }

    /// <summary>
    /// Build deep search by nested any
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="property"></param>
    /// <param name="keyword"></param>
    /// <param name="parameter"></param>
    /// <param name="parameterName"></param>
    /// <param name="body"></param>
    /// <param name="isNullCheck"></param>
    /// <returns></returns>
    static Expression BuildAnyQuery(
        Type type,
        string property,
        string keyword,
        ParameterExpression parameter,
        char parameterName,
        Expression? body = null,
        bool isNullCheck = false
    )
    {
        if (!property.Contains('.'))
        {
            var constant = Expression.Call(
                Expression.Constant(string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword),
                nameof(string.ToLower),
                Type.EmptyTypes
            );

            return BuildContainsQuery(type, property, parameter, constant, isNullCheck);
        }

        var properties = property.Split('.');
        string propertyName = properties[0];
        PropertyInfo propertyInfo = Reflections.PropertyInfoExtensions.GetNestedPropertyInfo(
            type,
            propertyName
        );

        Expression expressionMember = ExpressionExtension.GetExpressionMember(
            propertyName,
            body ?? parameter,
            isNullCheck,
            type
        );

        Type propertyType = propertyInfo.PropertyType;
        if (propertyInfo.IsArrayGenericType())
        {
            propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
            var anyParameter = Expression.Parameter(propertyType, (++parameterName).ToString());
            var contains = BuildAnyQuery(
                propertyType,
                string.Join(".", properties.Skip(1)),
                keyword,
                anyParameter,
                ++parameterName
            );
            var anyLamda = Expression.Lambda(contains, anyParameter);
            var anyCall = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Any),
                [propertyType],
                expressionMember,
                anyLamda
            );
            return anyCall;
        }

        return BuildAnyQuery(
            propertyType,
            string.Join(".", properties.Skip(1)),
            keyword,
            parameter!,
            ++parameterName,
            expressionMember
        );
    }

    /// <summary>
    /// Build contains query
    /// </summary>
    /// <param name="type"></param>
    /// <param name="propertyName"></param>
    /// <param name="parameter"></param>
    /// <param name="keyword"></param>
    /// <param name="isNullCheck"></param>
    /// <returns></returns>
    private static Expression BuildContainsQuery(
        Type type,
        string propertyName,
        Expression parameter,
        MethodCallExpression keyword,
        bool isNullCheck = false
    )
    {
        Expression member = ExpressionExtension.GetExpressionMember(
            propertyName,
            parameter,
            isNullCheck,
            type
        );

        Expression lower = Expression.Call(member, "ToLower", Type.EmptyTypes);
        Expression nullCheck = Expression.Equal(member, Expression.Constant(null));

        return isNullCheck
            ? Expression.Condition(
                nullCheck,
                Expression.Constant(false),
                Expression.Call(lower, "Contains", Type.EmptyTypes, keyword)
            )
            : Expression.Call(lower, "Contains", Type.EmptyTypes, keyword);
    }

    /// <summary>
    /// filter string propertiies of input
    /// </summary>
    /// <param name="type"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    private static List<KeyValuePair<PropertyType, string>> FilterSearchFields(
        Type type,
        IEnumerable<string> properties
    )
    {
        var result = new List<KeyValuePair<PropertyType, string>>();
        foreach (var propertyPath in properties)
        {
            if (
                Reflections
                    .PropertyInfoExtensions.GetNestedPropertyInfo(type, propertyPath)
                    .PropertyType != typeof(string)
            )
            {
                continue;
            }

            Type currentType = type;
            string[] parts = propertyPath.Split('.');
            PropertyType propertyType = PropertyType.Property;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                string propertyName = parts[i];
                PropertyInfo propertyInfo = currentType.GetNestedPropertyInfo(propertyName);
                Type propertyTypeInfo = propertyInfo.PropertyType;

                if (propertyInfo.IsArrayGenericType())
                {
                    propertyType = PropertyType.Array;
                    break;
                }

                if (propertyInfo.IsUserDefineType())
                {
                    propertyType = PropertyType.Object;
                    currentType = propertyTypeInfo;
                }
            }

            result.Add(new KeyValuePair<PropertyType, string>(propertyType, propertyPath));
        }

        return result;
    }

    /// <summary>
    /// Detect string properties automatically
    /// </summary>
    /// <param name="type"></param>
    /// <param name="deep"></param>
    /// <param name="parrentName"></param>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    static List<KeyValuePair<PropertyType, string>> DetectStringProperties(
        Type type,
        int deep = 1,
        string? parrentName = null,
        PropertyType? propertyType = null
    )
    {
        if (deep < 0)
        {
            return [];
        }

        List<KeyValuePair<PropertyType, string>> results = [];

        IEnumerable<PropertyInfo> properties = type.GetProperties();
        List<KeyValuePair<PropertyType, string>> stringProperties = properties
            .Where(x => x.PropertyType == typeof(string))
            .Select(x => new KeyValuePair<PropertyType, string>(
                propertyType ?? PropertyType.Property,
                parrentName != null ? $"{parrentName}.{x.Name}" : x.Name
            ))
            .ToList();

        results.AddRange(stringProperties);

        List<PropertyInfo> collectionObjectProperties = properties
            .Where(x =>
                (x.IsUserDefineType() || x.IsArrayGenericType()) && x.PropertyType != typeof(string)
            )
            .ToList();

        foreach (var propertyInfo in collectionObjectProperties)
        {
            string currentName =
                parrentName != null ? $"{parrentName}.{propertyInfo.Name}" : propertyInfo.Name;

            if (propertyInfo.IsArrayGenericType())
            {
                Type genericType = propertyInfo.PropertyType.GetGenericArguments()[0];
                results.AddRange(
                    DetectStringProperties(genericType, deep - 1, currentName, PropertyType.Array)
                );
            }
            else if (propertyInfo.IsUserDefineType())
            {
                results.AddRange(
                    DetectStringProperties(
                        propertyInfo.PropertyType,
                        deep - 1,
                        currentName,
                        PropertyType.Object
                    )
                );
            }
        }

        return results;
    }

    internal record SearchResult(Expression Expression, ParameterExpression Parameter);
}
