using System.Linq.Expressions;
using System.Reflection;
using Ardalis.GuardClauses;
using Contracts.Guards;

namespace Contracts.Extensions.Reflections;

public static class PropertyInfoExtensions
{
    public static PropertyInfo GetNestedPropertyInfo(this Type type, string propertyName)
    {
        var parts = propertyName.Trim().Split('.');
        PropertyInfo? propertyInfo = null;

        foreach (var part in parts)
        {
            propertyInfo = Guard.Against.NotFound(
                $"{type.FullName}.{propertyName}",
                type.GetProperty(
                    part.Trim(),
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                ),
                nameof(propertyName)
            );

            type = propertyInfo.PropertyType;
        }

        return propertyInfo!;
    }

    public static bool IsUserDefineType(this PropertyInfo? propertyInfo)
    {
        if (propertyInfo == null)
        {
            return false;
        }

        return propertyInfo.PropertyType.IsClass
            && propertyInfo.PropertyType.FullName?.StartsWith("System.") == false;
    }

    public static bool IsUserDefineType(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        return type?.IsClass == true && type?.FullName?.StartsWith("System.") == false;
    }

    public static string GetValue<T>(this T obj, Expression<Func<T, object>> expression)
    {
        PropertyInfo propertyInfo = expression.ToPropertyInfo();

        return propertyInfo.GetValue(obj, null)?.ToString() ?? string.Empty;
    }

    public static PropertyInfo ToPropertyInfo(this Expression expression)
    {
        LambdaExpression lambda = Guard.Against.ConvertLamda(expression);

        ExpressionType expressionType = lambda.Body.NodeType;

        MemberExpression? memberExpr = expressionType switch
        {
            ExpressionType.Convert => ((UnaryExpression)lambda.Body).Operand as MemberExpression,
            ExpressionType.MemberAccess => lambda.Body as MemberExpression,
            _ => throw new Exception("Expression Type is not support"),
        };

        return (PropertyInfo)memberExpr!.Member;
    }
}
