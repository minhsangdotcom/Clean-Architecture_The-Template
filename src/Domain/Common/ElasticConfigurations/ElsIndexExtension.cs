using System.Linq.Expressions;
using System.Reflection;
using CaseConverter;
using Contracts.Extensions;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Domain.Common.ElasticConfigurations;

public static class ElsIndexExtension
{
    /// <summary>
    /// get Index name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string GetName<T>() => $"{ElsPrefix.prefix}{typeof(T).Name}".Underscored();

    /// <summary>
    /// Get sub field keyword
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string GetKeywordName<T>(Expression<Func<T, object>> expression)
    {
        PropertyInfo propertyInfo = ExpressionExtension.ToPropertyInfo(expression);
        return $"{propertyInfo.Name.FirstCharToLowerCase()}{ElsPrefix.KeywordPrefixName}";
    }

    public static string GetKeywordName<T>(string propertyName)
    {
        PropertyInfo propertyInfo = propertyName.Contains('.')
            ? typeof(T).GetNestedPropertyInfo(propertyName)
            : typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            ) ?? throw new ArgumentException($"{propertyName} is not found.");

        return $"{propertyInfo.Name.FirstCharToLowerCase()}{ElsPrefix.KeywordPrefixName}";
    }
}
