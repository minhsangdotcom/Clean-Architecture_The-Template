using System.Linq.Expressions;
using System.Reflection;
using Ardalis.GuardClauses;
using Contracts.Extensions.Reflections;
using Contracts.Guards;

namespace Contracts.Extensions.Expressions;

public static class ExpressionExtension
{
    public static Expression GetExpressionMember<T>(
        string property,
        Expression expression,
        bool isNullCheck
    ) => GetExpressionMember(property, expression, isNullCheck, typeof(T));

    public static Expression GetExpressionMember(
        string propertyPath,
        Expression expression,
        bool isNullCheck,
        Type entityType
    )
    {
        Type type = entityType;
        string[] properties = propertyPath.Trim().Split('.', StringSplitOptions.TrimEntries);

        Expression propertyValue = expression;
        Expression nullCheck = null!;

        foreach (string property in properties)
        {
            PropertyInfo propertyInfo = type.GetNestedPropertyInfo(property);

            try
            {
                propertyValue = Expression.PropertyOrField(propertyValue, property);
            }
            catch (ArgumentException)
            {
                propertyValue = Expression.MakeMemberAccess(propertyValue, propertyInfo);
            }

            if (isNullCheck)
            {
                nullCheck = GenerateNullCheckExpression(propertyValue, nullCheck);
            }

            type = propertyInfo.PropertyType;
        }

        return nullCheck == null
            ? propertyValue
            : Expression.Condition(
                nullCheck,
                Expression.Default(propertyValue.Type),
                propertyValue
            );
    }

    public static PropertyInfo ToPropertyInfo(Expression expression)
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

    public static string ToStringProperty(this Expression expression)
    {
        LambdaExpression lambda = Guard.Against.ConvertLamda(expression);

        var stack = new Stack<string>();

        ExpressionType expressionType = lambda.Body.NodeType;

        MemberExpression? memberExpression = true switch
        {
            bool
                when expressionType == ExpressionType.Convert
                    || expressionType == ExpressionType.ConvertChecked => (
                lambda.Body as UnaryExpression
            )?.Operand as MemberExpression,
            bool when expressionType == ExpressionType.MemberAccess => lambda.Body
                as MemberExpression,
            _ => throw new Exception("Expression Type is not support"),
        };

        while (memberExpression != null)
        {
            stack.Push(memberExpression.Member.Name);
            memberExpression = memberExpression.Expression as MemberExpression;
        }

        return string.Join(".", [.. stack]);
    }

    public static Type GetMemberExpressionType(this MemberExpression expression)
    {
        MemberExpression memberExpression = Guard.Against.ConvertMember(expression);

        return memberExpression.Member switch
        {
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
            _ => throw new ArgumentException(
                $"{memberExpression.Member} neither a property nor a field "
            ),
        };
    }

    private static BinaryExpression GenerateNullCheckExpression(
        Expression propertyValue,
        Expression nullCheckExpression
    ) =>
        nullCheckExpression == null
            ? Expression.Equal(propertyValue, Expression.Default(propertyValue.Type))
            : Expression.OrElse(
                nullCheckExpression,
                Expression.Equal(propertyValue, Expression.Default(propertyValue.Type))
            );
}
