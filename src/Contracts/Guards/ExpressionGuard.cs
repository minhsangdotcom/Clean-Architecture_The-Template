using System.Linq.Expressions;
using Ardalis.GuardClauses;

namespace Contracts.Guards;

public static class ExpressionGuard
{
    public static LambdaExpression ConvertLamda(this IGuardClause guardClause, Expression expression)
    {
        if (expression == null)
        {
            Guard.Against.Null(expression, nameof(expression), "Expression must be not null.");
        }

        if (expression is not LambdaExpression lamda)
        {
            throw new ArgumentException($"Can not parse {expression} to LambdaExpression");
        }

        return lamda;
    }

    public static MemberExpression ConvertMember(this IGuardClause guardClause, Expression expression)
    {
        if (expression == null)
        {
            Guard.Against.Null(expression, nameof(expression), "Expression must be not null.");
        }

        if (expression is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"Can not parse {expression} to MemberExpression");
        }

        return memberExpression;
    }
}