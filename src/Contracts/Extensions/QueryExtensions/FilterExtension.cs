using System.Linq.Expressions;
using System.Reflection;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Contracts.Extensions.QueryExtensions;

public static class FilterExtension
{
    public static IQueryable<T> Filter<T>(this IQueryable<T> query, object? filterObject)
    {
        if (filterObject == null)
        {
            return query;
        }

        Type type = typeof(T);
        ParameterExpression parameter = Expression.Parameter(type, "a");
        Expression expression = FilterExpression(filterObject, parameter, type, "b");

        var lamda = Expression.Lambda<Func<T, bool>>(expression, parameter);

        return query.Where(lamda);
    }

    private static Expression FilterExpression(
        dynamic filterObject,
        Expression paramOrMember,
        Type type,
        string parameterName
    )
    {
        var dynamicFilters = (IDictionary<string, object>)filterObject;

        Expression body = null!;
        foreach (var dynamicFilter in dynamicFilters)
        {
            string propertyName = dynamicFilter.Key;
            object value = dynamicFilter.Value;

            if (propertyName.Contains('$'))
            {
                Expression left = paramOrMember;

                var a = Compare(propertyName, left, value);

                return a;
            }

            PropertyInfo propertyInfo = type.GetNestedPropertyInfo(propertyName);
            Type propertyType = propertyInfo.PropertyType;

            Expression memeberExpression = ExpressionExtension.GetExpressionMember(
                propertyName,
                paramOrMember,
                false,
                type
            );

            Expression expression = null!;
            if (value is IEnumerable<object> arrayValue) { }
            else
            {
                if (propertyType.IsArrayGenericType())
                {
                    propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
                    ParameterExpression anyParameter = Expression.Parameter(
                        propertyType,
                        parameterName.NextUniformSequence()
                    );

                    Expression operationBody = FilterExpression(
                        value,
                        anyParameter,
                        propertyType,
                        parameterName.NextUniformSequence()
                    );

                    LambdaExpression anyLamda = Expression.Lambda(operationBody, anyParameter);

                    expression = Expression.Call(
                        typeof(Enumerable),
                        nameof(Enumerable.Any),
                        [propertyType],
                        memeberExpression,
                        anyLamda
                    );
                }
                else
                {
                    expression = FilterExpression(
                        value,
                        memeberExpression,
                        propertyType,
                        parameterName.NextUniformSequence()
                    );
                }
            }

            body = body == null ? expression : Expression.AndAlso(body, expression);
        }

        return body;
    }

    /// <summary>
    /// Do compararison
    /// </summary>
    /// <param name="operationString"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private static Expression Compare(string operationString, Expression left, object right)
    {
        OperationType operationType = GetOperationType(operationString);

        bool isFound = BinaryCompararisions.TryGetValue(operationType, out var comparisonFunc);

        if (!isFound)
        {
            // find in another dictionary
            //if(!)
            //{
            //throw
            //}

            //logic
        }
        else
        {
            return CompareBinaryOperations(left, right, comparisonFunc!, operationType);
        }

        return Expression.Empty();
    }

    private static BinaryExpression CompareBinaryOperations(
        Expression left,
        object right,
        Func<Expression, Expression, BinaryExpression> comparisonFunc,
        OperationType operationType
    )
    {
        if (
            operationType.ToString().EndsWith("i", StringComparison.OrdinalIgnoreCase)
            && ((MemberExpression)left).GetMemberExpressionType() == typeof(string)
        )
        {
            MethodCallExpression memeber = Expression.Call(
                left,
                nameof(string.ToLower),
                Type.EmptyTypes
            );
            MethodCallExpression value = Expression.Call(
                Expression.Constant(right),
                nameof(string.ToLower),
                Type.EmptyTypes
            );

            BinaryExpression a = comparisonFunc(memeber, value);
            return a;
        }
        ConvertExpressionTypeResult convert = ParseObject((MemberExpression)left, right);
        BinaryExpression result = comparisonFunc(convert.Member, convert.Value);
        return result;
    }

    /// <summary>
    /// Change both types to the same type
    /// </summary>
    /// <param name="memberExpression"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static ConvertExpressionTypeResult ParseObject(
        MemberExpression memberExpression,
        object value
    )
    {
        Expression member = memberExpression;
        Type memberType = memberExpression.GetMemberExpressionType();

        if (
            (
                memberType.IsNullable()
                && memberType.GenericTypeArguments.Length > 0
                && memberType.GenericTypeArguments[0].IsEnum
            ) || memberType.IsEnum
        )
        {
            Type type = value?.GetType() ?? typeof(long);
            return new(Expression.Convert(member, type), Expression.Constant(value, type));
        }

        if (memberType != value?.GetType())
        {
            Type targetType = memberType;

            if (targetType.IsNullable() && targetType.GenericTypeArguments.Length > 0)
            {
                targetType = targetType.GenericTypeArguments[0];
            }
            object? changedTypeValue = Convert.ChangeType(value, targetType);
            return new(member, Expression.Constant(changedTypeValue, memberType));
        }

        return new(member, Expression.Constant(value));
    }

    /// <summary>
    /// Convert string operation to enum
    /// </summary>
    /// <param name="operationString"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static OperationType GetOperationType(string operationString)
    {
        // Extract the operation substring (remove the first character, e.g., '$')
        string operation = operationString[1..];

        // Try to parse the enum, handling case-insensitive matching
        if (
            Enum.TryParse(typeof(OperationType), operation, true, out var result)
            && result is OperationType parsedOperation
        )
        {
            return parsedOperation;
        }

        // Handle the case where no valid enum was found (optional, you can throw or return default)
        throw new ArgumentException($"Invalid operation: {operationString}");
    }

    private static readonly Dictionary<
        OperationType,
        Func<Expression, Expression, BinaryExpression>
    > BinaryCompararisions =
        new()
        {
            { OperationType.Eq, Expression.Equal },
            { OperationType.EqI, Expression.Equal },
            { OperationType.Ne, Expression.NotEqual },
            { OperationType.NeI, Expression.NotEqual },
            { OperationType.Lt, Expression.LessThan },
            { OperationType.Lte, Expression.LessThanOrEqual },
            { OperationType.Gt, Expression.GreaterThan },
            { OperationType.Gte, Expression.GreaterThanOrEqual },
        };

    /// <summary>
    /// Eq = 1,
    // EqI = 2,
    // Ne = 3,
    // NeI = 4,
    // NotContains = 12,
    // NotContainsI = 13,
    // Contains = 14,
    // ContainsI = 15,
    /// </summary>
    // private static readonly Dictionary<
    //     OperationType,
    //     Func<Expression, Expression, MethodCallExpression>
    // > MethodCallCompararisions =
    //     new()
    //     {
    //         { OperationType.Eq, },
    //         { OperationType.EqI, Expression.Equal },
    //         { OperationType.Ne, Expression.NotEqual },
    //         { OperationType.NeI, Expression.NotEqual },
    //     };
}

internal enum OperationType
{
    Eq = 1,
    EqI = 2,
    Ne = 3,
    NeI = 4,
    In = 5,
    NotIn = 6,
    Lt = 7,
    Lte = 8,
    Gt = 9,
    Gte = 10,
    Between = 11,
    NotContains = 12,
    NotContainsI = 13,
    Contains = 14,
    ContainsI = 15,
}
