using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Ardalis.GuardClauses;
using Contracts.Guards;

namespace Contracts.Extensions.Reflections;

public static class PropertyInfoExtensions
{
    public static bool IsArrayGenericType(this PropertyInfo propertyInfo) =>
        IsArrayGenericType(propertyInfo.PropertyType);

    public static bool IsArrayGenericType(this Type type)
    {
        if (
            type.IsGenericType
            && typeof(IEnumerable).IsAssignableFrom(type)
            && type.GetGenericArguments()[0].IsUserDefineType()
        )
        {
            return true;
        }
        return false;
    }

    public static PropertyInfo GetNestedPropertyInfo(this Type type, string propertyName)
    {
        // Split the propertyName by '.' to handle nested properties
        var propertyParts = propertyName.Trim().Split('.');

        PropertyInfo? propertyInfo = null;

        // Iterate through each part of the property chain
        foreach (var part in propertyParts)
        {
            // Attempt to find the property information for the current part
            propertyInfo = Guard.Against.NotFound(
                $"{type.FullName}.{propertyName}",
                type.GetProperty(
                    part.Trim(),
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                ),
                nameof(propertyName)
            );

            // Move to the next type in the chain (the type of the current property)
            type = propertyInfo.IsArrayGenericType()
                ? propertyInfo.PropertyType.GetGenericArguments()[0]
                : propertyInfo.PropertyType;
        }

        // Return the last found PropertyInfo (non-null due to Guard.Against)
        return propertyInfo!;
    }

    public static object? GetNestedPropertyValue(this Type type, string propertyName, object target)
    {
        var propertyParts = propertyName.Trim().Split('.');

        PropertyInfo? propertyInfo = null;
        object? objTarget = target;

        foreach (var part in propertyParts)
        {
            if (objTarget == null)
            {
                break;
            }

            propertyInfo = Guard.Against.NotFound(
                $"{type.FullName}.{propertyName}",
                type.GetProperty(
                    part.Trim(),
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                ),
                nameof(propertyName)
            );

            type = propertyInfo.IsArrayGenericType()
                ? propertyInfo.PropertyType.GetGenericArguments()[0]
                : propertyInfo.PropertyType;

            objTarget = propertyInfo.GetValue(objTarget, null);
        }

        return objTarget;
    }

    public static bool IsNestedPropertyValid(this Type type, string propertyName)
    {
        var propertyParts = propertyName.Trim().Split('.');

        foreach (var part in propertyParts)
        {
            PropertyInfo? propertyInfo = type.GetProperty(
                part.Trim(),
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (propertyInfo == null)
            {
                return false;
            }

            type = propertyInfo.IsArrayGenericType()
                ? propertyInfo.PropertyType.GetGenericArguments()[0]
                : propertyInfo.PropertyType;
        }

        return true;
    }

    public static bool IsUserDefineType(this PropertyInfo? propertyInfo) =>
        IsUserDefineType(propertyInfo?.PropertyType);

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
