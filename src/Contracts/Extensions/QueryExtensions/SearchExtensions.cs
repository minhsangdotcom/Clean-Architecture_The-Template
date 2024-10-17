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
                    ? BuildAnyQuery(new(type, field.Value, rootParameter, keyword, "b", isNullCheck))
                    : BuildContainsQuery(type, field.Value, rootParameter, constant, isNullCheck);

            body = body == null ? expression : Expression.OrElse(body, expression);
        }

        return body;
    }

    /// <summary>
    /// Build deep search by nested any
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    static Expression BuildAnyQuery(BuildAnyQueryPayload payload)
    {
        string propertyPath = payload.PropertyPath;
        string keyword = payload.Keyword;
        Type type = payload.Type;
        bool isNullChecking = payload.IsNullChecking;
        Expression? nullCheck = payload.NullCheck;
        Expression parameterOrMember = payload.ParameterOrMember;

        if (!propertyPath.Contains('.'))
        {
            var constant = Expression.Call(
                Expression.Constant(payload.Keyword),
                nameof(string.ToLower),
                Type.EmptyTypes
            );

            Expression member = parameterOrMember.MemberExpression(type, propertyPath);
            Expression lower = Expression.Call(member, nameof(string.ToLower), Type.EmptyTypes);

            Expression isNull = Expression.Equal(member, Expression.Constant(null));
            Expression notExpression = Expression.Not(isNull);

            return payload.IsNullChecking
                ? Expression.Condition(
                    AndOrNot(notExpression, nullCheck),
                    Expression.Call(lower, nameof(string.Contains), Type.EmptyTypes, constant),
                    Expression.Constant(false)
                )
                : Expression.Call(lower, nameof(string.Contains), Type.EmptyTypes, constant);
        }

        string[] properties = propertyPath.Split('.');
        string propertyName = properties[0];
        PropertyInfo propertyInfo = type.GetNestedPropertyInfo(propertyName);

        Expression expressionMember = parameterOrMember.MemberExpression(type, propertyName);

        if (isNullChecking)
        {
            nullCheck = AndOrNot(Expression.Not(NullOr(expressionMember)), nullCheck);
        }

        Type propertyType = propertyInfo.PropertyType;
        if (propertyType.IsArrayGenericType())
        {
            propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
            ParameterExpression anyParameter = Expression.Parameter(
                propertyType,
                payload.ParameterName.NextUniformSequence()
            );
            Expression contains = BuildAnyQuery(
                new(
                    propertyType,
                    string.Join(".", properties.Skip(1)),
                    anyParameter,
                    keyword,
                    payload.ParameterName.NextUniformSequence(),
                    isNullChecking
                )
            );
            LambdaExpression anyLamda = Expression.Lambda(contains, anyParameter);

            MethodCallExpression anyCall = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Any),
                [propertyType],
                expressionMember,
                anyLamda
            );
            return isNullChecking
                ? Expression.Condition(nullCheck!, anyCall, Expression.Constant(false))
                : anyCall;
        }

        return BuildAnyQuery(
            new(
                propertyType,
                string.Join(".", properties.Skip(1)),
                expressionMember,
                keyword,
                payload.ParameterName.NextUniformSequence(),
                isNullChecking,
                nullCheck
            )
        );
    }

    static BinaryExpression NullOr(Expression expressionMember) =>
        Expression.Equal(expressionMember, Expression.Constant(null));

    static Expression AndOrNot(Expression expression, Expression? root) =>
        root == null ? expression : Expression.AndAlso(root, expression);

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
        if (!isNullCheck)
        {
            Expression member = parameter.MemberExpression(type, propertyName);
            return Expression.Call(
                Expression.Call(member, nameof(string.ToLower), Type.EmptyTypes),
                nameof(string.Contains),
                Type.EmptyTypes,
                keyword
            );
        }

        MemberExpressionResult memberExpressionResult = parameter.MemberExpressionNullCheck(
            type,
            propertyName
        );

        Expression lower = Expression.Call(
            memberExpressionResult.Member,
            nameof(string.ToLower),
            Type.EmptyTypes
        );

        return Expression.Condition(
            memberExpressionResult.NullCheck,
            Expression.Call(lower, nameof(string.Contains), Type.EmptyTypes, keyword),
            Expression.Constant(false)
        );
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

    internal record BuildAnyQueryPayload(
        Type Type,
        string PropertyPath,
        Expression ParameterOrMember,
        string Keyword,
        string ParameterName,
        bool IsNullChecking = false,
        Expression? NullCheck = null
    );
}
