using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Ardalis.GuardClauses;
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
    private static BinaryExpression Compare(string operationString, Expression left, object right)
    {
        OperationType operationType = GetOperationType(operationString);

        bool isFound = BinaryCompararisions.TryGetValue(operationType, out var comparisonFunc);

        if (!isFound)
        {
            // find in another dictionary
            if (!MethodCallCompararisions.TryGetValue(operationType, out var callMethodType))
            {
                throw new NotFoundException(nameof(operationType), nameof(operationType));
            }

            return CompareMethodCallOpertations(left, right, callMethodType, operationType);
        }

        return CompareBinaryOperations(left, right, comparisonFunc!, operationType);
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
            MethodCallExpression member = Expression.Call(
                left,
                nameof(string.ToLower),
                Type.EmptyTypes
            );
            MethodCallExpression value = Expression.Call(
                Expression.Constant(right),
                nameof(string.ToLower),
                Type.EmptyTypes
            );

            BinaryExpression a = comparisonFunc(member, value);
            return a;
        }
        ConvertExpressionTypeResult convert = ParseObject((MemberExpression)left, right);
        BinaryExpression result = comparisonFunc(convert.Member, convert.Value);
        return result;
    }

    private static BinaryExpression CompareMethodCallOpertations(
        Expression left,
        object right,
        KeyValuePair<Type, string> callMethodType,
        OperationType operationType
    )
    {
        if (operationType == OperationType.In || operationType == OperationType.NotIn)
        {
            ConvertExpressionTypeResult convertExpressionType = ParseArray(
                (MemberExpression)left,
                right
            );

            MethodInfo methodInfo = callMethodType
                .Key.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == callMethodType.Value && m.GetParameters().Length == 2)
                .MakeGenericMethod(convertExpressionType.Type!);

            Expression expression = Expression.Call(
                methodInfo,
                convertExpressionType.Value,
                convertExpressionType.Member
            );

            return NotOr(operationType, expression);
        }

        Expression outter = left;
        Expression inner = Expression.Constant(right);
        if (operationType.ToString().EndsWith("i", StringComparison.OrdinalIgnoreCase))
        {
            outter = Expression.Call(left, nameof(string.ToLower), Type.EmptyTypes);
            inner = Expression.Call(
                Expression.Constant(right),
                nameof(string.ToLower),
                Type.EmptyTypes
            );
        }

        MethodCallExpression result = Expression.Call(
            outter,
            callMethodType.Key.GetMethod(callMethodType.Value, [typeof(string)])!,
            inner
        );

        return NotOr(operationType, result);
    }

    private static BinaryExpression NotOr(OperationType operationType, Expression expression) =>
        operationType.ToString().StartsWith("not", StringComparison.OrdinalIgnoreCase)
            ? Expression.Equal(expression, Expression.Constant(false))
            : Expression.Equal(expression, Expression.Constant(true));

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
        ConvertObjectTypeResult parse = Parse(memberExpression, value);
        return new(parse.Member, (ConstantExpression)parse.Value!);
    }

    /// <summary>
    /// Change both types to the same type for array value in $in and $notIn
    /// </summary>
    /// <param name="memberExpression"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    private static ConvertExpressionTypeResult ParseArray(
        MemberExpression memberExpression,
        object values
    )
    {
        List<object?> list = [];
        Expression member = null!;
        Type type = null!;
        foreach (object value in (IList)values)
        {
            var result = Parse(memberExpression, value, true);
            member = result.Member;
            type = result.Type;
            list.Add(result.Value);
        }

        (IList list, Type type) convertListResult = ConvertListToType(list!, type);
        return new(
            member,
            Expression.Constant(convertListResult.list, convertListResult.type),
            type
        );
    }

    /// <summary>
    /// Convert list object to explicit type
    /// </summary>
    /// <param name="sourceList"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    static (IList, Type) ConvertListToType(List<object> sourceList, Type targetType)
    {
        // Step 1: Create a generic List<T> where T is the targetType
        Type listType = typeof(List<>).MakeGenericType(targetType);
        IList typedList = (IList)Activator.CreateInstance(listType)!;

        // Step 2: Iterate through the source list and convert each item to the targetType
        foreach (var item in sourceList)
        {
            // Step 3: Add each item to the typed list (casting to the target type)
            typedList.Add(Convert.ChangeType(item, targetType));
        }

        return new(typedList, listType);
    }

    private static ConvertObjectTypeResult Parse(
        MemberExpression memberExpression,
        object value,
        bool isRawValue = false
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
            return new(
                Expression.Convert(member, type),
                isRawValue ? value : Expression.Constant(value, type),
                type
            );
        }

        if (memberType != value?.GetType())
        {
            Type targetType = memberType;

            if (targetType.IsNullable() && targetType.GenericTypeArguments.Length > 0)
            {
                targetType = targetType.GenericTypeArguments[0];
            }
            object? changedTypeValue = Convert.ChangeType(value, targetType);
            return new(
                member,
                isRawValue ? changedTypeValue : Expression.Constant(changedTypeValue, memberType),
                memberType
            );
        }

        return new(member, isRawValue ? value : Expression.Constant(value), memberType);
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

    private static readonly Dictionary<
        OperationType,
        KeyValuePair<Type, string>
    > MethodCallCompararisions =
        new()
        {
            { OperationType.In, new(typeof(Enumerable), nameof(Enumerable.Contains)) },
            { OperationType.NotIn, new(typeof(Enumerable), nameof(Enumerable.Contains)) },
            { OperationType.Contains, new(typeof(string), nameof(string.Contains)) },
            { OperationType.ContainsI, new(typeof(string), nameof(string.Contains)) },
            { OperationType.NotContains, new(typeof(string), nameof(string.Contains)) },
            { OperationType.NotContainsI, new(typeof(string), nameof(string.Contains)) },
            { OperationType.StartsWith, new(typeof(string), nameof(string.StartsWith)) },
            { OperationType.EndsWith, new(typeof(string), nameof(string.EndsWith)) },
        };
}

internal record ConvertObjectTypeResult(Expression Member, object? Value, Type Type);

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
    StartsWith = 16,
    EndsWith = 17,
}
